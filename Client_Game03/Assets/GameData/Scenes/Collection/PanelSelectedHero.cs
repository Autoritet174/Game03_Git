using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private const float LABEL_HERO_NAME_FONTSIZE = 30f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        private const float IMAGECONTAINER_SPACING = 10f;

        public PanelSelectedHero(PanelScene panelScene)
        {
            _PanelScene = panelScene;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
            _RectTransform.anchoredPosition = new Vector2(0f, 0f);
            _GameObject = _RectTransform.gameObject;

            _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroTop (id=0y6mrhc2)");
            _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=0ursxw0e)");
            _ButtonClose_RectTransform.gameObject.SetClickEvent(Hide, false);
            _LabelSelectedHero_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");

            _PanelBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottom (id=wejn6493)");
            _PanelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=uiufd2wv)");
            _PanelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=lf8q2aas)");
            _PanelBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment));

            _PanelBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=kzury0kd)");
            _PanelBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=6bjw6hi4)");
            _PanelBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

            _Slots = new()
            {
                new Slot("Head", 1, 1, _PanelBottom_RectTransform),
                new Slot("Armor", 2, 1, _PanelBottom_RectTransform),
                new Slot("Hands", 3, 1, _PanelBottom_RectTransform),
                new Slot("Feet", 4, 1, _PanelBottom_RectTransform),
                new Slot("Waist", 5, 1, _PanelBottom_RectTransform),
                new Slot("Ring", 1, 2, _PanelBottom_RectTransform, "1"),
                new Slot("Ring", 2, 2, _PanelBottom_RectTransform, "2"),
                new Slot("Neck", 3, 2, _PanelBottom_RectTransform),
                new Slot("Trinket", 4, 2, _PanelBottom_RectTransform, "1"),
                new Slot("Trinket", 5, 2, _PanelBottom_RectTransform, "2"),
                new Slot("Weapon", 1, 3, _PanelBottom_RectTransform),
                new Slot("WeaponShield", 2, 3, _PanelBottom_RectTransform)
            };

            _PanelTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottomTab1 (id=kn3yl79k)");
            _ImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=1l6gscif)");
            _SlotWeapon = _Slots.First(a => a.Name == "Weapon");

            _SelectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull (id=m5kn2f6p)");
            _SelectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=xami3s9q)");

            Hide().GetAwaiter().GetResult();
        }

        public PanelScene _PanelScene;
        public Guid HeroId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool IsVisible { get; private set; }

        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;

        private readonly RectTransform _PanelTop_RectTransform;
        private readonly RectTransform _ButtonClose_RectTransform;
        private readonly TextMeshProUGUI _LabelSelectedHero_TextMeshProUGUI;

        private readonly RectTransform _PanelBottom_RectTransform;

        private readonly RectTransform _PanelBottomTabButton1_RectTransform;
        private readonly RectTransform _PanelBottomTabButton2_RectTransform;
        private readonly TextMeshProUGUI _PanelBottomTabButton1_TextMeshProUGUI;
        private readonly TextMeshProUGUI _PanelBottomTabButton2_TextMeshProUGUI;

        private readonly RectTransform _PanelTab1_RectTransform;
        private readonly List<Slot> _Slots;
        private readonly RectTransform _ImageContainer_RectTransform;
        private readonly Slot _SlotWeapon;

        private readonly Image _SelectedHero_Image;
        private readonly Image _SelectedHeroRarity_Image;

        public void Show(CollectionElement collectionElement)
        {
            IsVisible = true;
            HeroId = collectionElement.Id;
            string tagUnique = collectionElement.IsUnique ? "Unique-" : string.Empty;
            _LabelSelectedHero_TextMeshProUGUI.SetText(collectionElement.Name);
            _SelectedHero_Image.sprite = AddressableCache.Heroes[$"{tagUnique}{collectionElement.Name}"];
            _SelectedHero_Image.preserveAspect = true; // Сохраняет пропорции изображения

            _SelectedHeroRarity_Image.sprite = AddressableCache.Rarityes[collectionElement.Rarity];
            _SelectedHeroRarity_Image.preserveAspect = false;
            _GameObject.SetActive(true);
            _PanelScene.OnResized();
        }

        public async UniTask Hide()
        {
            IsVisible = false;
            HeroId = Guid.Empty;
            _GameObject.SetActive(false);

            if (_PanelScene.CollectionMode == CollectionModeEnum.ChangingEquipment)
            {
                await _PanelScene.PanelCollection.PanelCollectionViewer.InstantiateCollectionAsync();
            }
            _PanelScene.OnResized();
        }

        public void OnResized()
        {
            if (!IsVisible)
            {
                Width = 0f;
                Height = 0f;
                return;
            }

            float coefHeight = G.GetCoefHeight();
            Width = WIDTH_BASE * coefHeight;
            float h1 = _PanelScene.PanelTop.Height;
            Height = Screen.height - h1;
            _RectTransform.sizeDelta = new Vector2(Width, Height);

            // Верхняя панель где написано имя героя
            _PanelTop_RectTransform.sizeDelta = new Vector2(Width, h1);
            _ButtonClose_RectTransform.sizeDelta = new Vector2(h1, h1);
            _LabelSelectedHero_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(Width - h1, h1);
            _LabelSelectedHero_TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;


            // Нижняя панель с характеристиками героя
            _PanelBottom_RectTransform.sizeDelta = new Vector2(Width, Height - h1);


            // Кнопки вкладок
            float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
            float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
            float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
            float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

            _PanelBottomTabButton1_RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            _PanelBottomTabButton1_RectTransform.anchoredPosition = new Vector2(tabButtonS, -tabButtonS);
            _PanelBottomTabButton1_TextMeshProUGUI.fontSize = tabFontSize;

            _PanelBottomTabButton2_RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            _PanelBottomTabButton2_RectTransform.anchoredPosition = new Vector2((tabButtonS * 2) + tabButtonW, -tabButtonS);
            _PanelBottomTabButton2_TextMeshProUGUI.fontSize = tabFontSize;


            _Slots.ForEach(a => a.OnResized());

            float panelTabHeight = Height - h1 - tabButtonH - (tabButtonS * 2);
            _PanelTab1_RectTransform.sizeDelta = new Vector2(Width, panelTabHeight);

            float imageContainerSpacing = IMAGECONTAINER_SPACING * coefHeight;
            _ImageContainer_RectTransform.anchoredPosition = new Vector2(imageContainerSpacing, imageContainerSpacing);

            float imageContainerHeight = panelTabHeight - _SlotWeapon.Top - _SlotWeapon.Height - (Slot.PANELSLOT_SPACING * coefHeight);
            _ImageContainer_RectTransform.sizeDelta = new Vector2(imageContainerHeight / 1.75f, imageContainerHeight);
        }

    }
}
