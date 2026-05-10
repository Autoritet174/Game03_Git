using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.BattleField
{
    public class BattlefieldSceneInitializator : MonoBehaviour
    {
        public static SpawnedBattlefield SpawnedBattlefield { get; set; } = null;

        private readonly Dictionary<Guid, BattlefieldUnit> battlefieldUnits = new();

        public void Start()
        {
            if (SpawnedBattlefield == null || SpawnedBattlefield.SpawnedHeroPlayerList == null)
            {
                GameMessage.Show("spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null", true);
                return;
            }

            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(SpawnedBattlefield));
            battlefieldUnits.Clear();

            // размещение героев игрока
            for (int i = 0; i < SpawnedBattlefield.SpawnedHeroPlayerList.Count; i++)
            {
                SpawnedHero spawnedHeroes = SpawnedBattlefield.SpawnedHeroPlayerList[i];
                battlefieldUnits.Add(spawnedHeroes.SpawnedId, new(spawnedHeroes, i, true));
            }

            
        }
    }
}
