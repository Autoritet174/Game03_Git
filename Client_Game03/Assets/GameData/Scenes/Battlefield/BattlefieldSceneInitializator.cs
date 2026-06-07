using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scenes.Battlefield.Animations;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Battlefield;
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

        private static readonly float _AbilityButton_Size = 150;
        private static readonly float _AbilityButton_Padding = 25;
        private static readonly float _AbilityButton_FontSize = 24;
        private RectTransform _AttackButton__RectTransform;
        private RectTransform _Ability1Button__RectTransform;
        private RectTransform _Ability2Button__RectTransform;
        private RectTransform _Ability3Button__RectTransform;
        private TextMeshProUGUI _AttackButton__TextMeshProUGUI;
        private TextMeshProUGUI _Ability1Button__TextMeshProUGUI;
        private TextMeshProUGUI _Ability2Button__TextMeshProUGUI;
        private TextMeshProUGUI _Ability3Button__TextMeshProUGUI;

        public static Transform CanvasDamage__Transform { get; private set; }

        private RectTransform _Turn__RectTransform;
        private TextMeshProUGUI _Turn__TextMeshProUGUI;

        private int battlefieldIndexAnimationStarted = -1;
        private bool battlefieldIndexAnimationActive = false;

        private DateTime dateTimeWaitFor = DateTime.MinValue;

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
            CanvasDamage__Transform = GameObjectFinder.FindByName("CanvasDamage").transform;

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


            _Turn__RectTransform = GameObjectFinder.FindByName<RectTransform>("TurnText");
            _Turn__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("TurnText");

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


            if (dateTimeWaitFor < DateTime.Now)
            {
                List<BattlefieldLogRecordBase> fullLog = SpawnedBattlefield.BattlefieldLog;
                if (!battlefieldIndexAnimationActive && fullLog != null)
                {
                    for (int i = 0; i < fullLog.Count; i++)
                    {
                        BattlefieldLogRecordBase iLog = fullLog[i];
                        if (iLog.Index > battlefieldIndexAnimationStarted)
                        {
                            switch (iLog)
                            {
                                case BattlefieldLogRecord_TurnStart log:
                                    _Turn__TextMeshProUGUI.text = $"{LM.GetValue(L.UI.Label.Turn)}: {log.Turn}";

                                    break;
                                //case BattlefieldLogRecord_ChangeActionPoints log:
                                //    break;
                                case BattlefieldLogRecord_UseAbility log:

                                    switch (log.Ability)
                                    {
                                        case EBattlefieldLogAbility.Attack:
                                            if (log.SpawnedHeroTargets.Length == 1)
                                            {
                                                BattlefieldUnit h1Unit = battlefieldUnits[log.SpawnedHero1Id];
                                                BattlefieldUnit h2Unit = battlefieldUnits[log.SpawnedHeroTargets[0]];

                                                // ищем в логе запись которая хранит значения изменения здоровья
                                                BattlefieldLogRecordBase logRecord = fullLog.FirstOrDefault(a => a is BattlefieldLogRecord_Damage d && d.IndexReason == log.Index);
                                                if (logRecord is not null and BattlefieldLogRecord_Damage logDamage)
                                                {
                                                    h1Unit.AnimationStartAttackUnit(h2Unit, logDamage.Damage, logDamage.IsCrit);
                                                    h2Unit.SpawnedHero.Health -= logDamage.Damage;

                                                    battlefieldIndexAnimationActive = true;
                                                }


                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                    dateTimeWaitFor = DateTime.Now.AddSeconds(0 + BattlefieldUnit.AnimationAttackTimeStage1 + BattlefieldUnit.AnimationAttackTimeStage2 + BattlefieldUnit.AnimationAttackTimeStage3);
                                    Debug.Log(dateTimeWaitFor);

                                    break;
                                    //case BattlefieldLogRecord_Damage log:
                                    //    break;

                                    //default:
                                    //    break;
                            }
                           
                            battlefieldIndexAnimationStarted++;
                            break;
                        }
                    }
                }
            }

            battlefieldIndexAnimationActive = false;
            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.UpdateAnimationAttack();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.UpdateAnimationAttack();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
            }

            HealthHub.Update();


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

            _Turn__RectTransform.anchoredPosition = new Vector2(-25 * coefHeight, -108 * coefHeight);
            _Turn__TextMeshProUGUI.fontSize = 70 * coefHeight;
        }

        private async UniTask AbilityAttackOnClickAsync()
        {
            SpawnedBattlefield.BattlefieldLog = await BattlefieldProvider.GetBattleLogAsync(
                CancellationTokenManager.Create($"{nameof(BattlefieldProvider)}.{nameof(BattlefieldProvider.GetBattleLogAsync)}()"));

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
