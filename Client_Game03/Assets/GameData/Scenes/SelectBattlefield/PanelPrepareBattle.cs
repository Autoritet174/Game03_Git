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
    public class PanelPrepareBattle
    {
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly SelectBattlefieldViewerContext _Context;

        private readonly RectTransform _PanelBattlefield__RectTransform;

        private readonly GameObject _StartBattleButton__GameObject;
        private readonly RectTransform _StartBattleButton__RectTransform;

        private readonly RectTransform _HeroesSelectedAndMaxLabel__RectTransform;
        private readonly TextMeshProUGUI _HeroesSelectedAndMaxLabel__TextMeshProUGUI;

        private EBattleFiled _BattlefieldId;
        private bool _BattleStarting;

        private General.DTO.Entities.GameData.Battlefield battlefield = null;

        public PanelPrepareBattle()
        {
            _GameObject = GameObjectFinder.FindByName("PanelPrepareBattle");
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _RectTransform.SetHorizontalOffsets(0, 0);

            PanelCollection = GameObjectFinder.FindByName<PanelCollection__prefab__scriptMB>(startParent: _RectTransform);
            TopButtons = PanelCollection.TopButtons;
            Viewer = PanelCollection.Viewer;
            _Context = new SelectBattlefieldViewerContext(Viewer);

            PanelCollection.SetContext(new SelectBattlefieldCollectionContext());
            TopButtons.SetContext(new SelectBattlefieldTopButtonsContext());
            Viewer.SetContext(_Context);

            GameObjectFinder.FindByName("ImageButtonEquipments (id=vuhjngaz)", TopButtons.gameObject).SetActive(false);



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
                GameObject buttonClose__GameObject = GameObjectFinder.FindByName("PanelTop", _GameObject)
                   .GetComponent<PanelTop__prefab__scriptMB>()
                   .ButtonClose__RectTransform.gameObject;

                if (buttonClose__GameObject.TryGetComponent(out ButtonClose_Click_MoveToMainMenu clickClose))
                {
                    UnityEngine.Object.Destroy(clickClose);
                }

                buttonClose__GameObject.GetComponent<Button>().onClick.RemoveAllListeners();

                GameObjectFinder.FindByName("PanelTop", _GameObject)
                    .GetComponent<PanelTop__prefab__scriptMB>()
                    .ButtonClose__RectTransform.gameObject
                    .SetClickEvent(Cancel, useButtonComponent: true);
            }
        }

        public PanelCollection__prefab__scriptMB PanelCollection { get; }

        public PanelCollectionTopButtons__prefab__scriptMB TopButtons { get; }

        public PanelCollectionViewer__prefab__scriptMB Viewer { get; }

        public bool IsVisible => _GameObject.activeSelf;

        public async UniTask ShowAsync(EBattleFiled battlefieldId)
        {
            _BattlefieldId = battlefieldId;
            _BattleStarting = false;
            _Context.ClearSelection();
            TopButtons.ResetPageCurrent();
            _GameObject.SetActive(true);
            if (SelectBattlefieldSceneInitializator.IsConfigured)
            {
                SelectBattlefieldSceneInitializator.Instance.OnResized();
            }

            battlefield = Game03Client.GameData.Container.Battlefields.First(a => a.Id == battlefieldId);

            _Context.Actions.Clear();
            _Context.Actions.Add(UpdateHeroesSelectedAndMaxLabel);
            UpdateHeroesSelectedAndMaxLabel();

            await Viewer.InstantiateCollectionAsync(ECollectionMode.Hero);
        }

        public void Hide()
        {
            _BattleStarting = false;
            _Context.ClearSelection();
            _GameObject.SetActive(false);
        }

        public void OnResized()
        {
            if (!_GameObject.activeSelf)
            {
                return;
            }

            float coefHeight = G.GetCoefHeight();


            _PanelBattlefield__RectTransform.sizeDelta = new Vector2(0, Screen.height - (G.PANELTOP_HEIGHT * coefHeight));
            _HeroesSelectedAndMaxLabel__TextMeshProUGUI.fontSize = 70f * coefHeight;


            float offset = 20f * coefHeight;
            _HeroesSelectedAndMaxLabel__RectTransform.SetHorizontalOffsets(offset, offset);
            _HeroesSelectedAndMaxLabel__RectTransform.anchoredPosition = new Vector2(0, -offset);
            _HeroesSelectedAndMaxLabel__RectTransform.sizeDelta = new Vector2(0, 90f * coefHeight);


            _StartBattleButton__RectTransform.anchoredPosition = new Vector2(-25 * coefHeight, 25 * coefHeight);
            _StartBattleButton__RectTransform.sizeDelta = new Vector2(325 * coefHeight, 100 * coefHeight);


            PanelCollection.OnResized();
            TopButtons.OnResized();
            Viewer.OnResized();

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_RectTransform);
        }

        private void UpdateHeroesSelectedAndMaxLabel()
        {
            int selectedCount = _Context.GetSelectedHeroIds().Length;
            int max = battlefield?.MaxHeroCount ?? 0;
            _HeroesSelectedAndMaxLabel__TextMeshProUGUI.SetText($"{LM.GetValue(L.UI.Button.Heroes)} {selectedCount}/{max}");
        }

        private async UniTask StartBattleAsync()
        {
            if (_BattleStarting)
            {
                return;
            }

            Guid[] heroIds = _Context.GetSelectedHeroIds();
            if (heroIds.Length == 0)
            {
                GameMessage.Show(LM.GetValue(L.Info.SelectHero), true);
                return;
            }

            _BattleStarting = true;
            try
            {
                BattlefieldSceneInitializator.SpawnedBattlefield = await Game03Client.Battlefield.BattlefieldProvider.LoadBattlefieldAsync(
                    _BattlefieldId,
                    heroIds,
                    CancellationTokenManager.Create($"{nameof(PanelPrepareBattle)}.{nameof(StartBattleAsync)}"));

                if (BattlefieldSceneInitializator.SpawnedBattlefield != null)
                {
                    BattlefieldSceneInitializator.SpawnedBattlefield.SpawnedHeroPlayerList.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));
                    BattlefieldSceneInitializator.SpawnedBattlefield.SpawnedHeroEnemyList.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));
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
                _BattleStarting = false;
                GameMessage.Close();
            }
        }

        private async UniTask Cancel()
        {
            Hide();
            await UniTask.Yield();
        }

    }
}
