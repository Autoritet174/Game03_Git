using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection.prefabs;
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
    public class PanelIconCollectionElement
    {
        private const float TEXT_COLLECTION_ELEMENT_FONTSIZE = 14f;
        public PanelIconCollectionElement(
            PanelGroupDivider__prefab__script panelGroupDivider,
            CollectionElement collectionElement,
            PanelCollection__prefab__scriptMB panelCollection
            )
        {
            Id = collectionElement.Id;
            _PanelGroupDivider = panelGroupDivider;
            _CollectionElement = collectionElement;
            _PanelCollection = panelCollection;

            _GameObject = AddressableCache.IconCollectionElementAddressableGameObject.SafeInstant();
            _GameObject.name = $"IconCollectionElement [{Id}]";
            _GameObject.transform.SetParent(panelGroupDivider.CellsContainer__Transform);
            _RarityImage_GameObject = GameObjectFinder.FindByName("ImageMaskRarity", _GameObject.transform);
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _RectTransform.anchoredPosition3D = Vector3.zero;
            _RectTransform.localScale = Vector3.one;

            Transform childImageMaskCollectionElement = _GameObject.transform.Find("ImageMaskCollectionElement");
            Transform childImageMaskRarity = _GameObject.transform.Find("ImageMaskRarity");
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

            _Rarity_Image = imageRarity;
            Transform childText = _GameObject.transform.Find("TextCollectionElement");
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

            _TextMeshPro = textMeshPro;

            textMeshPro.text = _CollectionElement.Name;//.ToUpper1Char();
            textMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE;
            imageRarity.sprite = AddressableCache.GetRarity(_CollectionElement.Rarity);
            imageRarity.preserveAspect = true;
            imageRarity.type = Image.Type.Simple; // Режим без растягивания;

            imageCollectionElement.sprite = _PanelCollection.CollectionMode switch
            {
                ECollectionMode.Hero => AddressableCache.Heroes[$"{_CollectionElement.Name}_face"],
                ECollectionMode.Equipment => AddressableCache.Equipments[_CollectionElement.Name],
                _ => throw new NotImplementedException(),
            };
            imageCollectionElement.preserveAspect = true;
            imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;

            _GameObject.SetClickEvent(OnClick);
            EventHelper.SetHoverEvents(_GameObject, OnPointerEnter, OnPointerExit);

            _OwnerHeroIcon_GameObject = GameObjectFinder.FindByName("OwnerHeroIcon", _GameObject.transform);
            _OwnerImageRarity_Image = GameObjectFinder.FindByName<Image>("OwnerImageRarity", _GameObject.transform);
            _OwnerImageHero_Image = GameObjectFinder.FindByName<Image>("OwnerImageHero", _GameObject.transform);

            _SelectedImage_GameObject = GameObjectFinder.FindByName("ImageSelected", _GameObject.transform);

            _Equipment = _CollectionElement.TypeCollectionElement == TypeCollectionElement.Equipment
                ? CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.Id == _CollectionElement.Id) : null;

            RefreshOwnerImage();
            _PanelCollection.AddElement(this);
        }

        public Guid Id { get; private set; }
        private readonly PanelGroupDivider__prefab__script _PanelGroupDivider;
        private readonly PanelCollection__prefab__scriptMB _PanelCollection;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly CollectionElement _CollectionElement;
        private readonly TextMeshProUGUI _TextMeshPro;
        private readonly Image _Rarity_Image;

        private readonly GameObject _OwnerHeroIcon_GameObject;
        private readonly Image _OwnerImageRarity_Image;
        private readonly Image _OwnerImageHero_Image;
        private readonly Equipment _Equipment;
        private readonly GameObject _SelectedImage_GameObject;
        private readonly GameObject _RarityImage_GameObject;

        private readonly PanelSelectedHero__prefab__scriptMB PanelSelectedHero;
        private readonly PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipment;
        public void SetText(string text)
        {
            _TextMeshPro.SetText(text);
        }

        public void RefreshOwnerImage()
        {
            if (_Equipment != null)
            {
                if (_Equipment.HeroId != null)
                {
                    Hero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.Id == _Equipment.HeroId);
                    _OwnerImageHero_Image.sprite = AddressableCache.GetHeroFaceSprite(hero);
                    _OwnerImageRarity_Image.sprite = AddressableCache.GetRarity(hero.BaseHero.Rarity);
                    _OwnerHeroIcon_GameObject.SetActive(true);
                }
                else
                {
                    _OwnerHeroIcon_GameObject.SetActive(false);
                }
            }
        }

        public void OnResized()
        {
            _TextMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE * G.GetCoefHeight();
        }

        public void Selected(bool selected, bool clearOthers = true)
        {
            if (selected && clearOthers)
            {
                _PanelCollection.UnselectAll();
            }
            _SelectedImage_GameObject.SetActive(selected);
            //_RarityImage_GameObject.SetActive(!selected);
        }

        private void OnClick()
        {
            _PanelCollection.PanelCollectionViewerContext.OnElementSelected(_CollectionElement.Id, _PanelCollection.CollectionMode);
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
            _Rarity_Image.sprite = AddressableCache.RaritySelected;
        }

        private void OnPointerExit()
        {
            _Rarity_Image.sprite = AddressableCache.GetRarity(_CollectionElement.Rarity);
        }
    }
}
