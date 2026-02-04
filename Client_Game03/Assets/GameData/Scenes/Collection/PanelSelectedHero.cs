using Assets.GameData.Scripts;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelSelectedHero
    {
        /// <summary>
        /// Ширина панели при разрешении 1920x1080.
        /// </summary>
        private const float WIDTH_BASE = 535f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_LEFT = 5f;
        private const float TAB_BUTTON_TOP = 5f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        public PanelSelectedHero(PanelScene panelScene)
        {
            _PanelScene = panelScene;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
            _GameObject = _RectTransform.gameObject;

            _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroTop (id=0y6mrhc2)");
            _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=0ursxw0e)");
            _ButtonClose_RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(Hide);
            _LabelSelectedHero_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");

            _PanelBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottom (id=wejn6493)");
            _PanelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=uiufd2wv)");
            _PanelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=lf8q2aas)");
            _PanelBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment));
        }

        public PanelScene _PanelScene;
        public Guid HeroId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }


        private bool visible;
        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;

        private readonly RectTransform _PanelTop_RectTransform;
        private readonly RectTransform _ButtonClose_RectTransform;
        private readonly TextMeshProUGUI _LabelSelectedHero_TextMeshProUGUI;

        private RectTransform _PanelBottom_RectTransform;

        private RectTransform _PanelBottomTabButton1_RectTransform;
        private RectTransform _PanelBottomTabButton2_RectTransform;
        private TextMeshProUGUI _PanelBottomTabButton1_TextMeshProUGUI;
        private TextMeshProUGUI _PanelBottomTabButton2_TextMeshProUGUI;
        public void Show(Guid equipmentId)
        {
            visible = true;
            HeroId = equipmentId;
            _GameObject.SetActive(true);
            _PanelScene.OnResized();
        }

        public void Hide()
        {
            visible = false;
            HeroId = Guid.Empty;
            _GameObject.SetActive(false);
            _PanelScene.OnResized();
        }

        public void OnResized()
        {
            if (!visible)
            {
                Width = 0f;
                Height = 0f;
                return;
            }

            float coefHeight = Screen.height / 1080f;
            Width = WIDTH_BASE * coefHeight;
            float h1 = _PanelScene.PanelTop.Height;
            Height = Screen.height - h1;
            _RectTransform.sizeDelta.Set(Width, Height);

            // Верхняя панель где написано имя героя
            {
                _PanelTop_RectTransform.sizeDelta.Set(Width, h1);
                _ButtonClose_RectTransform.sizeDelta.Set(h1, h1);
                _LabelSelectedHero_TextMeshProUGUI.rectTransform.sizeDelta.Set(Width - h1, h1);
                _LabelSelectedHero_TextMeshProUGUI.fontSize = 30f * coefHeight;
            }

            // Нижняя панель с характеристиками героя
            {
                _PanelBottom_RectTransform.sizeDelta.Set(Width, Height - h1);

                // Кнопки вкладок
                {
                    float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
                    float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
                    float tabButtonL = TAB_BUTTON_LEFT * coefHeight;
                    float tabButtonT = TAB_BUTTON_TOP * coefHeight;
                    float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
                    float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

                    _PanelBottomTabButton1_RectTransform.sizeDelta.Set(tabButtonW, tabButtonH);
                    _PanelBottomTabButton1_RectTransform.anchoredPosition.Set(tabButtonL, -tabButtonT);
                    _PanelBottomTabButton1_TextMeshProUGUI.fontSize = tabFontSize;

                    _PanelBottomTabButton2_RectTransform.sizeDelta.Set(tabButtonW, tabButtonH);
                    _PanelBottomTabButton2_RectTransform.anchoredPosition.Set(tabButtonL + tabButtonW + tabButtonS, -tabButtonT);
                    _PanelBottomTabButton2_TextMeshProUGUI.fontSize = tabFontSize;
                }
            }

        }

    }
}
