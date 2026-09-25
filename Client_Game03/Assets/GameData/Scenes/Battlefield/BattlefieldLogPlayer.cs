using Cysharp.Threading.Tasks;
using General;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Последовательно воспроизводит серверные события через заменяемые обработчики записей и способностей.</summary>
    public sealed class BattlefieldLogPlayer
    {
        /// <summary>Упорядоченная копия лога; серверные индексы не обязаны идти подряд или начинаться с единицы.</summary>
        private readonly BattlefieldLogRecordBase[] records;

        /// <summary>Обработчики известных типов серверных записей.</summary>
        private readonly Dictionary<Type, Func<BattlefieldLogRecordBase, CancellationToken, UniTask>> handlers = new();

        /// <summary>Способы получения причины немедленного эффекта; отложенные эффекты возвращают null.</summary>
        private readonly Dictionary<Type, Func<BattlefieldLogRecordBase, int?>> impactReasons = new();

        /// <summary>Анимации способностей, вызывающие переданный обработчик в момент попадания.</summary>
        private readonly Dictionary<EBattlefieldLogAbility, Func<BattlefieldLogRecord_UseAbility, Func<CancellationToken, UniTask>, CancellationToken, UniTask>> abilities = new();

        /// <summary>Вывод диагностических сообщений для неподдерживаемых событий.</summary>
        private readonly Action<string> warning;

        /// <summary>Сообщает фактический индекс обрабатываемой записи.</summary>
        public event Action<int> recordStarted;

        /// <summary>Проверяет идентификаторы и сохраняет отсортированный лог.</summary>
        public BattlefieldLogPlayer(IEnumerable<BattlefieldLogRecordBase> records, Action<string> warning)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.records = records.ToArray();
            if (this.records.Any(record => record == null))
            {
                throw new ArgumentException("Лог содержит пустую запись.", nameof(records));
            }

            Array.Sort(this.records, (left, right) => left.index.CompareTo(right.index));
            if (this.records.Select(record => record.index).Distinct().Count() != this.records.Length)
            {
                throw new ArgumentException("Индексы записей боя должны быть уникальны.", nameof(records));
            }

            this.warning = warning ?? (_ => { });
        }

        #region Регистрация обработчиков

        /// <summary>Регистрирует обработчик нового типа записи без изменения основного цикла воспроизведения.</summary>
        public void RegisterRecord<T>(Func<T, CancellationToken, UniTask> handler) where T : BattlefieldLogRecordBase
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            handlers[typeof(T)] = (record, token) => handler((T)record, token);
        }

        /// <summary>Позволяет типу эффекта воспроизводиться при попадании, если он непосредственно следует за своей причиной.</summary>
        public void RegisterImpactEffect<T>(Func<T, int?> getReason) where T : BattlefieldLogRecordBase
        {
            if (getReason == null)
            {
                throw new ArgumentNullException(nameof(getReason));
            }

            impactReasons[typeof(T)] = record => getReason((T)record);
        }

        /// <summary>Регистрирует анимацию способности; последствия должны запускаться через переданный callback попадания.</summary>
        public void RegisterAbility(EBattlefieldLogAbility ability,
            Func<BattlefieldLogRecord_UseAbility, Func<CancellationToken, UniTask>, CancellationToken, UniTask> handler)
        {
            abilities[ability] = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        #endregion Регистрация обработчиков

        #region Воспроизведение записей

        /// <summary>Ожидает каждое действие по порядку, не пересматривая уже обработанные записи на следующих кадрах.</summary>
        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            for (int position = 0; position < records.Length; position++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                BattlefieldLogRecordBase record = records[position];
                if (record is BattlefieldLogRecord_UseAbility ability)
                {
                    recordStarted?.Invoke(ability.index);
                    int effectsEnd = FindImpactEffectsEnd(position, ability.index);
                    await PlayAbilityAsync(ability, position + 1, effectsEnd, cancellationToken);
                    position = effectsEnd - 1;
                }
                else
                {
                    await DispatchAsync(record, cancellationToken);
                }
            }
        }

        /// <summary>Выделяет последовательные немедленные последствия, включая цепочки причин, не перенося события через другие действия.</summary>
        private int FindImpactEffectsEnd(int position, int reason)
        {
            HashSet<int> reasons = new() { reason };
            int end = position + 1;
            while (end < records.Length && impactReasons.TryGetValue(records[end].GetType(), out Func<BattlefieldLogRecordBase, int?> getReason))
            {
                int? parent = getReason(records[end]);
                if (!parent.HasValue || !reasons.Contains(parent.Value))
                {
                    break;
                }

                _ = reasons.Add(records[end].index);
                end++;
            }
            return end;
        }

        /// <summary>Воспроизводит способность и ровно один раз применяет её последствия, включая универсальный вариант неизвестной способности.</summary>
        private async UniTask PlayAbilityAsync(BattlefieldLogRecord_UseAbility ability, int start, int end, CancellationToken token)
        {
            bool impactPlayed = false;

            /// <summary>Последовательно применяет немедленные последствия в момент попадания.</summary>
            async UniTask ImpactAsync(CancellationToken impactToken)
            {
                impactToken.ThrowIfCancellationRequested();
                if (impactPlayed)
                {
                    return;
                }

                impactPlayed = true;
                for (int i = start; i < end; i++)
                {
                    await DispatchAsync(records[i], impactToken);
                }
            }

            if (abilities.TryGetValue(ability.ability, out Func<BattlefieldLogRecord_UseAbility, Func<CancellationToken, UniTask>, CancellationToken, UniTask> handler))
            {
                await handler(ability, ImpactAsync, token);
            }
            else
            {
                warning($"Способность {ability.ability}: отображаются известные последствия без специальной анимации.");
            }

            token.ThrowIfCancellationRequested();
            if (!impactPlayed)
            {
                await ImpactAsync(token);
            }
        }

        /// <summary>Передаёт запись её обработчику или диагностирует неизвестный тип, не останавливая оставшийся лог.</summary>
        private UniTask DispatchAsync(BattlefieldLogRecordBase record, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            recordStarted?.Invoke(record.index);
            if (handlers.TryGetValue(record.GetType(), out Func<BattlefieldLogRecordBase, CancellationToken, UniTask> handler))
            {
                return handler(record, token);
            }

            warning($"Нет обработчика события {record.GetType().Name}, индекс {record.index}.");
            return UniTask.CompletedTask;
        }

        #endregion Воспроизведение записей
    }
}
