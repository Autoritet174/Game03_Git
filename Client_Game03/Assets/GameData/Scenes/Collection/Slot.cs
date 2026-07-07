using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Collection;
using General;
using General.DTO.Entities.Collection;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using I = CollectionSceneInitializator;
using Assets.GameData.Scenes.Collection.Prefabs;

namespace Assets.GameData.Scenes.Collection
{
    public class Slot
    {
        private const float PANELSLOT_WIDTH = 95f;
        private const float PANELSLOT_HEIGHT = 112f;
        private const float PANELSLOT_LEFT = 10f;
        private const float PANELSLOT_TOP = 10f;
        public const float PANELSLOT_SPACING = 10f;
        private const float PANELSLOTLABEL_FONTSIZE = 13f;

        public string Name { get; private set; }
        private readonly int posX;
        private readonly int posY;
        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;
        private readonly RectTransform _ImageBackground_RectTransform;
        private readonly RectTransform _LabelSlot_RectTransform;
        private readonly TextMeshProUGUI _TextMeshProUGUI;
        private readonly GameObject _ImageContainer_GameObject;
        private readonly Image _Rarity_Image;
        private readonly Image _EquipmentFull_Image;
        private Equipment _Equipment;
        private readonly PanelSelectedEquipment__prefab__scriptMB _PanelSelectedEquipment;

        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Left { get; private set; }
        public float Top { get; private set; }
        public ESlot SlotId { get; private set; }

        public Slot(string name, int posX, int posY, Transform parent,
            PanelSelectedEquipment__prefab__scriptMB _PanelSelectedEquipment, ESlot slotId, string suffix = "")
        {
            Name = name;
            this.posX = posX;
            this.posY = posY;
            SlotId = slotId;
            this._PanelSelectedEquipment = _PanelSelectedEquipment;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>($"PanelSlot{name}{suffix}", parent);
            _GameObject = _RectTransform.gameObject;
            _ImageBackground_RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageBackground", _RectTransform);
            _LabelSlot_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelSlot", _RectTransform);

            _TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelSlot", _RectTransform);
            string lKey = L.UI.Label.Slot.GetKey(name);
            string text = Game03Client.LocalizationManager.GetValue(lKey);
            if (suffix != "")
            {
                text += $" {suffix}";
            }
            _TextMeshProUGUI.text = text;

            _ImageContainer_GameObject = GameObjectFinder.FindByName("ImageContainer", _RectTransform.transform);
            _Rarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", _RectTransform.transform);
            _EquipmentFull_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull", _RectTransform.transform);

            _GameObject.SetHoverEvents(OnPointerEnter, OnPointerExit);
            _GameObject.SetClickOnGameObject(OnClick);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();

            Left = (((PANELSLOT_WIDTH + PANELSLOT_SPACING) * (posX - 1)) + PANELSLOT_LEFT) * coefHeight;
            Top = (((PANELSLOT_HEIGHT + PANELSLOT_SPACING) * (posY - 1)) + PANELSLOT_TOP) * coefHeight;
            _RectTransform.anchoredPosition = new Vector2(Left, -Top);
            Width = PANELSLOT_WIDTH * coefHeight;
            Height = PANELSLOT_HEIGHT * coefHeight;
            _RectTransform.sizeDelta = new Vector2(Width, Height);
            _TextMeshProUGUI.fontSize = PANELSLOTLABEL_FONTSIZE * coefHeight;

            _ImageBackground_RectTransform.anchoredPosition = new Vector2(0f, 0f);
            _ImageBackground_RectTransform.sizeDelta = new Vector2(Width, Width);

            _LabelSlot_RectTransform.anchoredPosition = new Vector2(0f, -Width);
            _LabelSlot_RectTransform.sizeDelta = new Vector2(Width, (PANELSLOT_HEIGHT - PANELSLOT_WIDTH) * coefHeight);
        }

        public void EquipmentTakeOn(Guid equipmentId)
        {
            bool active = false;
            try
            {
                _Equipment = CollectionProvider.GetCollectionEquipmentsFromCache().FirstOrDefault(a => a.Id == equipmentId);
                if (_Equipment == null)
                {
                    return;
                }

                int rarity = _Equipment.BaseEquipment.Rarity;
                _Rarity_Image.sprite = AddressableCache.GetRarity(rarity);
                _EquipmentFull_Image.sprite = AddressableCache.Equipments[_Equipment.BaseEquipment.Name];
                active = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _ImageContainer_GameObject.SetActive(active);
            }
        }

        public void EquipmentTakeOff()
        {
            _Equipment = null;
            _ImageContainer_GameObject.SetActive(false);
        }

        private async UniTask OnPointerEnter()
        {
            _Rarity_Image.sprite = AddressableCache.RaritySelected;
            await UniTask.Yield();
        }

        private async UniTask OnPointerExit()
        {
            if (_Equipment != null)
            {
                _Rarity_Image.sprite = AddressableCache.GetRarity(_Equipment.BaseEquipment.Rarity);
            }
            await UniTask.Yield();
        }

        private async UniTask OnClick()
        {
            if (_Equipment != null)
            {
                _PanelSelectedEquipment.Show(_Equipment.Id);
            }
        }
    }

}
