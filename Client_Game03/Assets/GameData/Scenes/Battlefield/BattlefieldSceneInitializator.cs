using Assets.GameData.Scenes.Battlefield.Animations;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
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
        /// <summary>
        /// Делаем static так как объект передаётся между сценами
        /// </summary>
        public static SpawnedBattlefield spawnedBattlefield { get; set; } = null;

        private readonly Dictionary<Guid, BattlefieldUnit> battlefieldUnits = new();
        private readonly List<BattlefieldUnit> playerUnits = new();
        private readonly List<BattlefieldUnit> enemyUnits = new();
        private bool initialized = false;
        public static float width { get; private set; } = 0f;
        public static float height { get; private set; } = 0f;

        public static float animationSpeed { get; private set; } = 1f;

        private const string ANIMATION_SPEED_PREFS_KEY = "Battlefield.AnimationSpeed";

        private static readonly float animationSpeedButton_Size = 128;
        private static readonly float button_Padding = 25;
        //private static readonly float _AbilityButton_FontSize = 24;
        private static readonly float animationSpeedButton_FontSize = 50;
        private RectTransform animationSpeedButton__RectTransform;
        //private RectTransform _Ability1Button__RectTransform;
        //private RectTransform _Ability2Button__RectTransform;
        //private RectTransform _Ability3Button__RectTransform;
        private TextMeshProUGUI animationSpeedButton__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability1Button__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability2Button__TextMeshProUGUI;
        //private TextMeshProUGUI _Ability3Button__TextMeshProUGUI;

        private readonly HealthHub healthHub = new();

        public static Transform canvasDamage__Transform { get; private set; }

        private RectTransform turn__RectTransform;
        private TextMeshProUGUI turn__TextMeshProUGUI;

        public int battlefieldIndexAnimationStarted { get; private set; } = -1;
        private bool battlefieldIndexAnimationActive = false;

        private PanelDamage__script panelDamage__script;

        private readonly DateTime dateTimeWaitFor = DateTime.MinValue;
        public StatisticsBattle statisticsBattle { get; private set; }

        private void Start()
        {
            panelDamage__script = new();
            statisticsBattle = new(this);
            if (!TryInitialize())
            {
                return;
            }
            panelDamage__script.Initialize();
            this.RunAsync(StartAsync);
        }

        private bool TryInitialize()
        {
            if (spawnedBattlefield == null || spawnedBattlefield.spawnedHeroPlayerList == null)
            {
                GameMessage.Show("spawnedBattlefield == null || spawnedBattlefield.SpawnedHeroes == null", true);
                return false;
            }

            //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(SpawnedBattlefield));
            battlefieldUnits.Clear();

            Transform canvasUnits__Transform = GameObjectFinder.FindByName("CanvasUnits").transform;
            canvasDamage__Transform = GameObjectFinder.FindByName("CanvasDamage").transform;

            // размещение героев игрока
            for (int i = 0; i < spawnedBattlefield.spawnedHeroPlayerList.Count; i++)
            {
                SpawnedHero spawnedHeroes = spawnedBattlefield.spawnedHeroPlayerList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, true, canvasUnits__Transform, healthHub, statisticsBattle);
                battlefieldUnits.Add(spawnedHeroes.spawnedId, unit);
                playerUnits.Add(unit);

                BaseHero baseHero = Game03Client.GameData.Container.baseHeroes.First(a => a.id == spawnedHeroes.baseHeroId);
                statisticsBattle.AddHero(spawnedHeroes.spawnedId, true, baseHero.name);
            }

            // размещение героев врага
            for (int i = 0; i < spawnedBattlefield.spawnedHeroEnemyList.Count; i++)
            {
                SpawnedHero spawnedHeroes = spawnedBattlefield.spawnedHeroEnemyList[i];
                BattlefieldUnit unit = new(spawnedHeroes, i, false, canvasUnits__Transform, healthHub, statisticsBattle);
                battlefieldUnits.Add(spawnedHeroes.spawnedId, unit);
                enemyUnits.Add(unit);

                BaseHero baseHero = Game03Client.GameData.Container.baseHeroes.First(a => a.id == spawnedHeroes.baseHeroId);
                statisticsBattle.AddHero(spawnedHeroes.spawnedId, false, baseHero.name);
            }

            animationSpeedButton__RectTransform = GameObjectFinder.FindByName<RectTransform>("AnimationSpeedButton");
            animationSpeedButton__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", animationSpeedButton__RectTransform);

            animationSpeed = LoadAnimationSpeed();
            animationSpeedButton__TextMeshProUGUI.text = $"X{animationSpeed:0}";

            Button AnimationSpeedButton__Button = animationSpeedButton__RectTransform.gameObject.GetComponent<Button>();
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

            turn__RectTransform = GameObjectFinder.FindByName<RectTransform>("TurnText");
            turn__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("TurnText");

            return true;
        }

        private void AnimationSpeedChange()
        {
            if (animationSpeedButton__RectTransform != null)
            {
                if (animationSpeed == 1f)
                {
                    animationSpeed = 2f;
                    animationSpeedButton__TextMeshProUGUI.text = "X2";
                }
                else if (animationSpeed == 2f)
                {
                    animationSpeed = 5f;
                    animationSpeedButton__TextMeshProUGUI.text = "X5";
                }
                else if (animationSpeed == 5f)
                {
                    animationSpeed = 10f;
                    animationSpeedButton__TextMeshProUGUI.text = "X10";
                }
                else
                {
                    animationSpeed = 1f;
                    animationSpeedButton__TextMeshProUGUI.text = "X1";
                }

                PlayerPrefs.SetFloat(ANIMATION_SPEED_PREFS_KEY, animationSpeed);
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
            spawnedBattlefield.battlefieldLog = await Game03Client.Battlefield.BattlefieldProvider.GetBattleLogAsync(cancellationToken);
            if (spawnedBattlefield.battlefieldLog == null)
            {
                return;
            }
            spawnedBattlefield.battlefieldLog.Sort((a, b) => a.index.CompareTo(b.index));
            battlefieldIndexAnimationStarted = 0;
            battlefieldIndexAnimationActive = false;
            panelDamage__script.battlefieldSceneInitializator = this;

            for (int i = 0; i < playerUnits.Count; i++)
            {
                BaseHero h = Game03Client.GameData.GetBaseHeroById(playerUnits[i].SpawnedHero.baseHeroId);
                panelDamage__script.AddProgressBar();
            }
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                BaseHero h = Game03Client.GameData.GetBaseHeroById(enemyUnits[i].SpawnedHero.baseHeroId);
                panelDamage__script.AddProgressBar();
            }
            initialized = true;
        }

        private void Update()
        {
            if (initialized && (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width)))
            {
                OnResized();
            }

            List<BattlefieldLogRecordBase> fullLog = spawnedBattlefield.battlefieldLog;
            if (battlefieldIndexAnimationStarted > fullLog.Count)
            {
                return;
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
                if (!battlefieldIndexAnimationActive && fullLog != null)
                {
                    for (int i = 0; i < fullLog.Count; i++)
                    {
                        BattlefieldLogRecordBase iLog = fullLog[i];
                        if (iLog.index > battlefieldIndexAnimationStarted)
                        {
                            switch (iLog)
                            {
                                case BattlefieldLogRecord_TurnStart log:
                                    turn__TextMeshProUGUI.text = $"{LM.GetValue(L.UI.Label.Turn)}: {log.turn}";
                                    break;
                                //case BattlefieldLogRecord_ChangeActionPoints log:
                                //    break;
                                case BattlefieldLogRecord_UseAbility log:
                                    switch (log.ability)
                                    {
                                        case EBattlefieldLogAbility.attack:
                                            if (log.spawnedHeroTargets.Length == 1)
                                            {
                                                BattlefieldUnit h1Unit = battlefieldUnits[log.spawnedHero1Id];
                                                BattlefieldUnit h2Unit = battlefieldUnits[log.spawnedHeroTargets[0]];

                                                // ищем в логе запись которая хранит значения изменения здоровья
                                                BattlefieldLogRecordBase logRecord = fullLog.FirstOrDefault(a => a is BattlefieldLogRecord_Damage d && d.indexReason == log.index);
                                                if (logRecord is not null and BattlefieldLogRecord_Damage logDamage)
                                                {
                                                    h1Unit.AnimationStartAttackUnit(h2Unit, logDamage.damage, logDamage.isCrit);
                                                    h2Unit.SpawnedHero.health -= logDamage.damage;
                                                    //dateTimeWaitFor = DateTime.Now.AddSeconds(
                                                    //    0
                                                    //    + BattlefieldUnit.AnimationAttackTimeStage1
                                                    //    + BattlefieldUnit.AnimationAttackTimeStage2
                                                    //    //+ BattlefieldUnit.AnimationAttackTimeStage3
                                                    //    );
                                                    //void UpdatePanelDamage()
                                                    //{
                                                    //    PanelDamage__script.Bar bar = panelDamage__script.bars.FirstOrDefault(a => a.heroId == h1Unit.SpawnedHero.SpawnedId);
                                                    //    if (bar == null)
                                                    //    {
                                                    //        Debug.Log($"bar is null, SpawnedHero.SpawnedId={h1Unit.SpawnedHero.SpawnedId}");
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        bar.bar.value += logDamage.Damage;
                                                    //        bar.bar.SetTextLeft(bar.bar.value.ToStr());
                                                    //        panelDamage__script.ProgressBarsSortAndRefresh();
                                                    //    }
                                                    //}


                                                    battlefieldIndexAnimationActive = true;
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

                            battlefieldIndexAnimationStarted++;
                            break;
                        }
                    }


                }
            }

            panelDamage__script.Refresh();

            battlefieldIndexAnimationActive = false;
            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationDeathScale();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.UpdateAnimationAttack();
                unit.UpdateAnimationDeathScale();
                if (!battlefieldIndexAnimationActive && unit.AnimationAttackStage > 0)
                {
                    battlefieldIndexAnimationActive = true;
                }
            }

            healthHub.Update();


        }

        private void OnResized()
        {
            if (!initialized)
            {
                return;
            }

            height = Screen.height;
            width = Screen.width;

            foreach (BattlefieldUnit unit in playerUnits)
            {
                unit.OnResize();
            }
            foreach (BattlefieldUnit unit in enemyUnits)
            {
                unit.OnResize();
            }

            float coefHeight = G.GetCoefHeight();
            float animationSpeedButton_Size = BattlefieldSceneInitializator.animationSpeedButton_Size * coefHeight;
            Vector2 animationSpeedButton_SizeVector = new(animationSpeedButton_Size, animationSpeedButton_Size);

            animationSpeedButton__RectTransform.sizeDelta = animationSpeedButton_SizeVector;
            //_Ability1Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            //_Ability2Button__RectTransform.sizeDelta = abilityButton_SizeVector;
            //_Ability3Button__RectTransform.sizeDelta = abilityButton_SizeVector;

            float abilityButton_Padding = button_Padding * coefHeight;
            animationSpeedButton__RectTransform.anchoredPosition = new Vector2(-abilityButton_Padding, abilityButton_Padding);
            //_Ability1Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 2) - abilityButton_Size, abilityButton_Padding);
            //_Ability2Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 3) - (abilityButton_Size * 2), abilityButton_Padding);
            //_Ability3Button__RectTransform.anchoredPosition = new Vector2((-abilityButton_Padding * 4) - (abilityButton_Size * 3), abilityButton_Padding);

            animationSpeedButton__TextMeshProUGUI.fontSize = animationSpeedButton_FontSize * coefHeight;
            //_Ability1Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            //_Ability2Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;
            //_Ability3Button__TextMeshProUGUI.fontSize = _AbilityButton_FontSize * coefHeight;

            turn__RectTransform.anchoredPosition = new Vector2(-25 * coefHeight, -108 * coefHeight);
            turn__TextMeshProUGUI.fontSize = 70 * coefHeight;
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
