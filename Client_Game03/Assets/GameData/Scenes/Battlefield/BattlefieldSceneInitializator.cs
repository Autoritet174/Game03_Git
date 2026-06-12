using Assets.GameData.Scenes.Battlefield.Animations;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

namespace Assets.GameData.Scenes.Battlefield
{
    public class BattlefieldSceneInitializator : MonoBehaviour
    {
        public static SpawnedBattlefield SpawnedBattlefield { get; set; } = null;

        private readonly Dictionary<Guid, BattlefieldUnit> BattlefieldUnits = new();
        private readonly List<BattlefieldUnit> PlayerUnits = new();
        private readonly List<BattlefieldUnit> EnemyUnits = new();
        private bool Initialized = false;
        public static float Width { get; private set; } = 0f;
        public static float Height { get; private set; } = 0f;

        public static float AnimationSpeed { get; private set; } = 1f;

        private const string ANIMATION_SPEED_PREFS_KEY = "Battlefield.AnimationSpeed";

        private static readonly float AnimationSpeedButton_Size = 128;
        private static readonly float Button_Padding = 25;
        //private static readonly float _AbilityButton_FontSize = 24;
        private static readonly float AnimationSpeedButton_FontSize = 50;
        private RectTransform AnimationSpeedButton__RectTransform;
        //private RectTransform _Ability1Button__RectTransform;
        //private RectTransform _Ability2Button__RectTransform;
        //private RectTransform _Ability3Button__RectTransform;
        private TextMeshProUGUI AnimationSpeedButton__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability1Button__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability2Button__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability3Button__TextMeshProUGUI;

        public static Transform CanvasDamage__Transform { get; private set; }

        private RectTransform Turn__RectTransform;
        private TextMeshProUGUI Turn__TextMeshProUGUI;

        private int BattlefieldIndexAnimationStarted = -1;
        private bool BattlefieldIndexAnimationActive = false;

        private readonly DateTime DateTimeWaitFor = DateTime.MinValue;

        private void Start()
        {
            if (!TryInitialize())
            {
                return;
            }

            this.RunAsync(StartAsync);
        }

        private bool TryInitialize()
        {
            if (SpawnedBattlefield == null || SpawnedBattlefield.SpawnedHeroPlayerList == null)
            {
                GameMessage.Show("spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null", true);
                return false;
            }

            //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(SpawnedBattlefield));
            BattlefieldUnits.Clear();

            Transform canvasUnits__Transform = GameObjectFinder.FindByName("CanvasUnits").transform;
            CanvasDamage__Transform = GameObjectFinder.FindByName("CanvasDamage").transform;

            // размещение героев игрока
            for (int i = 0; i < SpawnedBattlefield.SpawnedHeroPlayerList.Count; i++)
            {
                SpawnedHero spawnedHeroes = SpawnedBattlefield.SpawnedHeroPlayerList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, true, canvasUnits__Transform);
                BattlefieldUnits.Add(spawnedHeroes.SpawnedId, unit);
                PlayerUnits.Add(unit);
            }

            // размещение героев врага
            for (int i = 0; i < SpawnedBattlefield.SpawnedHeroEnemyList.Count; i++)
            {
                SpawnedHero spawnedHeroes = SpawnedBattlefield.SpawnedHeroEnemyList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, false, canvasUnits__Transform);
                BattlefieldUnits.Add(spawnedHeroes.SpawnedId, unit);
                EnemyUnits.Add(unit);
            }

            AnimationSpeedButton__RectTransform = GameObjectFinder.FindByName<RectTransform>("AnimationSpeedButton");
            AnimationSpeedButton__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", AnimationSpeedButton__RectTransform);

            AnimationSpeed = LoadAnimationSpeed();
            AnimationSpeedButton__TextMeshProUGUI.text = $"X{AnimationSpeed:0}";

            Button AnimationSpeedButton__Button = AnimationSpeedButton__RectTransform.gameObject.GetComponent<Button>();
            AnimationSpeedButton__Button.onClick.RemoveAllListeners();
            AnimationSpeedButton__Button.onClick.AddListener(AnimationSpeedChange);

            //_Ability1Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability1Button");
            //_Ability1Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability1Button__RectTransform.transform);
            //EventHelper.SetClickEvent(_Ability1Button__RectTransform.gameObject, Ability1OnClickAsync, true);

