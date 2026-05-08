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
        public static SpawnedBattlefield spawnedBattlefield { get; set; } = null;

        private readonly Dictionary<Guid, BattlefieldUnit> battlefieldUnits = new();

        public void Start()
        {
            if (spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null)
            {
                GameMessage.Show("spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null", true);
                return;
            }

            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(spawnedBattlefield));
            battlefieldUnits.Clear();

            // размещение героев игрока
            for (int i = 0; i < spawnedBattlefield.SpawnedHeroes.Count; i++)
            {
                SpawnedHero spawnedHeroes = spawnedBattlefield.SpawnedHeroes[i];
                battlefieldUnits.Add(spawnedHeroes.SpawnedId, new(spawnedHeroes, i, true));
            }

            
        }
    }
}
