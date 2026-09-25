using Assets.GameData.Scripts;
using Game03Client.Collection;
using General;
using General.DTO.Entities.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection.Prefabs
{
    /// <summary>Показывает выбранного героя, его характеристики и слоты экипировки.</summary>
    public class PanelSelectedHero__prefab__scriptMB : MonoBehaviour, IPrefab
    {
        public bool initialized { get; private set; }

        public float width { get; private set; }

        public float height { get; private set; }

        /// <summary>Ширина панели при разрешении 1920x1080.</summary>
        private const float WIDTH_BASE = 535f;
        public const float WIDTH_SPACING = 10f;

        private const float LABEL_HERO_NAME_FONTSIZE = 30f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        private const float IMAGE_CONTAINER_SPACING = 10f;

        private const float BUTTON_CLOSE_SPACING = 5f;

        public Guid heroId { get; private set; }

        public bool isVisible { get; private set; }

        public float panelStatWidth { get; private set; }

        public float panelStatHeight { get; private set; }

        private RectTransform rectTransform;

        private RectTransform panelTop__RectTransform;
        private RectTransform buttonClose__RectTransform;
        private TextMeshProUGUI labelSelectedHero__TextMeshProUGUI;

        private RectTransform panelBottom__RectTransform;

        private RectTransform panelBottomTabButton1_RectTransform;
        private RectTransform panelBottomTabButton2_RectTransform;
        private TextMeshProUGUI panelBottomTabButton1_TextMeshProUGUI;
        private TextMeshProUGUI panelBottomTabButton2_TextMeshProUGUI;

        private RectTransform panelTab1_RectTransform;
        private List<Slot> slots;
        private RectTransform imageContainer_RectTransform;
        private Slot slotWeapon;

        private Image selectedHero_Image;
        private Image selectedHeroRarity_Image;

        //Stats
        private RectTransform panelStat_RectTransform;

        private Stat__prefab__script statLevel;
        private Stat__prefab__script statHealth;
        private Stat__prefab__script statStrength;
        private Stat__prefab__script statAgility;
        private Stat__prefab__script statIntelligence;
        private Stat__prefab__script statCritChance;
        private Stat__prefab__script statCritMultiplier;

        public Action sceneOnResized { get; set; }

        public PanelCollection__prefab__scriptMB panelCollection__prefab__context { get; set; }

        public PanelSelectedEquipment__prefab__scriptMB panelSelectedEquipment__context { get; set; }

        public void Initialize()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new(0f, 0f);

            // Верхняя панель
            {
                panelTop__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop", rectTransform);

                buttonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", panelTop__RectTransform);
                buttonClose__RectTransform.gameObject.SetClickOnGameObject(Hide);

                labelSelectedHero__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero", panelTop__RectTransform);
            }

            // Нижняя панель
            {
                panelBottom__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBottom", rectTransform);

                // кнопка "Вкладка 1"
                {
                    panelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1", panelBottom__RectTransform);
                    panelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text", panelBottomTabButton1_RectTransform);
                    panelBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment));
                }

                // кнопка "Вкладка 2"
                {
                    panelBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2", panelBottom__RectTransform);
                    panelBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text", panelBottomTabButton2_RectTransform);
                    panelBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");
                }

                // панель "Вкладка 1"
                {
                    panelTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTab1", panelBottom__RectTransform);

                    // Слоты
                    slots = new()
                    {
                        new Slot("Head", 1, 1, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.head),
                        new Slot("Armor", 2, 1, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.armor),
                        new Slot("Hands", 3, 1, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.hands),
                        new Slot("Feet", 4, 1, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.feet),
                        new Slot("Bracelet", 5, 1, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.bracelet),
                        new Slot("Ring", 1, 2, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.ring1, "1"),
                        new Slot("Ring", 2, 2, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.ring2, "2"),
                        new Slot("Neck", 3, 2, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.neck),
                        new Slot("Trinket", 4, 2, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.trinket1, "1"),
                        new Slot("Trinket", 5, 2, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.trinket2, "2"),
                        new Slot("Weapon", 1, 3, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.rightHand),
                        new Slot("WeaponShield", 2, 3, panelTab1_RectTransform, panelSelectedEquipment__context, ESlot.leftHand)
                    };

                    // Изображение героя
                    {
                        imageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container", panelTab1_RectTransform);
                        selectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull", imageContainer_RectTransform);
                        selectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", imageContainer_RectTransform);
                    }

                    // Панель статов
                    {
                        panelStat_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelStats", panelTab1_RectTransform);

                        statLevel = new("Level", 1, GameObjectFinder.FindByName("StatLevel", panelStat_RectTransform));
                        statHealth = new("Health", 2, GameObjectFinder.FindByName("StatHealth", panelStat_RectTransform));
                        statStrength = new("Strength", 3, GameObjectFinder.FindByName("StatStrength", panelStat_RectTransform));
                        statAgility = new("Agility", 4, GameObjectFinder.FindByName("StatAgility", panelStat_RectTransform));
                        statIntelligence = new("Intelligence", 5, GameObjectFinder.FindByName("StatIntelligence", panelStat_RectTransform));
                        statCritChance = new("CritChance", 6, GameObjectFinder.FindByName("StatCritChance", panelStat_RectTransform));
                        statCritMultiplier = new("CritMultiplier", 7, GameObjectFinder.FindByName("StatCritPower", panelStat_RectTransform));
                    }
                }
            }

            slotWeapon = slots.First(a => a.name == "Weapon");

            Hide();
        }

        public void Refresh()
        {
            Show(heroId);
        }

        public void Show(Guid heroId)
        {
            isVisible = true;
            this.heroId = heroId;
            Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.id == heroId);
            labelSelectedHero__TextMeshProUGUI.SetText(hero.baseHero.name);
            selectedHero_Image.sprite = AddressablePrefabProvider.GetHeroSprite(hero);
            selectedHero_Image.preserveAspect = true;

            selectedHeroRarity_Image.sprite = AddressablePrefabProvider.GetRarity(hero.baseHero.rarity);
            selectedHeroRarity_Image.preserveAspect = false;

            // отображаем всю одетую экипировку
            foreach (Slot slot in slots)
            {
                Equipment eqiup = CollectionProvider.GetCollectionEquipmentsFromCache()
                    .FirstOrDefault(a => a.slotId == slot.slotId && a.heroId == this.heroId);
                if (eqiup != null)
                {
                    slot.EquipmentTakeOn(eqiup.id);
                }
                else
                {
                    slot.EquipmentTakeOff();
                }
            }

            //Экипировка этого героя
            var equipments = CollectionProvider.GetCollectionEquipmentsFromCache().Where(a => a.heroId == heroId && a.stats != null).ToList();

            float bonus_Health = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.health).SelectMany(s => s.Value)).Sum();
            float bonus_Strength = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.strength).SelectMany(s => s.Value)).Sum();
            float bonus_Agility = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.agility).SelectMany(s => s.Value)).Sum();
            float bonus_Intelligence = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.intelligence).SelectMany(s => s.Value)).Sum();
            float bonus_CritChance = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.critChance).SelectMany(s => s.Value)).Sum();
            float bonus_CritMultiplier = equipments.SelectMany(e => e.stats.Where(s => s.Key == EStatType.critMultiplier).SelectMany(s => s.Value)).Sum();

            // Статы
            statLevel.SetValue(hero.level);
            statHealth.SetValue(hero.health + bonus_Health);
            statStrength.SetValue(hero.strength + bonus_Strength);
            statAgility.SetValue(hero.agility + bonus_Agility);
            statIntelligence.SetValue(hero.intelligence + bonus_Intelligence);
            statCritChance.SetValuePercent(hero.critChance + bonus_CritChance);
            statCritMultiplier.SetValuePercent(hero.critMultiplier + bonus_CritMultiplier);

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
            sceneOnResized();
        }

        private void Hide()
        {
            isVisible = false;
            SetViewerElementSelected(false);
            heroId = Guid.Empty;
            gameObject.SetActive(false);
            sceneOnResized();
        }

        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            if (!isVisible)
            {
                width = 0f;
                height = 0f;
                return;
            }

            width = WIDTH_BASE * coefHeight;
            height = Screen.height - top;
            rectTransform.sizeDelta = new(width, height);

            float h1 = G.PANELTOP_HEIGHT * coefHeight;
            // Верхняя панель где написано имя героя
            panelTop__RectTransform.sizeDelta = new(width, h1);

            float button_close_spacing = BUTTON_CLOSE_SPACING * coefHeight;
            float buttonCloseSize = h1 - (button_close_spacing * 2);
            buttonClose__RectTransform.sizeDelta = new(buttonCloseSize, buttonCloseSize);
            buttonClose__RectTransform.anchoredPosition = new(button_close_spacing, -button_close_spacing);

            labelSelectedHero__TextMeshProUGUI.rectTransform.sizeDelta = new(width - h1, h1);
            labelSelectedHero__TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;

            // Нижняя панель с характеристиками героя
            panelBottom__RectTransform.sizeDelta = new(width, height - h1);

            // Кнопки вкладок
            float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
            float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
            float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
            float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

            panelBottomTabButton1_RectTransform.sizeDelta = new(tabButtonW, tabButtonH);
            panelBottomTabButton1_RectTransform.anchoredPosition = new(tabButtonS, -tabButtonS);
            panelBottomTabButton1_TextMeshProUGUI.fontSize = tabFontSize;

            panelBottomTabButton2_RectTransform.sizeDelta = new(tabButtonW, tabButtonH);
            panelBottomTabButton2_RectTransform.anchoredPosition = new((tabButtonS * 2) + tabButtonW, -tabButtonS);
            panelBottomTabButton2_TextMeshProUGUI.fontSize = tabFontSize;

            slots.ForEach(a => a.OnResized());

            float panelTabHeight = height - h1 - tabButtonH - (tabButtonS * 2);
            panelTab1_RectTransform.sizeDelta = new(width, panelTabHeight);

            float imageContainerSpacing = IMAGE_CONTAINER_SPACING * coefHeight;
            imageContainer_RectTransform.anchoredPosition = new(imageContainerSpacing, imageContainerSpacing);

            float panelSlotSpacing = Slot.PANELSLOT_SPACING * coefHeight;
            float imageContainerHeight = panelTabHeight - slotWeapon.top - slotWeapon.height - panelSlotSpacing;
            float imageContainerWidth = imageContainerHeight / 1.75f;
            imageContainer_RectTransform.sizeDelta = new(imageContainerWidth, imageContainerHeight);

            // Stats
            panelStatWidth = width - (3f * panelSlotSpacing) - imageContainerWidth;
            panelStatHeight = panelStatWidth * 576f / 244.06f;
            panelStat_RectTransform.sizeDelta = new(panelStatWidth, panelStatHeight);
            panelStat_RectTransform.anchoredPosition = new(-imageContainerSpacing, imageContainerSpacing);
            statLevel.OnResized();
            statHealth.OnResized();
            statStrength.OnResized();
            statAgility.OnResized();
            statIntelligence.OnResized();
            statCritChance.OnResized();
            statCritMultiplier.OnResized();
        }

        private void SetViewerElementSelected(bool selected)
        {
            PanelIconCollectionElement element = panelCollection__prefab__context.GetElement(heroId);
            if (element == null)
            {
                return;
            }

            element.SetSelected(selected);
        }

    }
}