            //_Ability2Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability2Button");
            //_Ability2Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability2Button__RectTransform.transform);
            //EventHelper.SetClickEvent(_Ability2Button__RectTransform.gameObject, Ability2OnClickAsync, true);

            //_Ability3Button__RectTransform = GameObjectFinder.FindByName<RectTransform>("Ability3Button");
            //_Ability3Button__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _Ability3Button__RectTransform.transform);
            //EventHelper.SetClickEvent(_Ability3Button__RectTransform.gameObject, Ability3OnClickAsync, true);

            Turn__RectTransform = GameObjectFinder.FindByName<RectTransform>("TurnText");
            Turn__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("TurnText");

            return true;
        }

        private void AnimationSpeedChange()
        {
            if (AnimationSpeedButton__RectTransform != null)
            {
                if (AnimationSpeed == 1f)
                {
                    AnimationSpeed = 2f;
                    AnimationSpeedButton__TextMeshProUGUI.text = "X2";
                }
                else if (AnimationSpeed == 2f)
                {
                    AnimationSpeed = 5f;
                    AnimationSpeedButton__TextMeshProUGUI.text = "X5";
                }
                else if (AnimationSpeed == 5f)
                {
                    AnimationSpeed = 10f;
                    AnimationSpeedButton__TextMeshProUGUI.text = "X10";
                }
                else
                {
                    AnimationSpeed = 1f;
                    AnimationSpeedButton__TextMeshProUGUI.text = "X1";
                }

                PlayerPrefs.SetFloat(ANIMATION_SPEED_PREFS_KEY, AnimationSpeed);
                PlayerPrefs.Save();
            }
        }

        private static float LoadAnimationSpeed()
        {
            float storedValue = PlayerPrefs.GetFloat(ANIMATION_SPEED_PREFS_KEY, 1f);
            return storedValue is 1f or 2f or 5f or 10f ? storedValue : 1f;
        }

        private async UniTask StartAsync(CancellationToken cancellationToken)
        {
            SpawnedBattlefield.BattlefieldLog = await Game03Client.Battlefield.BattlefieldProvider.GetBattleLogAsync(cancellationToken);
            if (SpawnedBattlefield.BattlefieldLog == null)
            {
                return;
            }
            SpawnedBattlefield.BattlefieldLog.Sort((a, b) => a.Index.CompareTo(b.Index));
            BattlefieldIndexAnimationStarted = 0;
            BattlefieldIndexAnimationActive = false;

            Initialized = true;
        }

        private void Update()
        {
            if (Initialized && (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width)))
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


            if (DateTimeWaitFor < DateTime.Now)
            {
                List<BattlefieldLogRecordBase> fullLog = SpawnedBattlefield.BattlefieldLog;
                if (!BattlefieldIndexAnimationActive && fullLog != null)
                {
                    for (int i = 0; i < fullLog.Count; i++)
                    {
                        BattlefieldLogRecordBase iLog = fullLog[i];
                        if (iLog.Index > BattlefieldIndexAnimationStarted)
                        {
                            switch (iLog)
                            {
                                case BattlefieldLogRecord_TurnStart log:
                                    Turn__TextMeshProUGUI.text = $"{LM.GetValue(L.UI.Label.Turn)}: {log.Turn}";

                                    break;
                                //case BattlefieldLogRecord_ChangeActionPoints log:
                                //    break;
                                case BattlefieldLogRecord_UseAbility log:

                                    switch (log.Ability)
                                    {
                                        case EBattlefieldLogAbility.Attack:
                                            if (log.SpawnedHeroTargets.Length == 1)
                                            {
                                                BattlefieldUnit h1Unit = BattlefieldUnits[log.SpawnedHero1Id];
                                                BattlefieldUnit h2Unit = BattlefieldUnits[log.SpawnedHeroTargets[0]];

                                                // ищем в логе запись которая хранит значения изменения здоровья
                                                BattlefieldLogRecordBase logRecord = fullLog.FirstOrDefault(a => a is BattlefieldLogRecord_Damage d && d.IndexReason == log.Index);
                                                if (logRecord is not null and BattlefieldLogRecord_Damage logDamage)
                                                {
                                                    h1Unit.AnimationStartAttackUnit(h2Unit, logDamage.Damage, logDamage.IsCrit);
                                                    h2Unit.SpawnedHero.Health -= logDamage.Damage;
                                                    //dateTimeWaitFor = DateTime.Now.AddSeconds(
                                                    //    0
                                                    //    + BattlefieldUnit.AnimationAttackTimeStage1
                                                    //    + BattlefieldUnit.AnimationAttackTimeStage2
                                                    //    //+ BattlefieldUnit.AnimationAttackTimeStage3
                                                    //    );
                                                    BattlefieldIndexAnimationActive = true;
                                                }


                                            }

                                            break;
                                        default:
                                            break;
                                    }

                                    //Debug.Log(dateTimeWaitFor);

                                    break;
                                    //case BattlefieldLogRecord_Damage log:
                                    //    break;

                                    //default:
                                    //    break;
                            }

                            BattlefieldIndexAnimationStarted++;
                            break;
                        }
                    }
                }
            }

            BattlefieldIndexAnimationActive = false;
            foreach (BattlefieldUnit unit in PlayerUnits)
            {
                unit.UpdateAnimationAttack();
                if (!BattlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    BattlefieldIndexAnimationActive = true;
                }
            }
            foreach (BattlefieldUnit unit in EnemyUnits)
            {
                unit.UpdateAnimationAttack();
                if (!BattlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    BattlefieldIndexAnimationActive = true;
                }
            }

            HealthHub.Update();


        }

        private void OnResized()
        {
            if (!Initialized)
            {
                return;
            }

            Height = Screen.height;
            Width = Screen.width;

            foreach (BattlefieldUnit unit in PlayerUnits)
            {
                unit.OnResize();
            }
            foreach (BattlefieldUnit unit in EnemyUnits)
            {
                unit.OnResize();
            }

            float coefHeight = G.GetCoefHeight();
            float animationSpeedButton_Size = AnimationSpeedButton_Size * coefHeight;
            Vector2 animationSpeedButton_SizeVector = new(animationSpeedButton_Size, animationSpeedButton_Size);

            AnimationSpeedButton__RectTransform.sizeDelta = animationSpeedButton_SizeVector;
            //_Ability1Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            //_Ability2Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            //_Ability3Button__RectTransform.sizeDelta = abilityButton_SizeVector;

            float abilityButton_Padding = Button_Padding * coefHeight;
            AnimationSpeedButton__RectTransform.anchoredPosition = new Vector2(-abilityButton_Padding, abilityButton_Padding);
            //_Ability1Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 2) - abilityButton_Size, abilityButton_Padding);
            //_Ability2Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 3) - (abilityButton_Size * 2), abilityButton_Padding);
            //_Ability3Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 4) - (abilityButton_Size * 3), abilityButton_Padding);

            AnimationSpeedButton__TextMeshProUGUI.fontSize = AnimationSpeedButton_FontSize * coefHeight;
            //_Ability1Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            //_Ability2Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            //_Ability3Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;

            Turn__RectTransform.anchoredPosition = new Vector2(-25 * coefHeight, -108 * coefHeight);
            Turn__TextMeshProUGUI.fontSize = 70 * coefHeight;
        }

        //private async UniTask AbilityAttackOnClickAsync()
        //{
        //    SpawnedBattlefield.BattlefieldLog = await BattlefieldProvider.GetBattleLogAsync(
        //        CancellationTokenManager.Create($"{nameof(BattlefieldProvider)}.{nameof(BattlefieldProvider.GetBattleLogAsync)}()"));

        //    if (SpawnedBattlefield.BattlefieldLog == null)
        //    {
        //        return;
        //    }
        //    SpawnedBattlefield.BattlefieldLog.Sort((a, b) => a.Index.CompareTo(b.Index));
        //    battlefieldIndexAnimationStarted = 0;
        //    battlefieldIndexAnimationActive = false;
        //    //Debug.Log("количество записей в логе = "+SpawnedBattlefield.BattlefieldLog.Count.ToString());
        //    //string s = JSON.Serialize(SpawnedBattlefield.BattlefieldLog);
        //}
        //private async UniTask Ability1OnClickAsync()
        //{

        //}
        //private async UniTask Ability2OnClickAsync()
        //{

        //}
        //private async UniTask Ability3OnClickAsync()
        //{

        //}

    }
}
