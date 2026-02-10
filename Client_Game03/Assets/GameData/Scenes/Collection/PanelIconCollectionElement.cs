using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.Collection;
using System;
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
            _PanelGroupDivider = panelGroupDivider;
            _CollectionElement = collectionElement;
            _PanelScene = panelGroupDivider.PanelCollectionViewer.PanelCollection.PanelScene;
            _PanelCollectionViewer = panelGroupDivider.PanelCollectionViewer;
            _PanelSelectedHero = _PanelScene.PanelSelectedHero;
            _PanelSelectedEquipment = _PanelScene.PanelSelectedEquipment;

            _GameObject = AddressableCache.IconCollectionElementAddressableGameObject.SafeInstant();
            _GameObject.transform.SetParent(panelGroupDivider.CellsContainer_Transform);

            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _RectTransform.anchoredPosition3D = Vector3.zero;
            _RectTransform.localScale = Vector3.one;

            Transform childImageMaskCollectionElement = _GameObject.transform.Find("ImageMaskCollectionElement");
            Transform childImageMaskRarity = _GameObject.transform.Find("ImageMaskRarity");
            Transform childImageCollectionElement = childImageMaskCollectionElement.Find("ImageCollectionElement");
            Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");

            if (childImageCollectionElement != null && childImageCollectionElement.TryGetComponent(out Image imageCollectionElement)
                    && childImageRarity != null && childImageRarity.TryGetComponent(out Image imageRarity))
            {
                _ImageRarity = imageRarity;
                Transform childText = _GameObject.transform.Find("TextCollectionElement");
                if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
                {
                    _TextMeshPro = textMeshPro;

                    textMeshPro.text = _CollectionElement.Name;//.ToUpper1Char();
                    textMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE;
                    imageRarity.sprite = AddressableCache.Rarityes[_CollectionElement.Rarity];
                    imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
                    imageRarity.type = Image.Type.Simple; // Режим без растягивания;

                    string tagUnique = _CollectionElement.IsUnique ? "Unique-" : string.Empty;
                    imageCollectionElement.sprite = _PanelScene.CollectionMode switch
                    {
                        CollectionModeEnum.Hero => AddressableCache.Heroes[$"{_CollectionElement.Name}_face"],
                        CollectionModeEnum.Equipment => AddressableCache.Equipments[$"{tagUnique}{_CollectionElement.Name}_128"],
                        CollectionModeEnum.ChangingEquipment => _PanelScene.PanelSelectedHero.IsVisible
                            ? AddressableCache.Equipments[$"{tagUnique}{_CollectionElement.Name}_128"]
                            : AddressableCache.Heroes[$"{_CollectionElement.Name}_face"],
                        _ => throw new NotImplementedException(),
                    };
                    imageCollectionElement.preserveAspect = true; // Сохраняет пропорции изображения
                    imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;


                    EventHelper.SetClickEvent(_GameObject, OnClick, false);
                    EventHelper.AddHoverEvents(_GameObject, OnPointerEnter, OnPointerExit);

                }
            }
        }

        private readonly PanelGroupDivider _PanelGroupDivider;
        private readonly PanelScene _PanelScene;
        private readonly PanelCollectionViewer _PanelCollectionViewer;
        private readonly PanelSelectedHero _PanelSelectedHero;
        private readonly PanelSelectedEquipment _PanelSelectedEquipment;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly CollectionElement _CollectionElement;
        private readonly TextMeshProUGUI _TextMeshPro;
        private readonly Image _ImageRarity;

        public void SetText(string text)
        {
            _TextMeshPro.SetText(text);
        }

        public void OnResized()
        {
            _TextMeshPro.fontSize = TEXT_COLLECTION_ELEMENT_FONTSIZE * G.GetCoefHeight();
        }

        private async UniTask OnClick()
        {
            _PanelCollectionViewer.UnselectAll();

            //_RectTransform.localScale = Initializator.Vector3Selected;// ИСПРАВИТЬ

            switch (_PanelScene.CollectionMode)
            {
                case CollectionModeEnum.Hero:
                    _PanelSelectedEquipment.Hide();
                    _PanelSelectedHero.Show(_CollectionElement); break;

                case CollectionModeEnum.Equipment:
                    await _PanelSelectedHero.Hide();
                    _PanelSelectedEquipment.Show(_CollectionElement); break;

                case CollectionModeEnum.ChangingEquipment:
                    switch (_CollectionElement.TypeCollectionElement)
                    {
                        case TypeCollectionElement.Hero:
                            _PanelSelectedHero.Show(_CollectionElement);
                            await _PanelCollectionViewer.InstantiateCollectionAsync();
                            break;

                        case TypeCollectionElement.Equipment:
                            _PanelSelectedEquipment.Show(_CollectionElement);
                            if (!_PanelScene.PanelSelectedHero.IsVisible)
                            {
                                await _PanelCollectionViewer.InstantiateCollectionAsync();
                            }

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            _PanelScene.OnResized();
        }

        private async UniTask OnPointerEnter()
        {
            _ImageRarity.sprite = AddressableCache.Rarityes[0];
            await UniTask.Yield();
        }

        private async UniTask OnPointerExit()
        {
            _ImageRarity.sprite = AddressableCache.Rarityes[_CollectionElement.Rarity];
            await UniTask.Yield();
        }
    }
}
