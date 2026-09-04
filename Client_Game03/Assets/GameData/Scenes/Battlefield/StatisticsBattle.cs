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

        private readonly List<StatisticsHero> list_StatisticsHero = new();

        private int turnAdded = 0;

        public void AddHero(Guid heroId, bool inTeam1)
        {
            list_StatisticsHero.Add(new StatisticsHero(heroId, inTeam1));
        }
        public void Update()
        {
            int i = battlefieldSceneInitializator.battlefieldIndexAnimationStarted;
            IEnumerable<BattlefieldLogRecordBase> logs = BattlefieldSceneInitializator.spawnedBattlefield.battlefieldLog.Where(a => a.index <= i && a.index >= turnAdded);
            foreach (BattlefieldLogRecordBase log in logs)
            {
                switch (log)
                {
                    case BattlefieldLogRecord_Damage d:

                        // Запись нанесённого урона
                        {
                            var v = list_StatisticsHero.First(a => a.heroId == d.hero1Id);
                            v.damageDone += d.damage;
                        }


                        // Запись полученного урона
                        {
                            var v = list_StatisticsHero.First(a => a.heroId == d.hero2Id);
                            v.damageReceived += d.damage;
                        }

                        break;
                        //case BattlefieldLogRecord_TurnStart t:
                        //    break;
                }
            }

            turnAdded = i + 1;
        }
    }
}
