using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.GameData.Scenes.Battlefield
{
    public class StatisticsBattle
    {
        private readonly BattlefieldSceneInitializator battlefieldSceneInitializator;
        public StatisticsBattle(BattlefieldSceneInitializator battlefieldSceneInitializator)
        {
            this.battlefieldSceneInitializator = battlefieldSceneInitializator;
        }

        public List<StatisticsHero> list_StatisticsHero { get; private set; } = new();

        private int turnAdded = 0;

        public void AddHero(Guid heroId, bool inTeam1, string name)
        {
            list_StatisticsHero.Add(new StatisticsHero(heroId, inTeam1, name));
        }

        public void Update()
        {
            int i = battlefieldSceneInitializator.battlefieldIndexAnimationStarted+1;
            IEnumerable<BattlefieldLogRecordBase> logs = BattlefieldSceneInitializator.spawnedBattlefield.battlefieldLog.Where(a => a.index <= i && a.index >= turnAdded);
            foreach (BattlefieldLogRecordBase log in logs)
            {
                switch (log)
                {
                    case BattlefieldLogRecord_Damage d:

                        // Запись нанесённого урона
                        {
                            StatisticsHero v = list_StatisticsHero.First(a => a.heroId == d.hero1Id);
                            v.damageDone += d.damage;
                        }


                        // Запись полученного урона
                        {
                            StatisticsHero v = list_StatisticsHero.First(a => a.heroId == d.hero2Id);
                            v.damageReceived += d.damage;
                        }

                        break;
                        //case BattlefieldLogRecord_TurnStart t:
                        //    break;
                }
            }

            turnAdded = i + 1;

            list_StatisticsHero.Sort((a, b) => b.damageDone.CompareTo(a.damageDone));
        }
    }
}
