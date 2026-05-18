using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

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

        private readonly float _AbilityButton_Size = 150;
        private readonly float _AbilityButton_Padding = 25;
        private readonly float _AbilityButton_FontSize = 24;
        private RectTransform _AttackButton__RectTransform;
        private RectTransform _Ability1Button__RectTransform;
        private RectTransform _Ability2Button__RectTransform;
        private RectTransform _Ability3Button__RectTransform;
        private TextMeshProUGUI _AttackButton__TextMeshProUGUI;
        private TextMeshProUGUI _Ability1Button__TextMeshProUGUI;
        private TextMeshProUGUI _Ability2Button__TextMeshProUGUI;
        private TextMeshProUGUI _Ability3Button__TextMeshProUGUI;

        private int battlefieldIndexAnimationStarted = -1;
        private bool battlefieldIndexAnimationActive = false;

        private async void Start()
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

            _AttackButton__RectTransform = GameObjectFinder.FindByName<RectTransform>("AttackButton");
            _AttackButton__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _AttackButton__RectTransform.transform);
            _AttackButton__TextMeshProUGUI.text = LM.GetValue(L.UI.Button.Ability.Attack);
            EventHelper.SetClickEvent(_AttackButton__RectTransform.gameObject, AbilityAttackOnClickAsync, true);

            _Ability1Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability1Button");
            _Ability1Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability1Button__RectTransform.transform);
            EventHelper.SetClickEvent(_Ability1Button__RectTransform.gameObject, Ability1OnClickAsync, true);

            _Ability2Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability2Button");
            _Ability2Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability2Button__RectTransform.transform);
            EventHelper.SetClickEvent(_Ability2Button__RectTransform.gameObject, Ability2OnClickAsync, true);

            _Ability3Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability3Button");
            _Ability3Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability3Button__RectTransform.transform);
            EventHelper.SetClickEvent(_Ability3Button__RectTransform.gameObject, Ability3OnClickAsync, true);


            //testButton.onClick.RemoveAllListeners();
            //testButton.onClick.AddListener(() =>
            //{
            //    BattlefieldUnit myUnit = playerUnits[RandomShared.Next(playerUnits.Count)];
            //    BattlefieldUnit enemyUnit = enemyUnits[RandomShared.Next(enemyUnits.Count)];
            //    myUnit.AnimationStartAttackUnit(enemyUnit);
            //});

            SpawnedBattlefield.BattlefieldLog = await Game03Client.Battlefield.BattlefieldProvider.GetBattleLogAsync(default);
            if (SpawnedBattlefield.BattlefieldLog == null)
            {
                return;
            }
            SpawnedBattlefield.BattlefieldLog.Sort((a, b) => a.Index.CompareTo(b.Index));
            battlefieldIndexAnimationStarted = 0;
            battlefieldIndexAnimationActive = false;

            _Initialized = true;
        }

        private void Update()
        {
            if (_Initialized && (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width)))
            {
                OnResized();
            }
            //if (!playerUnits.Any(a => a.AnimationAttackStage > 0))
            //{
            //    var list = enemyUnits.Where(a => a.SpawnedHero.Health > 0).ToList();
            //    if (list.Count > 0)
            //    {
            //        BattlefieldUnit myUnit = playerUnits[RandomShared.Next(playerUnits.Count)];
            //        BattlefieldUnit enemyUnit = list[RandomShared.Next(list.Count)];
            //        myUnit.AnimationStartAttackUnit(enemyUnit);
            //    }
            //}

            //foreach (BattlefieldUnit unit in playerUnits)
            //{
            //    if (unit.AnimationAttackStage == 0)
            //    {
            //        BattlefieldUnit enemyUnit = enemyUnits[RandomShared.Next(enemyUnits.Count)];
            //        unit.AnimationStartAttackUnit(enemyUnit);
            //    }
            //}


            //if (battlefieldIndexAnimationActive && SpawnedBattlefield.BattlefieldLog[^1].Index <= battlefieldIndexAnimationStarted)
            //{
            //    battlefieldIndexAnimationStarted = -1;
            //    battlefieldIndexAnimationActive = false;
            //}

            if (!battlefieldIndexAnimationActive && SpawnedBattlefield.BattlefieldLog != null) {
                for (int i = 0; i < SpawnedBattlefield.BattlefieldLog.Count; i++)
                {
                    BattlefieldLogRecord b = SpawnedBattlefield.BattlefieldLog[i];
                    if (b.Index > battlefieldIndexAnimationStarted)
                    {
                        if (b.eAbility == EAbility.Attack)
                        {
                            BattlefieldUnit h1Unit = battlefieldUnits[b.H1.Value];
                            BattlefieldUnit h2Unit = battlefieldUnits[b.H2.Value];
                            h1Unit.AnimationStartAttackUnit(h2Unit, b.Damage.Value, b.IsCrit);
                            h2Unit.SpawnedHero.Health -= b.Damage.Value;
                            battlefieldIndexAnimationStarted++;
                            battlefieldIndexAnimationActive = true;

                            //Debug.Log("индекс анимации = " + b.Index.ToString());
                        }
                        break;
                    }
                }
            }


            battlefieldIndexAnimationActive = false;
            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationChangeHealth();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationChangeHealth();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
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

            float coefHeight = G.GetCoefHeight();
            float abilityButton_Size = _AbilityButton_Size * coefHeight;
            Vector2 abilityButton_SizeVector = new(abilityButton_Size, abilityButton_Size);

            _AttackButton__RectTransform.sizeDelta = abilityButton_SizeVector;
            _Ability1Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            _Ability2Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            _Ability3Button__RectTransform.sizeDelta = abilityButton_SizeVector;

            float abilityButton_Padding = _AbilityButton_Padding * coefHeight;
            _AttackButton__RectTransform.anchoredPosition = new Vector2(-abilityButton_Padding, abilityButton_Padding);
            _Ability1Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 2) - abilityButton_Size, abilityButton_Padding);
            _Ability2Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 3) - (abilityButton_Size * 2), abilityButton_Padding);
            _Ability3Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 4) - (abilityButton_Size * 3), abilityButton_Padding);

            _AttackButton__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            _Ability1Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            _Ability2Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            _Ability3Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
        }

        private async UniTask AbilityAttackOnClickAsync()
        {
            SpawnedBattlefield.BattlefieldLog = await Game03Client.Battlefield.BattlefieldProvider.GetBattleLogAsync(default);
            if (SpawnedBattlefield.BattlefieldLog == null)
            {
                return;
            }
            SpawnedBattlefield.BattlefieldLog.Sort((a, b) => a.Index.CompareTo(b.Index));
            battlefieldIndexAnimationStarted = 0;
            battlefieldIndexAnimationActive = false;
            //Debug.Log("количество записей в логе = "+SpawnedBattlefield.BattlefieldLog.Count.ToString());
            //string s = JSON.Serialize(SpawnedBattlefield.BattlefieldLog);
        }
        private async UniTask Ability1OnClickAsync()
        {

        }
        private async UniTask Ability2OnClickAsync()
        {

        }
        private async UniTask Ability3OnClickAsync()
        {

        }

    }
}
