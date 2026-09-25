using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection.Prefabs;
using Assets.GameData.Scripts;
using Game03Client.Collection;
using General.DTO.Entities.Collection;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Collection
{
    /// <summary>Отображает элемент коллекции, его владельца и состояние выбора.</summary>
    public class PanelIconCollectionElement
    {
        private const float TEXT_COLLECTION_ELEMENT_FONTSIZE = 14f;
        public PanelIconCollectionElement(
            PanelGroupDivider__prefab__script panelGroupDivider,
            CollectionElement collectionElement,
            PanelCollection__prefab__scriptMB panelCollection
            )
        {
            id = collectionElement.Id;
            this.panelGroupDivider = panelGroupDivider;
            this.collectionElement = collectionElement;
            this.panelCollection = panelCollection;

            gameObject = AddressablePrefabProvider.iconCollectionElementAddressableGameObject.SafeInstant();
            gameObject.name = $"IconCollectionElement [{id}]";
            gameObject.transform.SetParent(panelGroupDivider.cellsContainer__Transform);
            rarityImage_GameObject = GameObjectFinder.FindByName("ImageMaskRarity", gameObject.transform);
            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            Transform childImageMaskCollectionElement = gameObject.transform.Find("ImageMaskCollectionElement");
            Transform childImageMaskRarity = gameObject.transform.Find("ImageMaskRarity");
            Transform childImageCollectionElement = childImageMaskCollectionElement.Find("ImageCollectionElement");
            if (childImageCollectionElement == null)
            {
                Debug.LogError($"childImageCollectionElement = null");
                return;
            }
            if (!childImageCollectionElement.TryGetComponent(out Image imageCollectionElement))
            {
                Debug.LogError($"imageCollectionElement = null");
                return;
            }

            Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");
            if (childImageRarity == null)
            {
                Debug.LogError($"childImageRarity = null");
                return;
            }
            if (!childImageRarity.TryGetComponent(out Image imageRarity))
            {
                Debug.LogError($"imageRarity = null");
                return;
            }

            rarity_Image = imageRarity;
            Transform childText = gameObject.transform.Find("TextCollectionElement");
            if (childText == null)
            {
                Debug.LogError($"childText = null");
                return;
            }

            if (!childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
            {
                Debug.LogError($"textMeshPro = null");
                return;
            }

            this.textMeshPro = textMeshPro;

            textMeshPro.text = this.collectionElement.Name;//.ToUpper1Char();
            textMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE;
            imageRarity.sprite = AddressablePrefabProvider.GetRarity(this.collectionElement.Rarity);
            imageRarity.preserveAspect = true;
            imageRarity.type = Image.Type.Simple; // Режим без растягивания;

            imageCollectionElement.sprite = this.panelCollection.collectionMode switch
            {
                ECollectionMode.hero => AddressablePrefabProvider.heroes[$"{this.collectionElement.Name}_face"],
                ECollectionMode.equipment => AddressablePrefabProvider.equipments[this.collectionElement.Name],
                _ => throw new NotImplementedException(),
            };
            imageCollectionElement.preserveAspect = true;
            imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;

            gameObject.SetClickOnButton(OnClick);
            EventHelper.SetHoverEvents(gameObject, OnPointerEnter, OnPointerExit);

            ownerHeroIcon_GameObject = GameObjectFinder.FindByName("OwnerHeroIcon", gameObject.transform);
            ownerImageRarity_Image = GameObjectFinder.FindByName<Image>("OwnerImageRarity", gameObject.transform);
            ownerImageHero_Image = GameObjectFinder.FindByName<Image>("OwnerImageHero", gameObject.transform);

            selectedImage_GameObject = GameObjectFinder.FindByName("ImageSelected", gameObject.transform);

            equipment = this.collectionElement.TypeCollectionElement == TypeCollectionElement.Equipment
                ? CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.id == this.collectionElement.Id) : null;

            RefreshOwnerImage();
            this.panelCollection.AddElement(this);
        }

        public Guid id { get; private set; }

        private readonly PanelGroupDivider__prefab__script panelGroupDivider;
        private readonly PanelCollection__prefab__scriptMB panelCollection;
        private readonly GameObject gameObject;
        private readonly RectTransform rectTransform;
        private readonly CollectionElement collectionElement;
        private readonly TextMeshProUGUI textMeshPro;
        private readonly Image rarity_Image;

        private readonly GameObject ownerHeroIcon_GameObject;
        private readonly Image ownerImageRarity_Image;
        private readonly Image ownerImageHero_Image;
        private readonly Equipment equipment;
        private readonly GameObject selectedImage_GameObject;
        private readonly GameObject rarityImage_GameObject;

        private readonly PanelSelectedHero__prefab__scriptMB panelSelectedHero;
        private readonly PanelSelectedEquipment__prefab__scriptMB panelSelectedEquipment;

        public bool selected { get; private set; }

        public void SetText(string text)
        {
            textMeshPro.SetText(text);
        }

        public void RefreshOwnerImage()
        {
            if (equipment != null)
            {
                if (equipment.heroId != null)
                {
                    Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.id == equipment.heroId);
                    ownerImageHero_Image.sprite = AddressablePrefabProvider.GetHeroFaceSprite(hero);
                    ownerImageRarity_Image.sprite = AddressablePrefabProvider.GetRarity(hero.baseHero.rarity);
                    ownerHeroIcon_GameObject.SetActive(true);
                }
                else
                {
                    ownerHeroIcon_GameObject.SetActive(false);
                }
            }
        }

        public void OnResized()
        {
            textMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE * G.GetCoefHeight();
        }

        public void SetSelected(bool selected, bool clearOthers = true)
        {
            this.selected = selected;
            if (selected && clearOthers)
            {
                panelCollection.UnselectAll();
            }
            selectedImage_GameObject.SetActive(selected);
            //_RarityImage_GameObject.SetActive(!selected);
        }

        private void OnClick()
        {
            panelCollection.panelCollectionContext.OnClick(collectionElement.Id, panelCollection.collectionMode);
            //switch (_PanelCollection.CollectionMode)
            //{
            //    case ECollectionMode.Hero:
            //        PanelSelectedHero.Show(_CollectionElement.Id);
            //        break;
            //    case ECollectionMode.Equipment:
            //        PanelSelectedEquipment.Show(_CollectionElement.Id);
            //        break;
            //    default:
            //        throw new NotImplementedException();
            //}
        }

        private void OnPointerEnter()
        {
            rarity_Image.sprite = AddressablePrefabProvider.raritySelected;
        }

        private void OnPointerExit()
        {
            rarity_Image.sprite = AddressablePrefabProvider.GetRarity(collectionElement.Rarity);
        }
    }
}
