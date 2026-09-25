using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    /// <summary>Показывает параметры сражения, выбор героев и запускает бой.</summary>
    public class PanelPrepareBattle : IPrefab
    {
        public bool initialized { get; private set; }

        public float width { get; private set; }

        public float height { get; private set; }

        public PanelTop__prefab__scriptMB panelTop__prefab__context { get; private set; }

        private GameObject gameObject;
        private RectTransform rectTransform;

        private RectTransform panelBattlefield__RectTransform;

        private GameObject startBattleButton__GameObject;
        private RectTransform startBattleButton__RectTransform;

        private RectTransform heroesSelectedAndMaxLabel__RectTransform;
        private TextMeshProUGUI heroesSelectedAndMaxLabel__TextMeshProUGUI;

        private EBattleFiled battlefieldId;
        private bool battleStarting;

        private General.DTO.Entities.GameData.Battlefield battlefield = null;
        public PanelCollection__prefab__scriptMB panelCollection__prefab { get; set; }

        public Action sceneOnResized { get; set; }

        public SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator { get; set; }

        public void Initialize()
        {
            gameObject = GameObjectFinder.FindByName("PanelPrepareBattle");
            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetHorizontalOffsets(0, 0);//переместить в пределы экрана

            panelTop__prefab__context = GameObjectFinder.FindByName("PanelPrepareBattle_PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
            panelTop__prefab__context.Initialize();
            panelTop__prefab__context.SetActionOnButtonClose(Hide);

            panelCollection__prefab = GameObjectFinder.FindByName<PanelCollection__prefab__scriptMB>("PanelCollection", startParent: rectTransform);

            PanelCollectionContext panelCollectionContext = new();
            panelCollectionContext.OnCollectionLoaded(selectBattlefieldSceneInitializator, UpdateHeroesSelectedAndMaxLabel);
            panelCollection__prefab.panelCollectionContext = panelCollectionContext;
            panelCollection__prefab.Initialize();
            panelCollection__prefab.InstantiateCollection(panelCollection__prefab.collectionMode);

            //GameObjectFinder.FindByName("ImageButtonEquipments (id=vuhjngaz)", PanelCollection__prefab.gameObject).SetActive(false);

            startBattleButton__GameObject = GameObjectFinder.FindByName("StartBattleButton", gameObject);
            startBattleButton__RectTransform = startBattleButton__GameObject.GetComponent<RectTransform>();
            GameObjectFinder.FindByName<TextMeshProUGUI>("Text", startBattleButton__GameObject).SetText(LM.GetValue(L.UI.Button.StartBattle));
            startBattleButton__GameObject.GetComponent<Button>().onClick.AddListener(() => StartBattleAsync().Forget());

            // Панель подготовки к бою
            {
                panelBattlefield__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBattlefield", gameObject);

                // Лейбл "Выбрано X/Y героев"
                heroesSelectedAndMaxLabel__RectTransform = GameObjectFinder.FindByName<RectTransform>("HeroesSelectedAndMaxLabel", panelBattlefield__RectTransform);
                heroesSelectedAndMaxLabel__TextMeshProUGUI = heroesSelectedAndMaxLabel__RectTransform.GetComponent<TextMeshProUGUI>();

            }

            gameObject.SetActive(false);

            {
                //GameObject buttonClose__GameObject = GameObjectFinder.FindByName("PanelTop", _GameObject)
                //   .GetComponent<PanelTop__prefab__scriptMB>()
                //   ._ButtonClose__RectTransform.gameObject;

                //if (buttonClose__GameObject.TryGetComponent(out ButtonClose_Click_MoveToMainMenu clickClose))
                //{
                //    UnityEngine.Object.Destroy(clickClose);
                //}

                //buttonClose__GameObject.GetComponent<Button>().onClick.RemoveAllListeners();

                //GameObjectFinder.FindByName("PanelTop", _GameObject)
                //    .GetComponent<PanelTop__prefab__scriptMB>()
                //    ._ButtonClose__RectTransform.gameObject
                //    .SetClickEvent(Cancel, useButtonComponent: true);
            }
        }

        public bool isVisible => gameObject.activeSelf;

        public void Show(EBattleFiled battlefieldId)
        {
            this.battlefieldId = battlefieldId;
            battleStarting = false;
            panelCollection__prefab.UnselectAll();
            panelCollection__prefab.PanelTopButtons_ResetPageCurrent();
            gameObject.SetActive(true);

            battlefield = Game03Client.GameData.Container.battlefields.First(a => a.id == battlefieldId);

            //_PanelCollectionContext.Actions.Clear();
            //_PanelCollectionContext.Actions.Add(UpdateHeroesSelectedAndMaxLabel);
            UpdateHeroesSelectedAndMaxLabel();

            panelCollection__prefab.InstantiateCollection(ECollectionMode.hero);
            sceneOnResized();
        }

        public void Hide()
        {
            battleStarting = false;
            panelCollection__prefab.UnselectAll();
            gameObject.SetActive(false);
            sceneOnResized();
        }

        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            panelTop__prefab__context.OnResized(coefHeight);
            float heightValue = Screen.height - panelTop__prefab__context.height;
            float widthValue = Screen.width * 0.3333f;
            panelBattlefield__RectTransform.sizeDelta = new(widthValue, heightValue);
            heroesSelectedAndMaxLabel__TextMeshProUGUI.fontSize = 70f * coefHeight;

            float offset = 20f * coefHeight;
            heroesSelectedAndMaxLabel__RectTransform.SetHorizontalOffsets(offset, offset);
            heroesSelectedAndMaxLabel__RectTransform.anchoredPosition = new(0, -offset);
            heroesSelectedAndMaxLabel__RectTransform.sizeDelta = new(0, 90f * coefHeight);

            startBattleButton__RectTransform.anchoredPosition = new(-25 * coefHeight, 25 * coefHeight);
            startBattleButton__RectTransform.sizeDelta = new(325 * coefHeight, 100 * coefHeight);

            panelCollection__prefab.OnResized(coefHeight, top: panelTop__prefab__context.height, right: widthValue);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void UpdateHeroesSelectedAndMaxLabel()
        {
            int selectedCount = panelCollection__prefab.GetSelectedElements().Count;
            int max = battlefield.maxHeroCount;
            heroesSelectedAndMaxLabel__TextMeshProUGUI.SetText($"{LM.GetValue(L.UI.Button.Heroes)} {selectedCount}/{max}");
        }

        private async UniTask StartBattleAsync()
        {
            if (battleStarting)
            {
                return;
            }

            Guid[] heroIds = panelCollection__prefab.GetSelectedElements().ToArray();
            if (heroIds.Length == 0)
            {
                GameMessage.Show(LM.GetValue(L.Info.SelectHero), true);
                return;
            }

            battleStarting = true;
            try
            {
                BattlefieldSceneInitializator.spawnedBattlefield = await Game03Client.Battlefield.BattlefieldProvider.LoadBattlefieldAsync(
                    battlefieldId,
                    heroIds,
                    CancellationTokenManager.Create($"{nameof(PanelPrepareBattle)}.{nameof(StartBattleAsync)}"));

                if (BattlefieldSceneInitializator.spawnedBattlefield != null)
                {
                    BattlefieldSceneInitializator.spawnedBattlefield.spawnedHeroPlayerList.Sort((a, b) => b.initiative.CompareTo(a.initiative));
                    BattlefieldSceneInitializator.spawnedBattlefield.spawnedHeroEnemyList.Sort((a, b) => b.initiative.CompareTo(a.initiative));
                    Hide();
                    GameSceneManager.Load(GameSceneManager.ESceneName.battlefield);
                }
                else
                {
                    Debug.LogError("SpawnedBattlefield is null");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                battleStarting = false;
                GameMessage.Close();
            }
        }

    }
}
