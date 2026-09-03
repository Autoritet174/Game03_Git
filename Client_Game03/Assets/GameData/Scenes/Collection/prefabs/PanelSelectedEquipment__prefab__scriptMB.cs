using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client;
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
    public class PanelSelectedEquipment__prefab__scriptMB : MonoBehaviour, IPrefab
    {
        public bool initialized { get; private set; }
        public float width { get; private set; }
        public float height { get; private set; }

        private const float WIDTH_BASE = 535f;
        public const float WIDTH_SPACING = 10f;

        private const float LABEL_HERO_NAME_FONTSIZE = 30f;

        private const float TAB_BUTTON_WIDTH = 150f;
        private const float TAB_BUTTON_HEIGHT = 50f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float TAB_BUTTON_FONTSIZE = 15f;

        private const float BUTTON_WIDTH = 121.25f;
        private const float BUTTON_HEIGHT = 50f;
        private const float BUTTON_SPACING = 5f;
        private const float BUTTON_LEFT = 15f;
        private const float BUTTON_FONTSIZE = 15f;

        private const float BUTTON_CLOSE_SPACING = 5f;

        private const float IMAGE_CONTAINER_SPACING = 10f;

        public Guid equipmentId { get; private set; }
        public bool isVisible { get; private set; }
        public bool isEquipped { get; private set; }


        private RectTransform rectTransform;

        private RectTransform panelTop__RectTransform;
        private RectTransform buttonClose__RectTransform;
        private TextMeshProUGUI labelSelectedEquipment__TextMeshProUGUI;

        private RectTransform panelBottom__RectTransform;

        private RectTransform panelBottomTabButton1__RectTransform;
        private RectTransform panelBottomTabButton2__RectTransform;

        private TextMeshProUGUI panelBottomTabButton1__TextMeshProUGUI;
        private TextMeshProUGUI panelBottomTabButton2__TextMeshProUGUI;

        private RectTransform buttonTakeOnOff__RectTransform;
        private TextMeshProUGUI buttonTakeOnOff__TextMeshProUGUI;

        private RectTransform buttonTakeOnAlt__RectTransform;
        private TextMeshProUGUI buttonTakeOnAlt__TextMeshProUGUI;
        private Button buttonTakeOnAlt__Button;

        private RectTransform buttonSell__RectTransform;
        private TextMeshProUGUI buttonSell__TextMeshProUGUI;

        private RectTransform buttonShowHero__RectTransform;
        private TextMeshProUGUI buttonShowHero__TextMeshProUGUI;
        private Button buttonShowHero__Button;

        private RectTransform imageContainer__RectTransform;
        private Image selectedEquipment__Image;
        private Image selectedEquipmentRarity__Image;
        private Equipment equipment;

        private RectTransform panelTab1__RectTransform;
        private RectTransform panelStat__RectTransform;
        private Stat__prefab__script statLevel;
        private readonly Stat__prefab__script[] stats = new Stat__prefab__script[7];

        public Action sceneOnResized { get; set; }
        public Action tabButtonHeroesOnClick { get; set; }
        public PanelCollection__prefab__scriptMB panelCollectionContext { get; set; }
        public PanelSelectedHero__prefab__scriptMB panelSelectedHeroContext { get; set; }

        public void Initialize()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, 0f);


            // Верхняя панель
            {
                panelTop__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop", gameObject);
                buttonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", panelTop__RectTransform);
                buttonClose__RectTransform.gameObject.SetClickOnGameObject(Hide);
                labelSelectedEquipment__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedEquipment", panelTop__RectTransform);
            }


            // Нижняя панель
            {
                panelBottom__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBottom", gameObject);

                // кнопка "Вкладка 1"
                {
                    panelBottomTabButton1__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1", panelBottom__RectTransform);
                    panelBottomTabButton1__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text", panelBottomTabButton1__RectTransform);
                    panelBottomTabButton1__TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.Item));
                }

                // кнопка "Вкладка 2"
                {
                    panelBottomTabButton2__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2", panelBottom__RectTransform);
                    panelBottomTabButton2__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text", panelBottomTabButton2__RectTransform);
                    panelBottomTabButton2__TextMeshProUGUI.SetText("{Tab2}");
                }


                // панель "Вкладка 1"
                {
                    panelTab1__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTab1", panelBottom__RectTransform);

                    // Кнопка "Продать"
                    {
                        buttonSell__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonSell", panelTab1__RectTransform);
                        buttonSell__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonSellText", buttonSell__RectTransform);
                        buttonSell__TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.Sell);
                    }

                    // Кнопка "Надеть/Снять"
                    {
                        buttonTakeOnOff__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnOff", panelTab1__RectTransform);
                        buttonTakeOnOff__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnOffText", buttonTakeOnOff__RectTransform);
                        buttonTakeOnOff__RectTransform.gameObject.SetClickOnButton(TakeOnOffOnClickAsync);
                    }

                    // Кнопка "Надеть в другой слот"
                    {
                        buttonTakeOnAlt__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnAlt", panelTab1__RectTransform);
                        buttonTakeOnAlt__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnAltText", buttonTakeOnAlt__RectTransform);
                        buttonTakeOnAlt__TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.TakeOnAlt));
                        buttonTakeOnAlt__Button = buttonTakeOnAlt__RectTransform.gameObject.GetComponent<Button>();
                        buttonTakeOnAlt__RectTransform.gameObject.SetClickOnButton(TakeOnOffInAltSlotOnClickAsync);
                    }

                    // Кнопка "Показать героя"
                    {
                        buttonShowHero__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonShowHero", panelTab1__RectTransform);
                        buttonShowHero__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonShowHeroText", buttonShowHero__RectTransform);
                        buttonShowHero__TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.ShowHero);
                        buttonShowHero__Button = buttonShowHero__RectTransform.gameObject.GetComponent<Button>();
                    }

                    // Изображение предмета
                    {
                        imageContainer__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container", panelTab1__RectTransform);
                        selectedEquipment__Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull", imageContainer__RectTransform);
                        selectedEquipmentRarity__Image = GameObjectFinder.FindByName<Image>("ImageRarity", imageContainer__RectTransform);
                        buttonShowHero__RectTransform.gameObject.SetClickOnButton(ShowHeroOnClick);
                    }

                    // Статы
                    {
                        panelStat__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelStats", panelTab1__RectTransform);

                        statLevel = new Stat__prefab__script("Level", 1, GameObjectFinder.FindByName("StatLevel", panelStat__RectTransform));
                        for (int i = 0; i < stats.Length; i++)
                        {
                            string name = $"Stat{i + 1}";
                            stats[i] = new Stat__prefab__script(name, i + 2, GameObjectFinder.FindByName(name, panelStat__RectTransform));
                        }

                    }
                }
            }

            Hide();
        }

        public void Show(Guid equipmentId)
        {
            isVisible = true;
            this.equipmentId = equipmentId;
            equipment = CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.id == equipmentId);
            labelSelectedEquipment__TextMeshProUGUI.SetText(equipment.baseEquipment.name);
            selectedEquipment__Image.sprite = AddressablePrefabProvider.Equipments[equipment.baseEquipment.name];
            selectedEquipment__Image.preserveAspect = true; // Сохраняет пропорции изображения
            selectedEquipmentRarity__Image.sprite = AddressablePrefabProvider.GetRarity(equipment.baseEquipment.rarity);

            isEquipped = CollectionProvider.EquipmentIsEquipped(this.equipmentId);

            statLevel.SetValue(equipment.level);

            int i = 0;
            if (equipment.stats != null)
            {
                foreach (KeyValuePair<EStatType, List<float>> stat in equipment.stats)
                {
                    foreach (float value in stat.Value)
                    {
                        Stat__prefab__script statLocal = stats[i];
                        statLocal.SetActive(true);
                        statLocal.RefreshName(stat.Key.ToString());
                        statLocal.SetValue(value);
                        i++;
                    }
                }
            }
            for (; i < stats.Length; i++)
            {
                stats[i].SetActive(false);
            }


            UpdateButtons();

            SetViewerElementSelected(equipmentId, true);

            gameObject.SetActive(true);
            sceneOnResized();
        }

        private void Hide()
        {
            isVisible = false;
            SetViewerElementSelected(equipmentId, false);
            equipmentId = Guid.Empty;
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
            float h1 = G.PANELTOP_HEIGHT * coefHeight;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = new Vector2(-right - WIDTH_SPACING, 0f);

            // Верхняя панель где написано название экипировки
            panelTop__RectTransform.sizeDelta = new Vector2(width, h1);

            float button_close_spacing = BUTTON_CLOSE_SPACING * coefHeight;
            float buttonCloseSize = h1 - (button_close_spacing * 2);
            buttonClose__RectTransform.sizeDelta = new Vector2(buttonCloseSize, buttonCloseSize);
            buttonClose__RectTransform.anchoredPosition = new Vector2(button_close_spacing, -button_close_spacing);

            labelSelectedEquipment__TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(width - h1, h1);
            labelSelectedEquipment__TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;


            // Нижняя панель с характеристиками экипировки
            panelBottom__RectTransform.sizeDelta = new Vector2(width, height - h1);


            // Кнопки вкладок
            float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
            float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
            float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
            float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

            panelBottomTabButton1__RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            panelBottomTabButton1__RectTransform.anchoredPosition = new Vector2(tabButtonS, -tabButtonS);
            panelBottomTabButton1__TextMeshProUGUI.fontSize = tabFontSize;

            panelBottomTabButton2__RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            panelBottomTabButton2__RectTransform.anchoredPosition = new Vector2((tabButtonS * 2) + tabButtonW, -tabButtonS);
            panelBottomTabButton2__TextMeshProUGUI.fontSize = tabFontSize;


            float imageContainerSpacing = IMAGE_CONTAINER_SPACING * coefHeight;
            float imageContainerWidth = (width - (imageContainerSpacing * 3f)) / 2f;
            imageContainer__RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, imageContainerSpacing);
            imageContainer__RectTransform.sizeDelta = new Vector2(imageContainerWidth, imageContainerWidth);


            // Кнопки
            float buttonHeight = BUTTON_HEIGHT * coefHeight;
            float buttonY = 0f;

            buttonY += (imageContainerSpacing * 2f) + imageContainerWidth;
            buttonShowHero__RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            buttonShowHero__RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            buttonSell__RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            buttonSell__RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            buttonTakeOnAlt__RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            buttonTakeOnAlt__RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            buttonTakeOnOff__RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            buttonTakeOnOff__RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            float fontSize = 15 * coefHeight;
            buttonTakeOnOff__TextMeshProUGUI.fontSize = fontSize;
            buttonTakeOnAlt__TextMeshProUGUI.fontSize = fontSize;
            buttonShowHero__TextMeshProUGUI.fontSize = fontSize;
            buttonSell__TextMeshProUGUI.fontSize = fontSize;


            float panelTabHeight = height - h1 - tabButtonH - (tabButtonS * 2);
            panelTab1__RectTransform.sizeDelta = new Vector2(width, panelTabHeight);



            // Stats
            panelStat__RectTransform.anchoredPosition = new Vector2(0, imageContainerSpacing);
            float panelSlotSpacing = Slot.PANELSLOT_SPACING * coefHeight;
            float PanelStatWidth = width - (3f * panelSlotSpacing) - imageContainerWidth;

            panelStat__RectTransform.sizeDelta = new Vector2(width - (3f * panelSlotSpacing) - imageContainerWidth, PanelStatWidth * 576f / 244.06f);

            statLevel.OnResized();
            foreach (Stat__prefab__script i in stats)
            {
                i.OnResized();
            }
        }

        private async UniTask TakeOnOffOnClickAsync()
        {
            await EquipmentTakeOnOffAsync();
        }

        private async UniTask TakeOnOffInAltSlotOnClickAsync()
        {
            await EquipmentTakeOnOffAsync(true);
        }

        private void ShowHeroOnClick()
        {
            if (equipment.heroId != null)
            {
                panelSelectedHeroContext.Show(equipment.heroId.Value);
            }
        }

        private async UniTask EquipmentTakeOnOffAsync(bool? inAltSlot = null)
        {
            bool result;
            if (isEquipped)
            {
                // экипировка надета, снимаем

                _ = equipment.slotId.Value;
                Guid heroId = equipment.heroId.Value;
                result = await CollectionProvider.EquipmentTakeOffAsync(equipmentId,
                    CancellationTokenManager.Create("CollectionProvider.EquipmentTakeOffAsync", 5));
                if (result)
                {
                    Show(equipmentId);
                    panelSelectedHeroContext.Show(heroId);
                    RefreshViewerElementOwnerImage(equipmentId);
                }
                else
                {
                    Debug.Log("Ошибка снятия экипировки");
                }
            }
            else
            {
                // экипировка не одета, одеваем

                Guid heroId = panelSelectedHeroContext.HeroId;
                if (heroId == Guid.Empty)
                {
                    tabButtonHeroesOnClick();
                    GameMessage.Show(LocalizationManager.GetValue(L.Info.SelectHero), true);
                    return;
                }

                // тут запоминаем экипировку которая может быть одета в этот же слот
                Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.id == heroId);
                ESlot slotId = CollectionProvider.GetSlotId(equipment, inAltSlot);
                Equipment equipmentEquipped = CollectionProvider.GetCollectionEquipmentsFromCache().FirstOrDefault(a => a.heroId == heroId && a.slotId == slotId);


                result = await CollectionProvider.EquipmentTakeOnAsync(equipmentId,
                    panelSelectedHeroContext.HeroId, inAltSlot,
                    CancellationTokenManager.Create("CollectionProvider.EquipmentTakeOnAsync", 5));
                if (result)
                {
                    if (equipmentEquipped != null)
                    {
                        //Если была одетая экипировка в этот слот, то снимаем её
                        equipmentEquipped.slotId = null;
                        equipmentEquipped.heroId = null;
                        RefreshViewerElementOwnerImage(equipmentEquipped.id);
                    }


                    Show(equipmentId);
                    panelSelectedHeroContext.Show(equipment.heroId.Value);
                    RefreshViewerElementOwnerImage(equipmentId);
                }
                else
                {
                    Debug.Log("Ошибка одевания экипировки");
                }
            }
        }

        private void UpdateButtons()
        {
            string textLocalKey;
            if (isEquipped)
            {
                buttonTakeOnAlt__Button.interactable = false;
                buttonShowHero__Button.interactable = true;
                textLocalKey = L.UI.Button.TakeOff;
            }
            else
            {
                buttonTakeOnAlt__Button.interactable = HaveAltSlot();
                buttonShowHero__Button.interactable = false;
                textLocalKey = L.UI.Button.TakeOn;
            }
            buttonTakeOnOff__TextMeshProUGUI.SetText(LocalizationManager.GetValue(textLocalKey));
        }

        private bool HaveAltSlot()
        {
            return equipment != null && equipment.baseEquipment.equipmentType.slotType.haveAltSlot;
        }

        private void SetViewerElementSelected(Guid id, bool selected)
        {
            PanelCollection__prefab__scriptMB panelCollection = panelCollectionContext;
            if (panelCollection == null)
            {
                return;
            }

            PanelIconCollectionElement element = panelCollection.GetElement(id);
            if (element == null)
            {
                return;
            }

            element.SetSelected(selected);
        }

        private void RefreshViewerElementOwnerImage(Guid id)
        {
            PanelIconCollectionElement element = panelCollectionContext.GetElement(id);
            if (element == null)
            {
                return;
            }

            element.RefreshOwnerImage();
        }
    }
}
