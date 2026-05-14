using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using General;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.BattleField
{
    public class BattlefieldSceneInitializator : MonoBehaviour
    {
        public static SpawnedBattlefield SpawnedBattlefield { get; set; } = null;

        private readonly Dictionary<Guid, BattlefieldUnit> battlefieldUnits = new();
        private readonly List<BattlefieldUnit> playerUnits = new();
        private readonly List<BattlefieldUnit> enemyUnits = new();
        private bool _Initialized = false;
        public static float Width { get; private set; } = 0f;
        public static float Height { get; private set; } = 0f;
        private void Start()
        {
            if (SpawnedBattlefield == null || SpawnedBattlefield.SpawnedHeroPlayerList == null)
            {
                GameMessage.Show("spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null", true);
                return;
            }

            //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(SpawnedBattlefield));
            battlefieldUnits.Clear();

            Transform canvasUnits__Transform = GameObjectFinder.FindByName("CanvasUnits").transform;

            // размещение героев игрока
            for (int i = 0; i < SpawnedBattlefield.SpawnedHeroPlayerList.Count; i++)
            {
                SpawnedHero spawnedHeroes = SpawnedBattlefield.SpawnedHeroPlayerList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, true, canvasUnits__Transform);
                battlefieldUnits.Add(spawnedHeroes.SpawnedId, unit);
                playerUnits.Add(unit);
            }

            // размещение героев врага
            for (int i = 0; i < SpawnedBattlefield.SpawnedHeroEnemyList.Count; i++)
            {
                SpawnedHero spawnedHeroes = SpawnedBattlefield.SpawnedHeroEnemyList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, false, canvasUnits__Transform);
                battlefieldUnits.Add(spawnedHeroes.SpawnedId, unit);
                enemyUnits.Add(unit);
            }

            Button testButton = GameObjectFinder.FindByName<Button>("TestButton");
            testButton.onClick.RemoveAllListeners();
            testButton.onClick.AddListener(() =>
            {
                BattlefieldUnit myUnit = playerUnits[RandomShared.Next(playerUnits.Count)];
                BattlefieldUnit enemyUnit = enemyUnits[RandomShared.Next(enemyUnits.Count)];
                myUnit.AnimationStartAttackUnit(enemyUnit);
            });

            _Initialized = true;
        }

        private void Update()
        {
            if (_Initialized && (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width)))
            {
                OnResized();
            }
            if (!playerUnits.Any(a => a.AnimationAttackStage > 0))
            {
                var list = enemyUnits.Where(a => a.SpawnedHero.Health > 0).ToList();
                if (list.Count > 0)
                {
                    BattlefieldUnit myUnit = playerUnits[RandomShared.Next(playerUnits.Count)];
                    BattlefieldUnit enemyUnit = list[RandomShared.Next(list.Count)];
                    myUnit.AnimationStartAttackUnit(enemyUnit);
                }
            }

            //foreach (BattlefieldUnit unit in playerUnits)
            //{
            //    if (unit.AnimationAttackStage == 0)
            //    {
            //        BattlefieldUnit enemyUnit = enemyUnits[RandomShared.Next(enemyUnits.Count)];
            //        unit.AnimationStartAttackUnit(enemyUnit);
            //    }
            //}

            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationChangeHealth();
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationChangeHealth();
            }
        }

        private void OnResized()
        {
            if (!_Initialized)
            {
                return;
            }

            Height = Screen.height;
            Width = Screen.width;

            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.OnResize();
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.OnResize();
            }
        }
    }
}
