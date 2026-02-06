using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Collection;
using General;
using System;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelSelectedEquipment
    {/// <summary>
     /// Ширина панели при разрешении 1920x1080.
     /// </summary>
        private const float WIDTH_BASE = 535f;

        private const float LABEL_HERO_NAME_FONTSIZE = 30f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        private const float BUTTON_WIDTH = 121.25f;
        private const float BUTTON_HEIGHT = 50f;
        private const float BUTTON_SPACING = 10f;
        private const float BUTTON_FONTSIZE = 15f;

        public PanelSelectedEquipment(PanelScene panelScene)
        {
            _PanelScene = panelScene;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipment (id=ta39338e)");
            _GameObject = _RectTransform.gameObject;

            _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentTop (id=dp54agcp)");
            _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=va8d3lsz)");
            _ButtonClose_RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(Hide);

            _PanelBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottom (id=bj3zvapm)");

            _PanelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=n94o21t8)");
            _PanelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=yjb1gqbc)");
            _PanelBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Item));

            _PanelBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=c1xjs5dr)");
            _PanelBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=pn28dhfr)");
            _PanelBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

            _ButtonTakeOnOff_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnOff (id=fllqlepl)");
            _ButtonTakeOnOff_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnOffText (id=xfqoucqj)");

            _ButtonSell_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonSell (id=sp1vha3z)");
            _ButtonSell_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonSellText (id=b68za6o5)");
            _ButtonSell_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Sell);

            _SelectedContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=bqxjhczr)");

            _SelectedEquipment_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull (id=gu7wtz83)");
            _SelectedEquipmentRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=qje8dq78)");
        }

        public PanelScene _PanelScene;
        public Guid EquipmentId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool IsVisible { get; private set; }

        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;

        private readonly RectTransform _PanelTop_RectTransform;
        private readonly RectTransform _ButtonClose_RectTransform;
        private readonly TextMeshProUGUI _LabelSelectedEquipment_TextMeshProUGUI;

        private readonly RectTransform _PanelBottom_RectTransform;
        private readonly RectTransform _PanelBottomTabButton1_RectTransform;
        private readonly RectTransform _PanelBottomTabButton2_RectTransform;
        private readonly TextMeshProUGUI _PanelBottomTabButton1_TextMeshProUGUI;
        private readonly TextMeshProUGUI _PanelBottomTabButton2_TextMeshProUGUI;
        private readonly RectTransform _ButtonTakeOnOff_RectTransform;
        private readonly TextMeshProUGUI _ButtonTakeOnOff_TextMeshProUGUI;
        private readonly RectTransform _ButtonSell_RectTransform;
        private readonly TextMeshProUGUI _ButtonSell_TextMeshProUGUI;
        private readonly RectTransform _SelectedContainer_RectTransform;
        private readonly Image _SelectedEquipment_Image;
        private readonly Image _SelectedEquipmentRarity_Image;

        public void Show(CollectionElement collectionElement)
        {
            IsVisible = true;
            EquipmentId = collectionElement.Id;
            _LabelSelectedEquipment_TextMeshProUGUI.SetText(collectionElement.Name);
            string tagUnique = collectionElement.IsUnique ? "Unique-" : string.Empty;
            _SelectedEquipment_Image.sprite = AddressableCache.Equipments[$"{tagUnique}{collectionElement.Name}"];
            _SelectedEquipment_Image.preserveAspect = true; // Сохраняет пропорции изображения
            _SelectedEquipmentRarity_Image.sprite = AddressableCache.Rarityes[collectionElement.Rarity];

            _ButtonTakeOnOff_RectTransform.gameObject.SetClickEvent(OnClick, true);

            // тут нужна логика одет предмет или снят
            _ButtonTakeOnOff_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.TakeOn));

            _GameObject.SetActive(true);
            _PanelScene.OnResized();
        }

        public void Hide()
        {
            IsVisible = false;
            EquipmentId = Guid.Empty;
            _GameObject.SetActive(false);
            _PanelScene.OnResized();
        }

        public void OnResized()
        {
            if (!IsVisible)
            {
                return;
            }

            float coefHeight = G.GetCoefHeight();
            Width = WIDTH_BASE * coefHeight;
            float h1 = _PanelScene.PanelTop.Height;
            Height = Screen.height - h1;
            _RectTransform.sizeDelta.Set(Width, Height);

            // Верхняя панель где написано название экипировки
            _PanelTop_RectTransform.sizeDelta.Set(Width, h1);
            _ButtonClose_RectTransform.sizeDelta.Set(h1, h1);
            _LabelSelectedEquipment_TextMeshProUGUI.rectTransform.sizeDelta.Set(Width - h1, h1);
            _LabelSelectedEquipment_TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;


            // Нижняя панель с характеристиками экипировки
            _PanelBottom_RectTransform.sizeDelta.Set(Width, Height - h1);


            // Кнопки вкладок
            float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
            float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
            float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
            float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

            _PanelBottomTabButton1_RectTransform.sizeDelta.Set(tabButtonW, tabButtonH);
            _PanelBottomTabButton1_RectTransform.anchoredPosition.Set(tabButtonS, -tabButtonS);
            _PanelBottomTabButton1_TextMeshProUGUI.fontSize = tabFontSize;

            _PanelBottomTabButton2_RectTransform.sizeDelta.Set(tabButtonW, tabButtonH);
            _PanelBottomTabButton2_RectTransform.anchoredPosition.Set((tabButtonS * 2) + tabButtonW, -tabButtonS);
            _PanelBottomTabButton2_TextMeshProUGUI.fontSize = tabFontSize;

            // Кнопки "Надеть" "Продать"
            float buttonWidth = BUTTON_WIDTH * coefHeight;
            float buttonHeight = BUTTON_HEIGHT * coefHeight;
            float buttonSpacing = BUTTON_SPACING * coefHeight;
            _ButtonTakeOnOff_RectTransform.sizeDelta.Set(buttonWidth, buttonHeight);
            _ButtonTakeOnOff_RectTransform.anchoredPosition.Set(buttonSpacing, buttonSpacing);

            _ButtonSell_RectTransform.sizeDelta.Set(buttonWidth, buttonHeight);
            _ButtonSell_RectTransform.anchoredPosition.Set((buttonSpacing * 2) + buttonWidth, buttonSpacing);

            float imageWidth = Width - (buttonWidth * 2) - (buttonSpacing * 4);
            _SelectedContainer_RectTransform.anchoredPosition.Set(-buttonSpacing, buttonSpacing);
            _SelectedContainer_RectTransform.sizeDelta.Set(imageWidth, imageWidth);
        }
        private async UniTask OnClick() {
            await UniTask.Delay(1);
        }
    }
}
