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
using I = CollectionSceneInitializator;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection.prefabs
{
    public class PanelSelectedEquipment__prefab__scriptMB : MonoBehaviour
    {
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

        public PanelScene _PanelScene { get; private set; }
        public Guid EquipmentId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool IsVisible { get; private set; }
        public bool IsEquipped { get; private set; }

        private RectTransform _RectTransform;

        private RectTransform _PanelTop_RectTransform;
        private RectTransform _ButtonClose_RectTransform;
        private TextMeshProUGUI _LabelSelectedEquipment_TextMeshProUGUI;

        private RectTransform _PanelBottom__RectTransform;

        private RectTransform _PanelBottomTabButton1__RectTransform;
        private RectTransform _PanelBottomTabButton2__RectTransform;

        private TextMeshProUGUI _PanelBottomTabButton1__TextMeshProUGUI;
        private TextMeshProUGUI _PanelBottomTabButton2__TextMeshProUGUI;

        private RectTransform _ButtonTakeOnOff_RectTransform;
        private TextMeshProUGUI _ButtonTakeOnOff_TextMeshProUGUI;

        private RectTransform _ButtonTakeOnAlt_RectTransform;
        private TextMeshProUGUI _ButtonTakeOnAlt_TextMeshProUGUI;
        private Button _ButtonTakeOnAlt_Button;

        private RectTransform _ButtonSell_RectTransform;
        private TextMeshProUGUI _ButtonSell_TextMeshProUGUI;

        private RectTransform _ButtonShowHero_RectTransform;
        private TextMeshProUGUI _ButtonShowHero_TextMeshProUGUI;
        private Button _ButtonShowHero_Button;

        private RectTransform _ImageContainer_RectTransform;
        private Image _SelectedEquipment_Image;
        private Image _SelectedEquipmentRarity_Image;
        private Equipment _DtoEquipment;

        private RectTransform _PanelTab1__RectTransform;
        private RectTransform _PanelStat_RectTransform;
        private Stat__prefab__script _StatLevel;
        private readonly Stat__prefab__script[] _Stats = new Stat__prefab__script[7];

        private void Start()
        {
            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchoredPosition = new Vector2(0f, 0f);


            // Верхняя панель
            {
                _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop", gameObject);
                _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", _PanelTop_RectTransform);
                _ButtonClose_RectTransform.gameObject.SetClickEvent(Hide, false);
                _LabelSelectedEquipment_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedEquipment", _PanelTop_RectTransform);
            }


            // Нижняя панель
            {
                _PanelBottom__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelBottom", gameObject);

                // кнопка "Вкладка 1"
                {
                    _PanelBottomTabButton1__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1", _PanelBottom__RectTransform);
                    _PanelBottomTabButton1__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text", _PanelBottomTabButton1__RectTransform);
                    _PanelBottomTabButton1__TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.Item));
                }

                // кнопка "Вкладка 2"
                {
                    _PanelBottomTabButton2__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2", _PanelBottom__RectTransform);
                    _PanelBottomTabButton2__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text", _PanelBottomTabButton2__RectTransform);
                    _PanelBottomTabButton2__TextMeshProUGUI.SetText("{Tab2}");
                }


                // панель "Вкладка 1"
                {
                    _PanelTab1__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTab1", _PanelBottom__RectTransform);

                    // Кнопка "Продать"
                    {
                        _ButtonSell_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonSell", _PanelTab1__RectTransform);
                        _ButtonSell_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonSellText", _ButtonSell_RectTransform);
                        _ButtonSell_TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.Sell);
                    }

                    // Кнопка "Надеть/Снять"
                    {
                        _ButtonTakeOnOff_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnOff", _PanelTab1__RectTransform);
                        _ButtonTakeOnOff_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnOffText", _ButtonTakeOnOff_RectTransform);
                        _ButtonTakeOnOff_RectTransform.gameObject.SetClickEvent(TakeOnOffOnClickAsync, true);
                    }

                    // Кнопка "Надеть в другой слот"
                    {
                        _ButtonTakeOnAlt_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnAlt", _PanelTab1__RectTransform);
                        _ButtonTakeOnAlt_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnAltText", _ButtonTakeOnAlt_RectTransform);
                        _ButtonTakeOnAlt_TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.TakeOnAlt));
                        _ButtonTakeOnAlt_Button = _ButtonTakeOnAlt_RectTransform.gameObject.GetComponent<Button>();
                        _ButtonTakeOnAlt_RectTransform.gameObject.SetClickEvent(TakeOnOffInAltSlotOnClickAsync, true);
                    }

                    // Кнопка "Показать героя"
                    {
                        _ButtonShowHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonShowHero", _PanelTab1__RectTransform);
                        _ButtonShowHero_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonShowHeroText", _ButtonShowHero_RectTransform);
                        _ButtonShowHero_TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.ShowHero);
                        _ButtonShowHero_Button = _ButtonShowHero_RectTransform.gameObject.GetComponent<Button>();
                    }

                    // Изображение предмета
                    {
                        _ImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container", _PanelTab1__RectTransform);
                        _SelectedEquipment_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull", _ImageContainer_RectTransform);
                        _SelectedEquipmentRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", _ImageContainer_RectTransform);
                        _ButtonShowHero_RectTransform.gameObject.SetClickEvent(ShowHeroOnClickAsync, true);
                    }

                    // Статы
                    {
                        _PanelStat_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelStats", _PanelTab1__RectTransform);

                        _StatLevel = new Stat__prefab__script("Level", 1, GameObjectFinder.FindByName("StatLevel", _PanelStat_RectTransform));
                        for (int i = 0; i < _Stats.Length; i++)
                        {
                            string name = $"Stat{i + 1}";
                            _Stats[i] = new Stat__prefab__script(name, i + 2, GameObjectFinder.FindByName(name, _PanelStat_RectTransform));
                        }

                    }
                }
            }

            Hide().GetAwaiter().GetResult();
        }

        public void Show(Guid equipmentId)
        {
            IsVisible = true;
            EquipmentId = equipmentId;
            _DtoEquipment = CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.Id == equipmentId);
            _LabelSelectedEquipment_TextMeshProUGUI.SetText(_DtoEquipment.BaseEquipment.Name);
            _SelectedEquipment_Image.sprite = AddressableCache.Equipments[_DtoEquipment.BaseEquipment.Name];
            _SelectedEquipment_Image.preserveAspect = true; // Сохраняет пропорции изображения
            _SelectedEquipmentRarity_Image.sprite = AddressableCache.GetRarity(_DtoEquipment.BaseEquipment.Rarity);

            IsEquipped = CollectionProvider.EquipmentIsEquipped(EquipmentId);

            _StatLevel.SetValue(_DtoEquipment.Level);

            int i = 0;
            if (_DtoEquipment.Stats != null)
            {
                foreach (KeyValuePair<EStatType, List<float>> stat in _DtoEquipment.Stats)
                {
                    foreach (float value in stat.Value)
                    {
                        Stat__prefab__script statLocal = _Stats[i];
                        statLocal.SetActive(true);
                        statLocal.RefreshName(stat.Key.ToString());
                        statLocal.SetValue(value);
                        i++;
                    }
                }
            }
            for (; i < _Stats.Length; i++)
            {
                _Stats[i].SetActive(false);
            }


            UpdateButtons();

            I.PanelCollectionViewerInstance.GetElement(equipmentId)?.Selected(true);

            gameObject.SetActive(true);
            I.OnResized();
        }

        public async UniTask Hide()
        {
            IsVisible = false;
            I.PanelCollectionViewerInstance.GetElement(EquipmentId)?.Selected(false);
            EquipmentId = Guid.Empty;
            gameObject.SetActive(false);
            I.OnResized();
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
            float h1 = I.PanelTopInstance.Height;
            Height = Screen.height - h1;
            _RectTransform.sizeDelta = new Vector2(Width, Height);
            _RectTransform.anchoredPosition = new Vector2(-I.PanelSelectedHeroInstance.Width - WIDTH_SPACING, 0f);

            // Верхняя панель где написано название экипировки
            _PanelTop_RectTransform.sizeDelta = new Vector2(Width, h1);

            float button_close_spacing = BUTTON_CLOSE_SPACING * coefHeight;
            float buttonCloseSize = h1 - (button_close_spacing * 2);
            _ButtonClose_RectTransform.sizeDelta = new Vector2(buttonCloseSize, buttonCloseSize);
            _ButtonClose_RectTransform.anchoredPosition = new Vector2(button_close_spacing, -button_close_spacing);

            _LabelSelectedEquipment_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(Width - h1, h1);
            _LabelSelectedEquipment_TextMeshProUGUI.fontSize = LABEL_HERO_NAME_FONTSIZE * coefHeight;


            // Нижняя панель с характеристиками экипировки
            _PanelBottom__RectTransform.sizeDelta = new Vector2(Width, Height - h1);


            // Кнопки вкладок
            float tabButtonW = TAB_BUTTON_WIDTH * coefHeight;
            float tabButtonH = TAB_BUTTON_HEIGHT * coefHeight;
            float tabButtonS = TAB_BUTTON_SPACING * coefHeight;
            float tabFontSize = TAB_BUTTON_FONTSIZE * coefHeight;

            _PanelBottomTabButton1__RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            _PanelBottomTabButton1__RectTransform.anchoredPosition = new Vector2(tabButtonS, -tabButtonS);
            _PanelBottomTabButton1__TextMeshProUGUI.fontSize = tabFontSize;

            _PanelBottomTabButton2__RectTransform.sizeDelta = new Vector2(tabButtonW, tabButtonH);
            _PanelBottomTabButton2__RectTransform.anchoredPosition = new Vector2((tabButtonS * 2) + tabButtonW, -tabButtonS);
            _PanelBottomTabButton2__TextMeshProUGUI.fontSize = tabFontSize;


            float imageContainerSpacing = IMAGE_CONTAINER_SPACING * coefHeight;
            float imageContainerWidth = (Width - (imageContainerSpacing * 3f)) / 2f;
            _ImageContainer_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, imageContainerSpacing);
            _ImageContainer_RectTransform.sizeDelta = new Vector2(imageContainerWidth, imageContainerWidth);


            // Кнопки
            float buttonHeight = BUTTON_HEIGHT * coefHeight;
            float buttonY = 0f;

            buttonY += (imageContainerSpacing * 2f) + imageContainerWidth;
            _ButtonShowHero_RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            _ButtonShowHero_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            _ButtonSell_RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            _ButtonSell_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            _ButtonTakeOnAlt_RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            _ButtonTakeOnAlt_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            buttonY += imageContainerSpacing + buttonHeight;
            _ButtonTakeOnOff_RectTransform.sizeDelta = new Vector2(imageContainerWidth, buttonHeight);
            _ButtonTakeOnOff_RectTransform.anchoredPosition = new Vector2(-imageContainerSpacing, buttonY);

            float fontSize = 15 * coefHeight;
            _ButtonTakeOnOff_TextMeshProUGUI.fontSize = fontSize;
            _ButtonTakeOnAlt_TextMeshProUGUI.fontSize = fontSize;
            _ButtonShowHero_TextMeshProUGUI.fontSize = fontSize;
            _ButtonSell_TextMeshProUGUI.fontSize = fontSize;


            float panelTabHeight = Height - h1 - tabButtonH - (tabButtonS * 2);
            _PanelTab1__RectTransform.sizeDelta = new Vector2(Width, panelTabHeight);



            // Stats
            _PanelStat_RectTransform.anchoredPosition = new Vector2(0, imageContainerSpacing);
            float panelSlotSpacing = Slot.PANELSLOT_SPACING * coefHeight;
            float PanelStatWidth = Width - (3f * panelSlotSpacing) - imageContainerWidth;

            _PanelStat_RectTransform.sizeDelta = new Vector2(Width - (3f * panelSlotSpacing) - imageContainerWidth, PanelStatWidth * 576f / 244.06f);

            _StatLevel.OnResized();
            foreach (var i in _Stats)
            {
                i.OnResized();
            }
        }

        private async UniTask ShowHeroOnClickAsync()
        {
            if (_DtoEquipment.HeroId != null)
            {
                I.PanelSelectedHeroInstance.Show(_DtoEquipment.HeroId.Value);
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

        private async UniTask EquipmentTakeOnOffAsync(bool? inAltSlot = null)
        {
            bool result;
            if (IsEquipped)
            {
                // экипировка надета, снимаем
                _ = _DtoEquipment.SlotId.Value;
                Guid heroId = _DtoEquipment.HeroId.Value;
                result = await CollectionProvider.EquipmentTakeOffAsync(EquipmentId,
                    CancellationTokenManager.Create("CollectionProvider.EquipmentTakeOffAsync", 5));
                if (result)
                {
                    Show(EquipmentId);
                    I.PanelSelectedHeroInstance.Show(heroId);
                    I.PanelCollectionViewerInstance.GetElement(EquipmentId)?.RefreshOwnerImage();
                }
                else
                {
                    Debug.Log("Ошибка снятия экипировки");
                }
            }
            else
            {
                // экипировка не одета, одеваем
                Guid heroId = I.PanelSelectedHeroInstance.HeroId;
                if (heroId == Guid.Empty)
                {
                    await I.PanelTopInstance.OnClickHeroes();
                    GameMessage.Show(LocalizationManager.GetValue(L.Info.SelectHero), true);
                    return;
                }

                // тут запоминаем экипировку которая может быть одета в этот же слот
                Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.Id == heroId);
                ESlot slotId = CollectionProvider.GetSlotId(_DtoEquipment, inAltSlot);
                Equipment equipmentEquipped = CollectionProvider.GetCollectionEquipmentsFromCache().FirstOrDefault(a => a.HeroId == heroId && a.SlotId == slotId);


                result = await CollectionProvider.EquipmentTakeOnAsync(EquipmentId,
                    I.PanelSelectedHeroInstance.HeroId, inAltSlot,
                    CancellationTokenManager.Create("CollectionProvider.EquipmentTakeOnAsync", 5));
                if (result)
                {
                    if (equipmentEquipped != null)
                    {
                        //Если была одетая экипировка в этот слот, то снимаем её
                        equipmentEquipped.SlotId = null;
                        equipmentEquipped.HeroId = null;
                        I.PanelCollectionViewerInstance.GetElement(equipmentEquipped.Id)?.RefreshOwnerImage();
                    }


                    Show(EquipmentId);
                    I.PanelSelectedHeroInstance.Show(_DtoEquipment.HeroId.Value);
                    I.PanelCollectionViewerInstance.GetElement(EquipmentId)?.RefreshOwnerImage();
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
            if (IsEquipped)
            {
                _ButtonTakeOnAlt_Button.interactable = false;
                _ButtonShowHero_Button.interactable = true;
                textLocalKey = L.UI.Button.TakeOff;
            }
            else
            {
                _ButtonTakeOnAlt_Button.interactable = HaveAltSlot();
                _ButtonShowHero_Button.interactable = false;
                textLocalKey = L.UI.Button.TakeOn;
            }
            _ButtonTakeOnOff_TextMeshProUGUI.SetText(LocalizationManager.GetValue(textLocalKey));
        }

        private bool HaveAltSlot()
        {
            return _DtoEquipment != null && _DtoEquipment.BaseEquipment.EquipmentType.SlotType.HaveAltSlot;
        }


    }
}
