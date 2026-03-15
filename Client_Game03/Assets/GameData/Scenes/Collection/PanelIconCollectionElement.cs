using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
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

        public PanelIconCollectionElement(PanelGroupDivider panelGroupDivider, CollectionElement collectionElement)
        {
            Id = collectionElement.Id;
            _PanelGroupDivider = panelGroupDivider;
            _CollectionElement = collectionElement;
            _PanelScene = panelGroupDivider.PanelCollectionViewer.PanelCollection.PanelScene;
            _PanelCollectionViewer = panelGroupDivider.PanelCollectionViewer;
            _PanelSelectedHero = _PanelScene.PanelSelectedHero;
            _PanelSelectedEquipment = _PanelScene.PanelSelectedEquipment;

            _GameObject = AddressableCache.IconCollectionElementAddressableGameObject.SafeInstant();
            _GameObject.name = $"IconCollectionElement [{Id}]";
            _GameObject.transform.SetParent(panelGroupDivider.CellsContainer_Transform);
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
            imageRarity.sprite = AddressableCache.Rarityes[_CollectionElement.Rarity];
            imageRarity.preserveAspect = true;
            imageRarity.type = Image.Type.Simple; // Режим без растягивания;

            imageCollectionElement.sprite = _PanelScene.CollectionMode switch
            {
                CollectionModeEnum.Hero => AddressableCache.Heroes[$"{_CollectionElement.Name}_face"],
                CollectionModeEnum.Equipment => AddressableCache.Equipments[_CollectionElement.Name],
                _ => throw new NotImplementedException(),
            };
            imageCollectionElement.preserveAspect = true;
            imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;

            EventHelper.SetClickEvent(_GameObject, OnClick, false);
            EventHelper.AddHoverEvents(_GameObject, OnPointerEnter, OnPointerExit);

            _OwnerHeroIcon_GameObject = GameObjectFinder.FindByName("OwnerHeroIcon", _GameObject.transform);
            _OwnerImageRarity_Image = GameObjectFinder.FindByName<Image>("OwnerImageRarity", _GameObject.transform);
            _OwnerImageHero_Image = GameObjectFinder.FindByName<Image>("OwnerImageHero", _GameObject.transform);

            _SelectedImage_GameObject = GameObjectFinder.FindByName("ImageSelected", _GameObject.transform);

            _Equipment = _CollectionElement.TypeCollectionElement == TypeCollectionElement.Equipment
                ? CollectionProvider.GetCollectionEquipmentsFromCache().First(a => a.Id == _CollectionElement.Id) : null;

            RefreshOwnerImage();
            _PanelCollectionViewer.AddElement(this);
        }

        public Guid Id { get; private set; }
        private readonly PanelGroupDivider _PanelGroupDivider;
        private readonly PanelScene _PanelScene;
        private readonly PanelCollectionViewer _PanelCollectionViewer;
        private readonly PanelSelectedHero _PanelSelectedHero;
        private readonly PanelSelectedEquipment _PanelSelectedEquipment;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly CollectionElement _CollectionElement;
        private readonly TextMeshProUGUI _TextMeshPro;
        private readonly Image _Rarity_Image;

        private readonly GameObject _OwnerHeroIcon_GameObject;
        private readonly Image _OwnerImageRarity_Image;
        private readonly Image _OwnerImageHero_Image;
        private readonly DtoEquipment _Equipment;
        private readonly GameObject _SelectedImage_GameObject;
        private readonly GameObject _RarityImage_GameObject;

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
                    DtoHero hero = CollectionProvider.GetCollectionHeroesFromCache().First(a => a.Id == _Equipment.HeroId);
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

        public void Selected(bool selected)
        {
            if (selected)
            {
                _PanelCollectionViewer.UnselectAll();
            }
            _SelectedImage_GameObject.SetActive(selected);
            //_RarityImage_GameObject.SetActive(!selected);
        }

        private async UniTask OnClick()
        {
            //_RectTransform.localScale = Initializator.Vector3Selected;// ИСПРАВИТЬ

            switch (_PanelScene.CollectionMode)
            {
                case CollectionModeEnum.Hero:
                    //_PanelSelectedEquipment.Hide();
                    _PanelSelectedHero.Show(_CollectionElement.Id); break;

                case CollectionModeEnum.Equipment:
                    //await _PanelSelectedHero.Hide();
                    _PanelSelectedEquipment.Show(_CollectionElement.Id); break;

                //case CollectionModeEnum.ChangingEquipment:
                //    switch (_CollectionElement.TypeCollectionElement)
                //    {
                //        case TypeCollectionElement.Hero:
                //            _PanelSelectedHero.Show(_CollectionElement);
                //            await _PanelCollectionViewer.InstantiateCollectionAsync();
                //            break;

                //        case TypeCollectionElement.Equipment:
                //            _PanelSelectedEquipment.Show(_CollectionElement);
                //            if (!_PanelScene.PanelSelectedHero.IsVisible)
                //            {
                //                await _PanelCollectionViewer.InstantiateCollectionAsync();
                //            }

                //            break;

                //        default:
                //            throw new NotImplementedException();
                //    }
                //    break;
                default:
                    throw new NotImplementedException();
            }

            Selected(true);
            _PanelScene.OnResized();
        }

        private async UniTask OnPointerEnter()
        {
            _Rarity_Image.sprite = AddressableCache.Rarityes[0];
            await UniTask.Yield();
        }

        private async UniTask OnPointerExit()
        {
            _Rarity_Image.sprite = AddressableCache.Rarityes[_CollectionElement.Rarity];
            await UniTask.Yield();
        }
    }
}
