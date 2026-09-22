using General.DTO.Battlefield;
using System;
using System.Collections.Generic;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Накапливает статистику только по уже воспроизведённым последствиям боя.</summary>
    public class StatisticsBattle
    {
        /// <summary>Статистика участников в порядке регистрации для отображения на панели.</summary>
        public List<StatisticsHero> list_StatisticsHero { get; } = new();

        /// <summary>Быстрый доступ к статистике героя по серверному идентификатору.</summary>
        private readonly Dictionary<Guid, StatisticsHero> heroes = new();

        /// <summary>Индексы уже учтённых последствий для защиты от повторного начисления.</summary>
        private readonly HashSet<int> appliedRecords = new();

        /// <summary>Регистрирует участника боя с нулевыми начальными показателями.</summary>
        public void AddHero(Guid heroId, bool inTeam1, string name)
        {
            StatisticsHero hero = new(heroId, inTeam1, name);
            heroes.Add(heroId, hero);
            list_StatisticsHero.Add(hero);
        }

        /// <summary>Учитывает конкретное показанное событие без поиска будущих записей в логе.</summary>
        public void ApplyDamage(BattlefieldLogRecord_Damage record)
        {
            if (!appliedRecords.Add(record.index))
                return;
            if (heroes.TryGetValue(record.hero1Id, out StatisticsHero source))
                source.damageDone += record.damage;
            if (heroes.TryGetValue(record.hero2Id, out StatisticsHero target))
                target.damageReceived += record.damage;
        }
    }
}
