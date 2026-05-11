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

namespace Assets.GameData.Scenes.Collection
{
    public class PanelSelectedEquipment
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

        public PanelSelectedEquipment()
        {
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipment (id=ta39338e)");
            _RectTransform.anchoredPosition = new Vector2(0f, 0f);
            _GameObject = _RectTransform.gameObject;

            _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentTop (id=dp54agcp)");
            _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=va8d3lsz)");
            _ButtonClose_RectTransform.gameObject.SetClickEvent(Hide, false);
            _LabelSelectedEquipment_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedEquipment (id=004gk90y)");

            _PanelBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottom (id=bj3zvapm)");

            _PanelBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=n94o21t8)");
            _PanelBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=yjb1gqbc)");
            _PanelBottomTabButton1_TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.Item));

            _PanelBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=c1xjs5dr)");
            _PanelBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=pn28dhfr)");
            _PanelBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

            _ButtonTakeOnOff_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnOff (id=fllqlepl)");
            _ButtonTakeOnOff_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnOffText (id=xfqoucqj)");

            _ButtonTakeOnAlt_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnAlt (id=t1aolr9g)");
            _ButtonTakeOnAlt_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnAltText (id=1kxgiw2d)");
            _ButtonTakeOnAlt_TextMeshProUGUI.SetText(LocalizationManager.GetValue(L.UI.Button.TakeOnAlt));
            _ButtonTakeOnAlt_Button = _ButtonTakeOnAlt_RectTransform.gameObject.GetComponent<Button>();

            _ButtonSell_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonSell (id=sp1vha3z)");
            _ButtonSell_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonSellText (id=b68za6o5)");
            _ButtonSell_TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.Sell);

            _ButtonShowHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonShowHero (id=1odbub2l)");
            _ButtonShowHero_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonShowHeroText (id=9u9bz66s)");
            _ButtonShowHero_TextMeshProUGUI.text = LocalizationManager.GetValue(L.UI.Button.ShowHero);
            _ButtonShowHero_Button = _ButtonShowHero_RectTransform.gameObject.GetComponent<Button>();

            _ImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=bqxjhczr)");

            _SelectedEquipment_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull (id=gu7wtz83)");
            _SelectedEquipmentRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=qje8dq78)");
            _ButtonTakeOnOff_RectTransform.gameObject.SetClickEvent(TakeOnOffOnClick, true);
            _ButtonTakeOnAlt_RectTransform.gameObject.SetClickEvent(TakeOnOffInAltSlotOnClick, true);
            _ButtonShowHero_RectTransform.gameObject.SetClickEvent(ShowHeroOnClick, true);

            _PanelTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottomTab1 (id=9nwzj7p8)");

            // Stats
            _PanelStat_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelStats (id=hqmporj6)");

            _StatLevel = new Stat("Level", 1, GameObjectFinder.FindByName("StatLevel", _PanelStat_RectTransform.transform));
            for (int i = 0; i < _Stats.Length; i++)
            {
                string name = $"Stat{i + 1}";
                _Stats[i] = new Stat(name, i + 1, GameObjectFinder.FindByName(name, _PanelStat_RectTransform.transform));
            }


            Hide().GetAwaiter().GetResult();
        }

        public PanelScene _PanelScene { get; private set; }
        public Guid EquipmentId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool IsVisible { get; private set; }
        public bool IsEquipped { get; private set; }

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

        private readonly RectTransform _ButtonTakeOnAlt_RectTransform;
        private readonly TextMeshProUGUI _ButtonTakeOnAlt_TextMeshProUGUI;
        private readonly Button _ButtonTakeOnAlt_Button;

        private readonly RectTransform _ButtonSell_RectTransform;
        private readonly TextMeshProUGUI _ButtonSell_TextMeshProUGUI;

        private readonly RectTransform _ButtonShowHero_RectTransform;
        private readonly TextMeshProUGUI _ButtonShowHero_TextMeshProUGUI;
        private readonly Button _ButtonShowHero_Button;

        private readonly RectTransform _ImageContainer_RectTransform;
        private readonly Image _SelectedEquipment_Image;
        private readonly Image _SelectedEquipmentRarity_Image;
        private Equipment _DtoEquipment;

        private readonly RectTransform _PanelTab1_RectTransform;
        private readonly RectTransform _PanelStat_RectTransform;
        private readonly Stat _StatLevel;
        private readonly Stat[] _Stats = new Stat[7];

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
                        Stat statLocal = _Stats[i];
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

            _GameObject.SetActive(true);
            I.OnResized();
        }

        public async UniTask Hide()
        {
            IsVisible = false;
            I.PanelCollectionViewerInstance.GetElement(EquipmentId)?.Selected(false);
            EquipmentId = Guid.Empty;
            _GameObject.SetActive(false);
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


            float panelTabHeight = Height - h1 - tabButtonH - (tabButtonS * 2);
            _PanelTab1_RectTransform.sizeDelta = new Vector2(Width, panelTabHeight);

            // Stats
            _PanelStat_RectTransform.anchoredPosition = new Vector2(imageContainerSpacing, imageContainerSpacing);
            _PanelStat_RectTransform.sizeDelta = new Vector2(I.PanelSelectedHeroInstance.PanelStatWidth, I.PanelSelectedHeroInstance.PanelStatHeight);
        }

        private async UniTask ShowHeroOnClick()
        {
            if (_DtoEquipment.HeroId != null)
            {
                I.PanelSelectedHeroInstance.Show(_DtoEquipment.HeroId.Value);
            }
        }
        private async UniTask TakeOnOffOnClick()
        {
            await EquipmentTakeOnOff();
        }

        private async UniTask TakeOnOffInAltSlotOnClick()
        {
            await EquipmentTakeOnOff(true);
        }

        private async UniTask EquipmentTakeOnOff(bool? inAltSlot = null)
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
