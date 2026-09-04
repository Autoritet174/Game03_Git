using System;

namespace Assets.GameData.Scenes.Battlefield
{
    public class StatisticsHero
    {
        public StatisticsHero(Guid heroId, bool inTeam1)
        {
            this.heroId = heroId;
            this.inTeam1 = inTeam1;
        }

        /// <summary> Существование приватного конструктора блокирует создание без параметров. </summary>
        private StatisticsHero() { }

        public Guid heroId { get; }
        public bool inTeam1 { get; }
        public float damageDone { get; set; }
        public float damageReceived { get; set; }
        public float healingDone { get; set; }
        public float healingReceived { get; set; }
    }
}
