using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client;
using General;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class PanelPrepareBattle
    {
        private const float BUTTON_WIDTH = 220f;
        private const float BUTTON_HEIGHT = 60f;
        private const float BUTTON_MARGIN = 25f;
        private const float BUTTON_FONT_SIZE = 22f;
        private const float VIEWER_BOTTOM_OFFSET = 90f;

        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly PanelCollectionViewer__prefab__scriptMB _Viewer;
        private readonly SelectBattlefieldViewerContext _Context;
        private readonly RectTransform _ButtonStartBattle__RectTransform;
        private readonly RectTransform _ButtonCancel__RectTransform;
        private readonly TextMeshProUGUI _ButtonStartBattle__TextMeshProUGUI;
        private readonly TextMeshProUGUI _ButtonCancel__TextMeshProUGUI;

        //RectTransform _PanelTop__RectTransform;
        private readonly RectTransform viewerRectTransform;
        private EBattleFiled _BattlefieldId;
        private bool _BattleStarting;

        public PanelPrepareBattle()
        {
            _GameObject = GameObjectFinder.FindByName("PanelPrepareBattle");
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _RectTransform.SetHorizontalOffsets(0, 0);

            _Viewer = GameObjectFinder.FindByName<PanelCollectionViewer__prefab__scriptMB>(startParent: _RectTransform);
            _Context = new SelectBattlefieldViewerContext(_Viewer);
            _Viewer.SetContext(_Context);

            (_ButtonStartBattle__RectTransform, _ButtonStartBattle__TextMeshProUGUI) = CreateButton(
                "ButtonStartBattle",
                LocalizationManager.GetValue(L.UI.Button.StartBattle));
            (_ButtonCancel__RectTransform, _ButtonCancel__TextMeshProUGUI) = CreateButton(
                "ButtonCancelPrepareBattle",
                LocalizationManager.GetValue(L.UI.Button.Cancel));

            _ButtonStartBattle__RectTransform.SetParent(_RectTransform, false);
            _ButtonCancel__RectTransform.SetParent(_RectTransform, false);

            _ButtonStartBattle__RectTransform.gameObject
                .SetClickEvent(StartBattleAsync, useButtonComponent: true);
            _ButtonCancel__RectTransform.gameObject
                .SetClickEvent(Cancel, useButtonComponent: true);

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
            viewerRectTransform = _Viewer.GetComponent<RectTransform>();
        }

        public bool IsVisible => _GameObject.activeSelf;

        public async UniTask ShowAsync(EBattleFiled battlefieldId)
        {
            _BattlefieldId = battlefieldId;
            _BattleStarting = false;
            _Context.ClearSelection();
            _GameObject.SetActive(true);
            if (SelectBattlefieldSceneInitializator.IsConfigured)
            {
                SelectBattlefieldSceneInitializator.Instance.OnResized();
            }
            await _Viewer.InstantiateCollectionAsync(ECollectionMode.Hero);
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
            float buttonWidth = BUTTON_WIDTH * coefHeight;
            float buttonHeight = BUTTON_HEIGHT * coefHeight;
            float buttonMargin = BUTTON_MARGIN * coefHeight;
            float fontSize = BUTTON_FONT_SIZE * coefHeight;
            _ = VIEWER_BOTTOM_OFFSET * coefHeight;

            _ButtonStartBattle__RectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            _ButtonStartBattle__RectTransform.anchoredPosition = new Vector2(-buttonMargin, buttonMargin);
            _ButtonStartBattle__TextMeshProUGUI.fontSize = fontSize;

            _ButtonCancel__RectTransform.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            _ButtonCancel__RectTransform.anchorMin = new Vector2(0f, 0f);
            _ButtonCancel__RectTransform.anchorMax = new Vector2(0f, 0f);
            _ButtonCancel__RectTransform.pivot = new Vector2(0f, 0f);
            _ButtonCancel__RectTransform.anchoredPosition = new Vector2(buttonMargin, buttonMargin);
            _ButtonCancel__TextMeshProUGUI.fontSize = fontSize;


            //viewerRectTransform.anchorMin = new Vector2(0f, 0f);
            //viewerRectTransform.anchorMax = new Vector2(1f, 1f);
            //viewerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            //viewerRectTransform.anchoredPosition = new Vector2(0f, viewerBottomOffset * 0.5f);
            float panelTop_Height = G.PANELTOP_HEIGHT * coefHeight;
            viewerRectTransform.sizeDelta = new Vector2(0, Screen.height - panelTop_Height);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_RectTransform);
            _Viewer.OnResized();
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
                GameMessage.Show(LocalizationManager.GetValue(L.Info.SelectHero), true);
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

        private static (RectTransform rectTransform, TextMeshProUGUI textMeshProUGUI) CreateButton(string name, string text)
        {
            GameObject buttonObject = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            GameObject textObject = new("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(buttonObject.transform, false);
            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI textMeshProUGUI = textObject.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = text;
            textMeshProUGUI.alignment = TextAlignmentOptions.Center;
            textMeshProUGUI.color = Color.white;

            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(1f, 0f);

            return (rectTransform, textMeshProUGUI);
        }
    }
}
