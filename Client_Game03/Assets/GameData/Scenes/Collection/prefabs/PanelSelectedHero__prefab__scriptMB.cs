using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Collection;
using General;
using General.DTO.Entities.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I = CollectionSceneInitializator;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection.prefabs
{
    public class PanelSelectedHero__prefab__scriptMB : MonoBehaviour, IPrefab
    {
        public bool Initialized { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        /// <summary>
        /// Ширина панели при разрешении 1920x1080.
        /// </summary>
        private const float WIDTH_BASE = 535f;
        public const float WIDTH_SPACING = 10f;

        private const float LABEL_HERO_NAME_FONTSIZE = 30f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        private const float IMAGE_CONTAINER_SPACING = 10f;

        private const float BUTTON_CLOSE_SPACING = 5f;


        public Guid HeroId { get; private set; }
        public bool IsVisible { get; private set; }
        public float PanelStatWidth { get; private set; }
        public float PanelStatHeight { get; private set; }


        private RectTransform _RectTransform;

        private RectTransform _PanelTop__RectTransform;
        private RectTransform _ButtonClose__RectTransform;
        private TextMeshProUGUI _LabelSelectedHero__TextMeshProUGUI;

        private RectTransform _PanelBottom__RectTransform;

        private RectTransform _PanelBottomTabButton1_RectTransform;
        private RectTransform _PanelBottomTabButton2_RectTransform;
        private TextMeshProUGUI _PanelBottomTabButton1_TextMeshProUGUI;
        private TextMeshProUGUI _PanelBottomTabButton2_TextMeshProUGUI;

        private RectTransform _PanelTab1_RectTransform;
        private List<Slot> _Slots;
        private RectTransform _ImageContainer_RectTransform;
        private Slot _SlotWeapon;

        private Image _SelectedHero_Image;
        private Image _SelectedHeroRarity_Image;


        //Stats
        private RectTransform _PanelStat_RectTransform;

        private Stat__prefab__script _StatLevel;
        private Stat__prefab__script _StatHealth;
        private Stat__prefab__script _StatStrength;
        private Stat__prefab__script _StatAgility;
        private Stat__prefab__script _StatIntelligence;
        private Stat__prefab__script _StatCritChance;
        private Stat__prefab__script _StatCritMultiplier;

        public Action SceneOnResized { get; set; }
        public PanelCollection__prefab__scriptMB PanelCollection__prefab__context { get; set; }
        public PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipment__context { get; set; }

        public void Initialize()
        {
            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchoredPosition = new Vector2(0f, 0f);


            // Верхняя панель
            {
                _PanelTop__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop", _RectTransform);

                _ButtonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", _PanelTop__RectTransform);
                _ButtonClose__RectTransform.gameObject.SetClickEvent(HideAsync, false);

                _LabelSelectedHero__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero", _PanelTop__RectTransform);
            }


            // Нижняя панель
            {
                _PanelBottom__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBottom", _RectTransform);

                // кнопка "Вкладка 1"
                {
                    _PanelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1", _PanelBottom__RectTransform);
                    _PanelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text", _PanelBottomTabButton1_RectTransform);
                    _PanelBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment));
                }

                // кнопка "Вкладка 2"
                {
                    _PanelBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2", _PanelBottom__RectTransform);
                    _PanelBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text", _PanelBottomTabButton2_RectTransform);
                    _PanelBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");
                }

                // панель "Вкладка 1"
                {
                    _PanelTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTab1", _PanelBottom__RectTransform);

                    // Слоты
                    _Slots = new()
                    {
                        new Slot("Head", 1, 1, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Head),
                        new Slot("Armor", 2, 1, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Armor),
                        new Slot("Hands", 3, 1, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Hands),
                        new Slot("Feet", 4, 1, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Feet),
                        new Slot("Bracelet", 5, 1, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Bracelet),
                        new Slot("Ring", 1, 2, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Ring1, "1"),
                        new Slot("Ring", 2, 2, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Ring2, "2"),
                        new Slot("Neck", 3, 2, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Neck),
                        new Slot("Trinket", 4, 2, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Trinket1, "1"),
                        new Slot("Trinket", 5, 2, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.Trinket2, "2"),
                        new Slot("Weapon", 1, 3, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.RightHand),
                        new Slot("WeaponShield", 2, 3, _PanelTab1_RectTransform, PanelSelectedEquipment__context, ESlot.LeftHand)
                    };

                    // Изображение героя
                    {
                        _ImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container", _PanelTab1_RectTransform);
                        _SelectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull", _ImageContainer_RectTransform);
                        _SelectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", _ImageContainer_RectTransform);
                    }

                    // Панель статов
                    {
                        _PanelStat_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelStats", _PanelTab1_RectTransform);

                        _StatLevel = new Stat__prefab__script("Level", 1, GameObjectFinder.FindByName("StatLevel", _PanelStat_RectTransform));
                        _StatHealth = new Stat__prefab__script("Health", 2, GameObjectFinder.FindByName("StatHealth", _PanelStat_RectTransform));
                        _StatStrength = new Stat__prefab__script("Strength", 3, GameObjectFinder.FindByName("StatStrength", _PanelStat_RectTransform));
                        _StatAgility = new Stat__prefab__script("Agility", 4, GameObjectFinder.FindByName("StatAgility", _PanelStat_RectTransform));
                        _StatIntelligence = new Stat__prefab__script("Intelligence", 5, GameObjectFinder.FindByName("StatIntelligence", _PanelStat_RectTransform));
                        _StatCritChance = new Stat__prefab__script("CritChance", 6, GameObjectFinder.FindByName("StatCritChance", _PanelStat_RectTransform));
                        _StatCritMultiplier = new Stat__prefab__script("CritMultiplier", 7, GameObjectFinder.FindByName("StatCritPower", _PanelStat_RectTransform));
                    }
                }
            }


            _SlotWeapon = _Slots.First(a => a.Name == "Weapon");

            Hide();
        }

        public void Refresh()
        {
            Show(HeroId);
        }
        public void Show(Guid heroId)
        {
            IsVisible = true;
            HeroId = heroId;
            Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.Id == heroId);
            _LabelSelectedHero__TextMeshProUGUI.SetText(hero.BaseHero.Name);
            _SelectedHero_Image.sprite = AddressableCache.GetHeroSprite(hero);
            _SelectedHero_Image.preserveAspect = true;

            _SelectedHeroRarity_Image.sprite = AddressableCache.GetRarity(hero.BaseHero.Rarity);
            _SelectedHeroRarity_Image.preserveAspect = false;

            // отображаем всю одетую экипировку
            foreach (Slot slot in _Slots)
            {
                Equipment eqiup = CollectionProvider.GetCollectionEquipmentsFromCache()
                    .FirstOrDefault(a => a.SlotId == slot.SlotId && a.HeroId == HeroId);
                if (eqiup != null)
                {
                    slot.EquipmentTakeOn(eqiup.Id);
                }
                else
                {
                    slot.EquipmentTakeOff();
                }
            }



            //Экипировка этого героя
            var equipments = CollectionProvider.GetCollectionEquipmentsFromCache().Where(a => a.HeroId == heroId && a.Stats != null).ToList();

            float bonus_Health = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.Health).SelectMany(s => s.Value)).Sum();
            float bonus_Strength = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.Strength).SelectMany(s => s.Value)).Sum();
            float bonus_Agility = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.Agility).SelectMany(s => s.Value)).Sum();
            float bonus_Intelligence = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.Intelligence).SelectMany(s => s.Value)).Sum();
            float bonus_CritChance = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.CritChance).SelectMany(s => s.Value)).Sum();
            float bonus_CritMultiplier = equipments.SelectMany(e => e.Stats.Where(s => s.Key == EStatType.CritMultiplier).SelectMany(s => s.Value)).Sum();

            // Статы
            _StatLevel.SetValue(hero.Level);
            _StatHealth.SetValue(hero.Health + bonus_Health);
            _StatStrength.SetValue(hero.Strength + bonus_Strength);
            _StatAgility.SetValue(hero.Agility + bonus_Agility);
            _StatIntelligence.SetValue(hero.Intelligence + bonus_Intelligence);
            _StatCritChance.SetValuePercent(hero.CritChance + bonus_CritChance);
            _StatCritMultiplier.SetValuePercent(hero.CritMultiplier + bonus_CritMultiplier);


            // Изменения статов если выбран предмет
            /*if (_PanelSelectedEquipment != null && _PanelSelectedEquipment.EquipmentId != Guid.Empty)
            {
                // Создаем виртуальный предмет который сейчас выбран
                DtoEquipment equipmentNow = CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.Id == _PanelSelectedEquipment.EquipmentId);
                if (equipmentNow.HeroId != hero.Id)
                {
                    DtoEquipment vEquipment = equipmentNow.CreateCopy();

                    // Создаем виртуального героя
                    DtoHero vHero = hero.CreateCopy();

                    // Создаем виртуальные предметы надетые в данный момент на реального героя
                    var vEquipments = new DtoEquipment[equipments.Count];
                    for (int i = 0; i < equipments.Count; i++)
                    {
                        vEquipments[i] = equipments[i].CreateCopy();
                    }
                }
            }*/



            SetViewerElementSelected(true);
            gameObject.SetActive(true);
            SceneOnResized();
        }


        private void Hide()
        {
            IsVisible = false;
            SetViewerElementSelected(false);
            HeroId = Guid.Empty;
            gameObject.SetActive(false);
            SceneOnResized();
        }

        private UniTask HideAsync()
        {
            Hide();
            return UniTask.CompletedTask;
        }

        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            if (!IsVisible)
            {
                Width = 0f;
                Height = 0f;
                return;
            }

            Width = WIDTH_BASE * coefHeight;
            Height = Screen.height - top;
            _RectTransform.sizeDelta = new Vector2(Width, Height);

            float h1 = G.PANELTOP_HEIGHT * coefHeight;
            // Верхняя панель где написано имя героя
            _PanelTop__RectTransform.sizeDelta = new Vector2(Width, h1);

            float button_close_spacing = BUTTON_CLOSE_SPACING * coefHeight;
            float buttonCloseSize = h1 - (button_close_spacing * 2);
            _ButtonClose__RectTransform.sizeDelta = new Vector2(buttonCloseSize, buttonCloseSize);
            _ButtonClose__RectTransform.anchoredPosition = new Vector2(button_close_spacing, -button_close_spacing);

            _LabelSelectedHero__TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(Width - h1, h1);
            _LabelSelectedHero__TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;


            // Нижняя панель с характеристиками героя
            _PanelBottom__RectTransform.sizeDelta = new Vector2(Width, Height - h1);


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

            float imageContainerSpacing = IMAGE_CONTAINER_SPACING * coefHeight;
            _ImageContainer_RectTransform.anchoredPosition = new Vector2(imageContainerSpacing, imageContainerSpacing);

            float panelSlotSpacing = Slot.PANELSLOT_SPACING * coefHeight;
            float imageContainerHeight = panelTabHeight - _SlotWeapon.Top - _SlotWeapon.Height - panelSlotSpacing;
            float imageContainerWidth = imageContainerHeight / 1.75f;
            _ImageContainer_RectTransform.sizeDelta = new Vector2(imageContainerWidth, imageContainerHeight);

            // Stats
            PanelStatWidth = Width - (3f * panelSlotSpacing) - imageContainerWidth;
            PanelStatHeight = PanelStatWidth * 576f / 244.06f;
            _PanelStat_RectTransform.sizeDelta = new Vector2(PanelStatWidth, PanelStatHeight);
            _PanelStat_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, imageContainerSpacing);
            _StatLevel.OnResized();
            _StatHealth.OnResized();
            _StatStrength.OnResized();
            _StatAgility.OnResized();
            _StatIntelligence.OnResized();
            _StatCritChance.OnResized();
            _StatCritMultiplier.OnResized();
        }

        private void SetViewerElementSelected(bool selected)
        {
            PanelIconCollectionElement element = PanelCollection__prefab__context.GetElement(HeroId);
            if (element == null)
            {
                return;
            }

            element.Selected(selected);
        }
       
    }
}
