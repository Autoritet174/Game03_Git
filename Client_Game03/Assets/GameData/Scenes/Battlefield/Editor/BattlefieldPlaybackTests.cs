using Assets.GameData.Scenes.Battlefield.Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.DTO.Battlefield;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield.Tests
{
    /// <summary>Регрессионные проверки порядка событий, ожиданий, статистики и жизненного цикла DOTween.</summary>
    public class BattlefieldPlaybackTests
    {
        /// <summary>Записывает имя выполняемой проверки в журнал пакетного запуска Unity.</summary>
        [SetUp]
        public void LogTestStart()
        {
            Debug.Log($"Battlefield test: {TestContext.CurrentContext.Test.Name}");
        }

        /// <summary>Воспроизводит реальный пример полностью и сверяет каждое последствие и итоговую статистику.</summary>
        [Test]
        public async Task SampleLogAppliesEveryRecordAndDamageExactlyOnce()
        {
            string json = File.ReadAllText(Path.Combine(Application.dataPath, "../TestData/пример лога.txt"));
            List<BattlefieldLogRecordBase> records = JSON.Deserialize<List<BattlefieldLogRecordBase>>(json);
            BattlefieldLogPlayer player = new(records, Assert.Fail);
            List<int> processed = new();
            StatisticsBattle statistics = new();
            BattlefieldLogRecord_Damage[] damages = records.OfType<BattlefieldLogRecord_Damage>().ToArray();
            foreach (Guid id in damages.SelectMany(damage => new[] { damage.hero1Id, damage.hero2Id }).Distinct())
                statistics.AddHero(id, true, id.ToString());
            player.RecordStarted += processed.Add;
            player.RegisterRecord<BattlefieldLogRecord_TurnStart>((record, token) => UniTask.CompletedTask);
            player.RegisterRecord<BattlefieldLogRecord_ChangeActionPoints>((record, token) => UniTask.CompletedTask);
            player.RegisterRecord<BattlefieldLogRecord_Damage>((record, token) =>
            {
                statistics.ApplyDamage(record);
                return UniTask.CompletedTask;
            });
            player.RegisterImpactEffect<BattlefieldLogRecord_Damage>(record => record.isPerodic ? null : record.indexReason);
            player.RegisterAbility(EBattlefieldLogAbility.attack, (record, impact, token) => impact(token));

            await player.PlayAsync(CancellationToken.None);

            CollectionAssert.AreEqual(records.OrderBy(record => record.index).Select(record => record.index), processed);
            Assert.That(damages.Length, Is.EqualTo(64));
            foreach (StatisticsHero hero in statistics.list_StatisticsHero)
            {
                Assert.That(hero.damageDone, Is.EqualTo(damages.Where(damage => damage.hero1Id == hero.heroId).Sum(damage => damage.damage)));
                Assert.That(hero.damageReceived, Is.EqualTo(damages.Where(damage => damage.hero2Id == hero.heroId).Sum(damage => damage.damage)));
            }
        }

        /// <summary>Проверяет ожидание попадания и возврата, цепочки эффектов, нулевой индекс, пропуски и отложенный урон.</summary>
        [Test]
        public async Task ImpactAndReturnAreAwaitedWithoutPullingPeriodicOrLaterDamageForward()
        {
            List<BattlefieldLogRecordBase> records = ParseRecords(
                "{\"$type\":\"turn_start\",\"index\":90,\"turn\":2}," +
                AbilityJson(10) + "," + DamageJson(20, 10) + "," + DamageJson(40, 20) + "," +
                DamageJson(50, 10, true) + "," + DamageJson(100, 10) + "," +
                "{\"$type\":\"turn_start\",\"index\":0,\"turn\":1}");
            BattlefieldLogPlayer player = new(records, Assert.Fail);
            UniTaskCompletionSource impactGate = new();
            UniTaskCompletionSource returnGate = new();
            List<int> processed = new();
            List<int> impactRecords = new();
            bool duringImpact = false;
            player.RecordStarted += processed.Add;
            player.RegisterRecord<BattlefieldLogRecord_TurnStart>((record, token) => UniTask.CompletedTask);
            player.RegisterRecord<BattlefieldLogRecord_Damage>((record, token) =>
            {
                if (duringImpact)
                    impactRecords.Add(record.index);
                return UniTask.CompletedTask;
            });
            player.RegisterImpactEffect<BattlefieldLogRecord_Damage>(record => record.isPerodic ? null : record.indexReason);
            player.RegisterAbility(EBattlefieldLogAbility.attack, async (record, impact, token) =>
            {
                await impactGate.Task;
                duringImpact = true;
                await impact(token);
                await impact(token);
                duringImpact = false;
                await returnGate.Task;
            });

            Task playback = player.PlayAsync(CancellationToken.None).AsTask();
            CollectionAssert.AreEqual(new[] { 0, 10 }, processed);
            Assert.That(playback.IsCompleted, Is.False);
            impactGate.TrySetResult();
            CollectionAssert.AreEqual(new[] { 20, 40 }, impactRecords);
            CollectionAssert.AreEqual(new[] { 0, 10, 20, 40 }, processed);
            Assert.That(playback.IsCompleted, Is.False);
            returnGate.TrySetResult();
            await playback;
            CollectionAssert.AreEqual(new[] { 0, 10, 20, 40, 50, 90, 100 }, processed);
        }

        /// <summary>Неизвестная способность сохраняет свои известные последствия, даже если у неё нет целей для специальной анимации.</summary>
        [Test]
        public async Task UnknownAbilityStillAppliesKnownConsequences()
        {
            List<string> warnings = new();
            BattlefieldLogPlayer player = new(ParseRecords(AbilityJson(1, 999) + "," + DamageJson(2, 1)), warnings.Add);
            int hits = 0;
            player.RegisterRecord<BattlefieldLogRecord_Damage>((record, token) => { hits++; return UniTask.CompletedTask; });
            player.RegisterImpactEffect<BattlefieldLogRecord_Damage>(record => record.indexReason);
            await player.PlayAsync(CancellationToken.None);
            Assert.That(hits, Is.EqualTo(1));
            Assert.That(warnings.Count, Is.EqualTo(1));
        }

        /// <summary>Отмена после подготовки атаки не позволяет применить её последствия.</summary>
        [Test]
        public void CancellationBeforeImpactPreventsDamage()
        {
            using CancellationTokenSource cancellation = new();
            BattlefieldLogPlayer player = new(ParseRecords(AbilityJson(1) + "," + DamageJson(2, 1)), Assert.Fail);
            int hits = 0;
            player.RegisterRecord<BattlefieldLogRecord_Damage>((record, token) => { hits++; return UniTask.CompletedTask; });
            player.RegisterImpactEffect<BattlefieldLogRecord_Damage>(record => record.indexReason);
            player.RegisterAbility(EBattlefieldLogAbility.attack, (record, impact, token) =>
            {
                cancellation.Cancel();
                return impact(token);
            });
            Assert.CatchAsync<OperationCanceledException>(async () => await player.PlayAsync(cancellation.Token));
            Assert.That(hits, Is.Zero);
        }

        /// <summary>Повторный серверный индекс отвергается до начала проигрывания.</summary>
        [Test]
        public void DuplicateIndicesAreRejected()
        {
            Assert.Throws<ArgumentException>(() => new BattlefieldLogPlayer(ParseRecords(AbilityJson(1) + "," + DamageJson(1, 1)), Assert.Fail));
        }

        /// <summary>Повторное начисление одного последствия не удваивает статистику.</summary>
        [Test]
        public void StatisticsAreIdempotent()
        {
            BattlefieldLogRecord_Damage damage = (BattlefieldLogRecord_Damage)ParseRecords(DamageJson(2, 1))[0];
            StatisticsBattle statistics = new();
            statistics.AddHero(damage.hero1Id, true, "Source");
            statistics.AddHero(damage.hero2Id, false, "Target");
            statistics.ApplyDamage(damage);
            statistics.ApplyDamage(damage);
            Assert.That(statistics.list_StatisticsHero.Sum(hero => hero.damageDone), Is.EqualTo(25f));
            Assert.That(statistics.list_StatisticsHero.Sum(hero => hero.damageReceived), Is.EqualTo(25f));
        }

        /// <summary>Пустой лог завершается сразу и не требует отдельного кадра.</summary>
        [Test]
        public async Task EmptyLogCompletes()
        {
            await new BattlefieldLogPlayer(Array.Empty<BattlefieldLogRecordBase>(), Assert.Fail).PlayAsync(CancellationToken.None);
        }

        /// <summary>Смена скорости немедленно влияет на активный tween и корректно завершает await.</summary>
        [Test]
        public async Task SpeedChangesAffectRunningTween()
        {
            using BattlefieldAnimationPlayer player = new(1f);
            float value = 0;
            Tween tween = DOTween.To(() => value, next => value = next, 10f, 1f).SetEase(Ease.Linear);
            Task playback = player.PlayAsync(tween, CancellationToken.None).AsTask();
            tween.SetUpdate(UpdateType.Manual, true);
            DOTween.ManualUpdate(0.25f, 0.25f);
            Assert.That(value, Is.EqualTo(2.5f).Within(0.001f));
            player.SetSpeed(2f);
            DOTween.ManualUpdate(0.25f, 0.25f);
            Assert.That(value, Is.EqualTo(7.5f).Within(0.001f));
            player.SetSpeed(5f);
            DOTween.ManualUpdate(0.1f, 0.1f);
            Assert.That(playback.IsCompleted, Is.True, "DOTween должен завершить ожидание через callback.");
            await playback;
            Assert.That(value, Is.EqualTo(10f));
            Assert.That(tween.IsActive(), Is.False);
        }

        /// <summary>Отмена уничтожает tween, не выполняя callback успешного завершения.</summary>
        [Test]
        public void CancellationKillsTweenWithoutCompletingIt()
        {
            using BattlefieldAnimationPlayer player = new(1f);
            using CancellationTokenSource cancellation = new();
            float value = 0;
            int completions = 0;
            Tween tween = DOTween.To(() => value, next => value = next, 10f, 1f).OnComplete(() => completions++);
            Task playback = player.PlayAsync(tween, cancellation.Token).AsTask();
            cancellation.Cancel();
            Assert.That(playback.IsCompleted, Is.True, "Отмена не должна ждать следующего кадра.");
            Assert.CatchAsync<OperationCanceledException>(async () => await playback);
            Assert.That(completions, Is.Zero);
            Assert.That(value, Is.Zero);
            Assert.That(tween.IsActive(), Is.False);
        }

        /// <summary>Отключение одного боя не уничтожает анимации другого владельца.</summary>
        [Test]
        public async Task DisposingPlayerOnlyKillsItsOwnTweens()
        {
            using BattlefieldAnimationPlayer first = new(1f);
            using BattlefieldAnimationPlayer second = new(1f);
            float firstValue = 0;
            float secondValue = 0;
            Tween firstTween = DOTween.To(() => firstValue, next => firstValue = next, 1f, 1f);
            Tween secondTween = DOTween.To(() => secondValue, next => secondValue = next, 1f, 1f);
            Task firstTask = first.PlayAsync(firstTween, CancellationToken.None).AsTask();
            Task secondTask = second.PlayAsync(secondTween, CancellationToken.None).AsTask();
            secondTween.SetUpdate(UpdateType.Manual, true);
            first.Dispose();
            Assert.That(firstTask.IsCompleted, Is.True, "Dispose должен освободить ожидание без кадра.");
            Assert.CatchAsync<OperationCanceledException>(async () => await firstTask);
            Assert.That(secondTween.IsActive(), Is.True);
            DOTween.ManualUpdate(1f, 1f);
            Assert.That(secondTask.IsCompleted, Is.True);
            await secondTask;
            Assert.That(secondValue, Is.EqualTo(1f));
        }

        /// <summary>Десериализует фрагменты тестового лога тем же сериализатором, что используется в клиенте.</summary>
        private static List<BattlefieldLogRecordBase> ParseRecords(string records)
        {
            return JSON.Deserialize<List<BattlefieldLogRecordBase>>("[" + records + "]");
        }

        /// <summary>Создаёт JSON записи способности с заданным индексом и видом.</summary>
        private static string AbilityJson(int index, int ability = 1)
        {
            return $"{{\"$type\":\"use_ability\",\"index\":{index},\"spawnedHero1Id\":\"00000000-0000-0000-0000-000000000001\",\"ability\":{ability},\"spawnedHeroTargets\":[]}}";
        }

        /// <summary>Создаёт JSON последствия с заданной причиной и признаком периодического урона.</summary>
        private static string DamageJson(int index, int reason, bool periodic = false)
        {
            return $"{{\"$type\":\"damage\",\"index\":{index},\"indexReason\":{reason},\"hero1Id\":\"00000000-0000-0000-0000-000000000001\",\"hero2Id\":\"00000000-0000-0000-0000-000000000002\",\"damage\":25,\"isPerodic\":{periodic.ToString().ToLowerInvariant()}}}";
        }
    }
}
