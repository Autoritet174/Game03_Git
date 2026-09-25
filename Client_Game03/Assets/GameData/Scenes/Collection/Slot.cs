using Assets.GameData.Scenes.Collection.Prefabs;
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

namespace Assets.GameData.Scenes.Collection
{
    /// <summary>Отображает слот экипировки героя и обрабатывает выбор предмета.</summary>
    public class Slot
    {
        private const float PANELSLOT_WIDTH = 95f;
        private const float PANELSLOT_HEIGHT = 112f;
        private const float PANELSLOT_LEFT = 10f;
        private const float PANELSLOT_TOP = 10f;
        public const float PANELSLOT_SPACING = 10f;
        private const float PANELSLOTLABEL_FONTSIZE = 13f;

        public string name { get; private set; }

        private readonly int posX;
        private readonly int posY;
        private readonly RectTransform rectTransform;
        private readonly GameObject gameObject;
        private readonly RectTransform imageBackground_RectTransform;
        private readonly RectTransform labelSlot_RectTransform;
        private readonly TextMeshProUGUI textMeshProUGUI;
        private readonly GameObject imageContainer_GameObject;
        private readonly Image rarity_Image;
        private readonly Image equipmentFull_Image;
        private Equipment equipment;
        private readonly PanelSelectedEquipment__prefab__scriptMB panelSelectedEquipment;

        public float width { get; private set; }

        public float height { get; private set; }

        public float left { get; private set; }

        public float top { get; private set; }

        public ESlot slotId { get; private set; }

        public Slot(string name, int posX, int posY, Transform parent,
            PanelSelectedEquipment__prefab__scriptMB panelSelectedEquipment, ESlot slotId, string suffix = "")
        {
            this.name = name;
            this.posX = posX;
            this.posY = posY;
            this.slotId = slotId;
            this.panelSelectedEquipment = panelSelectedEquipment;

            rectTransform = GameObjectFinder.FindByName<RectTransform>($"PanelSlot{name}{suffix}", parent);
            gameObject = rectTransform.gameObject;
            imageBackground_RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageBackground", rectTransform);
            labelSlot_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelSlot", rectTransform);

            textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelSlot", rectTransform);
            string lKey = L.UI.Label.Slot.GetKey(name);
            string text = Game03Client.LocalizationManager.GetValue(lKey);
            if (suffix != "")
            {
                text += $" {suffix}";
            }
            textMeshProUGUI.text = text;

            imageContainer_GameObject = GameObjectFinder.FindByName("ImageContainer", rectTransform.transform);
            rarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", rectTransform.transform);
            equipmentFull_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull", rectTransform.transform);

            gameObject.SetHoverEvents(OnPointerEnter, OnPointerExit);
            gameObject.SetClickOnGameObject(OnClick);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();

            left = (((PANELSLOT_WIDTH + PANELSLOT_SPACING) * (posX - 1)) + PANELSLOT_LEFT) * coefHeight;
            top = (((PANELSLOT_HEIGHT + PANELSLOT_SPACING) * (posY - 1)) + PANELSLOT_TOP) * coefHeight;
            rectTransform.anchoredPosition = new(left, -top);
            width = PANELSLOT_WIDTH * coefHeight;
            height = PANELSLOT_HEIGHT * coefHeight;
            rectTransform.sizeDelta = new(width, height);
            textMeshProUGUI.fontSize = PANELSLOTLABEL_FONTSIZE * coefHeight;

            imageBackground_RectTransform.anchoredPosition = new(0f, 0f);
            imageBackground_RectTransform.sizeDelta = new(width, width);

            labelSlot_RectTransform.anchoredPosition = new(0f, -width);
            labelSlot_RectTransform.sizeDelta = new(width, (PANELSLOT_HEIGHT - PANELSLOT_WIDTH) * coefHeight);
        }

        public void EquipmentTakeOn(Guid equipmentId)
        {
            bool active = false;
            try
            {
                equipment = CollectionProvider.GetCollectionEquipmentsFromCache().FirstOrDefault(a => a.id == equipmentId);
                if (equipment == null)
                {
                    return;
                }

                int rarity = equipment.baseEquipment.rarity;
                rarity_Image.sprite = AddressablePrefabProvider.GetRarity(rarity);
                equipmentFull_Image.sprite = AddressablePrefabProvider.equipments[equipment.baseEquipment.name];
                active = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                imageContainer_GameObject.SetActive(active);
            }
        }

        public void EquipmentTakeOff()
        {
            equipment = null;
            imageContainer_GameObject.SetActive(false);
        }

        private async UniTask OnPointerEnter()
        {
            rarity_Image.sprite = AddressablePrefabProvider.raritySelected;
            await UniTask.Yield();
        }

        private async UniTask OnPointerExit()
        {
            if (equipment != null)
            {
                rarity_Image.sprite = AddressablePrefabProvider.GetRarity(equipment.baseEquipment.rarity);
            }
            await UniTask.Yield();
        }

        private async UniTask OnClick()
        {
            if (equipment != null)
            {
                panelSelectedEquipment.Show(equipment.id);
            }
        }
    }

}
