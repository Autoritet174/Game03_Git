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
    public class PanelPrepareBattle: IPrefab
    {
        public bool initialized { get; private set; }
        public float width { get; private set; }
        public float height { get; private set; }
        public PanelTop__prefab__scriptMB PanelTop__prefab__context { get; private set; }
        private GameObject _GameObject;
        private RectTransform _RectTransform;

        private  RectTransform _PanelBattlefield__RectTransform;

        private  GameObject _StartBattleButton__GameObject;
        private  RectTransform _StartBattleButton__RectTransform;

        private  RectTransform _HeroesSelectedAndMaxLabel__RectTransform;
        private  TextMeshProUGUI _HeroesSelectedAndMaxLabel__TextMeshProUGUI;

        private EBattleFiled battlefieldId;
        private bool battleStarting;

        private General.DTO.Entities.GameData.Battlefield battlefield = null;
        public PanelCollection__prefab__scriptMB panelCollection__prefab { get; set; }
        public Action SceneOnResized { get; set; }
        public SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator { get; set; }
        public void Initialize()
        {
            _GameObject = GameObjectFinder.FindByName("PanelPrepareBattle");
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _RectTransform.SetHorizontalOffsets(0, 0);//переместить в пределы экрана

            PanelTop__prefab__context = GameObjectFinder.FindByName("PanelPrepareBattle_PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
            PanelTop__prefab__context.Initialize();
            PanelTop__prefab__context.SetActionOnButtonClose(Hide);

            panelCollection__prefab = GameObjectFinder.FindByName<PanelCollection__prefab__scriptMB>("PanelCollection", startParent: _RectTransform);

            PanelCollectionContext panelCollectionContext = new();
            panelCollectionContext.OnCollectionLoaded(selectBattlefieldSceneInitializator, UpdateHeroesSelectedAndMaxLabel);
            panelCollection__prefab.panelCollectionContext = panelCollectionContext;
            panelCollection__prefab.Initialize();
            panelCollection__prefab.InstantiateCollection(panelCollection__prefab.collectionMode);

            //GameObjectFinder.FindByName("ImageButtonEquipments (id=vuhjngaz)", PanelCollection__prefab.gameObject).SetActive(false);



            _StartBattleButton__GameObject = GameObjectFinder.FindByName("StartBattleButton", _GameObject);
            _StartBattleButton__RectTransform = _StartBattleButton__GameObject.GetComponent<RectTransform>();
            GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _StartBattleButton__GameObject).SetText(LM.GetValue(L.UI.Button.StartBattle));
            _StartBattleButton__GameObject.GetComponent<Button>().onClick.AddListener(() => StartBattleAsync().Forget());



            // Панель подготовки к бою
            {
                _PanelBattlefield__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBattlefield", _GameObject);


                // Лейбл "Выбрано X/Y героев"
                _HeroesSelectedAndMaxLabel__RectTransform = GameObjectFinder.FindByName<RectTransform>("HeroesSelectedAndMaxLabel", _PanelBattlefield__RectTransform);
                _HeroesSelectedAndMaxLabel__TextMeshProUGUI = _HeroesSelectedAndMaxLabel__RectTransform.GetComponent<TextMeshProUGUI>();
              
            }

            _GameObject.SetActive(false);

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


        public bool IsVisible => _GameObject.activeSelf;

        public void Show(EBattleFiled battlefieldId)
        {
            this.battlefieldId = battlefieldId;
            battleStarting = false;
            panelCollection__prefab.UnselectAll();
            panelCollection__prefab.PanelTopButtons_ResetPageCurrent();
            _GameObject.SetActive(true);
            

            battlefield = Game03Client.GameData.Container.battlefields.First(a => a.id == battlefieldId);

            //_PanelCollectionContext.Actions.Clear();
            //_PanelCollectionContext.Actions.Add(UpdateHeroesSelectedAndMaxLabel);
            UpdateHeroesSelectedAndMaxLabel();

            panelCollection__prefab.InstantiateCollection(ECollectionMode.Hero);
            SceneOnResized();
        }

        public void Hide()
        {
            battleStarting = false;
            panelCollection__prefab.UnselectAll();
            _GameObject.SetActive(false);
            SceneOnResized();
        }

        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right=0)
        {
            if (!_GameObject.activeSelf)
            {
                return;
            }

            PanelTop__prefab__context.OnResized(coefHeight);
            float _height = Screen.height - PanelTop__prefab__context.height;
            float _width = Screen.width*0.3333f;
            _PanelBattlefield__RectTransform.sizeDelta = new Vector2(_width, _height);
            _HeroesSelectedAndMaxLabel__TextMeshProUGUI.fontSize = 70f * coefHeight;


            float offset = 20f * coefHeight;
            _HeroesSelectedAndMaxLabel__RectTransform.SetHorizontalOffsets(offset, offset);
            _HeroesSelectedAndMaxLabel__RectTransform.anchoredPosition = new Vector2(0, -offset);
            _HeroesSelectedAndMaxLabel__RectTransform.sizeDelta = new Vector2(0, 90f * coefHeight);


            _StartBattleButton__RectTransform.anchoredPosition = new Vector2(-25 * coefHeight, 25 * coefHeight);
            _StartBattleButton__RectTransform.sizeDelta = new Vector2(325 * coefHeight, 100 * coefHeight);


            panelCollection__prefab.OnResized(coefHeight, top: PanelTop__prefab__context.height, right: _width);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_RectTransform);
        }

        private void UpdateHeroesSelectedAndMaxLabel()
        {
            int selectedCount = panelCollection__prefab.GetSelectedElements().Count;
            int max = battlefield.maxHeroCount;
            _HeroesSelectedAndMaxLabel__TextMeshProUGUI.SetText($"{LM.GetValue(L.UI.Button.Heroes)} {selectedCount}/{max}");
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
                    GameSceneManager.Load(GameSceneManager.SceneName.Battlefield);
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
