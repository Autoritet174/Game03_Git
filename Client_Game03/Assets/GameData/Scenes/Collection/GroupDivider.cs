using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
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

namespace Assets.GameData.Scenes.Collection
{
    /// <summary>
    /// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
    /// с асинхронной анимацией высоты.
    /// </summary>
    public class GroupDivider
    {
        public GroupDivider(PanelCollectionViewer panelCollectionViewer, string groupName) {
            _PanelCollectionViewer = panelCollectionViewer;
            _GroupName = groupName;
        }

        private readonly PanelCollectionViewer _PanelCollectionViewer;
        private readonly string _GroupName;

        private GameObject _GameObject;
        private RectTransform _RectTransform;

        private GameObject _DividerButton_GameObject;
        private RectTransform _DividerButton_RectTransform;
        /// <summary>
        /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
        /// </summary>
        private Button _DividerButton_Button;

        /// <summary>
        /// Контейнер, содержащий все ячейки инвентаря для этой группы.
        /// На этом объекте должен быть RectTransform.
        /// </summary>
        private GameObject _CellsContainer_GameObject;
        private RectTransform _CellsContainer_RectTransform;
        private GridLayoutGroup _CellsContainer_GridLayoutGroup;

        private TextMeshProUGUI DividerButton_TextMeshProUGUI;

        private IEnumerable<CollectionElement> _listCollectionElement;

        /// <summary>
        /// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
        /// </summary>
        private bool _Destroying = false;

        /// <summary>
        /// Текущее состояние группы (true - развернута, false - свернута).
        /// </summary>
        private bool _Expanded = true;

        
        public readonly List<DataCollectionElement> ListDataCollectionElement = new();

        /// <summary>
        /// Переключает состояние группы и запускает анимацию.
        /// </summary>
        public void ToggleGroup()
        {
            //Debug.Log(1);
            _Expanded = !_Expanded;

            if (_Expanded)
            {
                //    // Разворачивание
                //    // Сначала активируем контейнер, чтобы он участвовал в макете, но с высотой 0
                _CellsContainer_GameObject.SetActive(true);
                _DividerButton_Button.image.sprite = AddressableCache.UI_button_with_arrow_v2;
                //_CellsContainer_RectTransform.sizeDelta = new Vector2();
                //    //await AnimateHeightAsync(0, expandedHeight, token);
            }
            else
            {
                //    // Сворачивание
                //    //await AnimateHeightAsync(expandedHeight, 0, token);
                //    // После завершения анимации деактивируем контейнер
                _CellsContainer_GameObject.SetActive(false);
                _DividerButton_Button.image.sprite = AddressableCache.UI_button_with_arrow_v2_reverse;
            }
            Resize();
            //await UniTask.Delay(1); // Заглушка для асинхронности
            //UpdateDividerVisual(isExpanded);
        }

        public async UniTask Init(string group_name, Initializator init_Collection, GameObject gameObjectInput, IEnumerable<CollectionElement> listCollectionElement)
        {
            _GameObject = gameObjectInput;
            _RectTransform = gameObjectInput.GetComponent<RectTransform>();
            _DividerButton_GameObject = GameObjectFinder.FindByName("DividerButton", _GameObject.transform);
            _DividerButton_RectTransform = _DividerButton_GameObject.GetComponent<RectTransform>();
            _CellsContainer_GameObject = GameObjectFinder.FindByName("CellsContainer", _GameObject.transform);
            _CellsContainer_RectTransform = _CellsContainer_GameObject.GetComponent<RectTransform>();
            _CellsContainer_GridLayoutGroup = _CellsContainer_GameObject.GetComponent<GridLayoutGroup>();
            _DividerButton_Button = _DividerButton_GameObject.GetComponent<Button>();
            _listCollectionElement = listCollectionElement;

            _Init_Collection.OnResizeWindow();

            //DividerButton
            DividerButton_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _DividerButton_GameObject.transform);
            Transform cellsContainer_Transform = _CellsContainer_GameObject.transform;
            if (string.IsNullOrWhiteSpace(group_name))
            {
                DividerButton_TextMeshProUGUI.text = "---No Group---";
                DividerButton_TextMeshProUGUI.fontStyle = FontStyles.Italic;
            }
            else
            {
                DividerButton_TextMeshProUGUI.text = group_name;
            }


            ListDataCollectionElement.Clear();
            foreach (CollectionElement collectionElement in _listCollectionElement)
            {
                GameObject _prefabIconCollectionElement = AddressableCache.IconCollectionElementAddressableGameObject.SafeInstant();
                _prefabIconCollectionElement.transform.SetParent(cellsContainer_Transform);

                RectTransform _prefabIconCollectionElement_Transform = _prefabIconCollectionElement.GetComponent<RectTransform>();
                _prefabIconCollectionElement_Transform.anchoredPosition3D = Vector3.zero;
                _prefabIconCollectionElement_Transform.localScale = Vector3.one;



                Transform childImageMaskCollectionElement = _prefabIconCollectionElement.transform.Find("ImageMaskCollectionElement");
                Transform childImageMaskRarity = _prefabIconCollectionElement.transform.Find("ImageMaskRarity");
                Transform childImageCollectionElement = childImageMaskCollectionElement.Find("ImageCollectionElement");
                Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");
                if (childImageCollectionElement != null && childImageCollectionElement.TryGetComponent(out Image imageCollectionElement)
                    && childImageRarity != null && childImageRarity.TryGetComponent(out Image imageRarity))
                {
                    Transform childText = _prefabIconCollectionElement.transform.Find("TextCollectionElement");
                    if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
                    {
                        DataCollectionElement dataCollectionElement = new()
                        {
                            gameObject = _prefabIconCollectionElement,
                            collectionElement = collectionElement,
                            textMeshPro = textMeshPro,
                            imageRarity = imageRarity,
                            rectTransform = _prefabIconCollectionElement_Transform
                        };

                        ListDataCollectionElement.Add(dataCollectionElement);
                        textMeshPro.text = collectionElement.Name;//.ToUpper1Char();
                        textMeshPro.fontSize = 14;
                        imageRarity.sprite = AddressableCache.Rarityes[collectionElement.Rarity];
                        imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
                        imageRarity.type = Image.Type.Simple; // Режим без растягивания;

                        string tagUnique = collectionElement.IsUnique ? "Unique-" : string.Empty;
                        imageCollectionElement.sprite = _Init_Collection.CollectionMode switch
                        {
                            1 => AddressableCache.Heroes[$"{collectionElement.Name}_face"],
                            2 => AddressableCache.Equipments[$"{tagUnique}{collectionElement.Name}_128"],
                            3 => _Init_Collection.PanelSelectedHeroIsActive
                                    ? AddressableCache.Equipments[$"{tagUnique}{collectionElement.Name}_128"]
                                    : AddressableCache.Heroes[$"{collectionElement.Name}_face"],
                            _ => throw new Exception("CollectionMode > 2"),
                        };
                        imageCollectionElement.preserveAspect = true; // Сохраняет пропорции изображения
                        imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;

                        async UniTask OnClick()
                        {
                            _Init_Collection.UnselectAll();
                            dataCollectionElement.Selected = true;
                            dataCollectionElement.rectTransform.localScale = Initializator.Vector3Selected;

                            async UniTask ShowHero()
                            {
                                if (collectionElement.TypeCollectionElement != TypeCollectionElement.Hero)
                                {
                                    await UniTask.Yield();
                                    throw new Exception();
                                }
                                _Init_Collection.PanelSelectedHeroSetActive(true, false);
                                //string name = collectionElement.Name;
                                _Init_Collection.SelectedHeroTop_TextMeshProUGUI.text = collectionElement.Name;//name.ToUpper1Char();
                                _Init_Collection.SelectedHero_Image.sprite = AddressableCache.Heroes[collectionElement.Name];
                                _Init_Collection.SelectedHero_Image.preserveAspect = true; // Сохраняет пропорции изображения

                                _Init_Collection.SelectedHeroRarity_Image.sprite = AddressableCache.Rarityes[collectionElement.Rarity];
                                _Init_Collection.SelectedHeroId = collectionElement.Id;
                            }

                            async UniTask ShowEquipment()
                            {
                                _Init_Collection.PanelSelectedEquipmentSetActive(true, false);
                                string name = collectionElement.Name;
                                _Init_Collection.SelectedEquipmentTop_TextMeshProUGUI.text = name.ToUpper1Char();

                                string tagUnique = collectionElement.IsUnique ? "Unique-" : string.Empty;
                                _Init_Collection.SelectedEquipment_Image.sprite = AddressableCache.Equipments[$"{tagUnique}{name}"];

                                _Init_Collection.SelectedEquipment_Image.preserveAspect = true; // Сохраняет пропорции изображения
                                _Init_Collection.SelectedEquipmentRarity_Image.sprite = AddressableCache.Rarityes[collectionElement.Rarity];
                                _Init_Collection.SelectedEquipmentId = collectionElement.Id;

                                _Init_Collection.ButtonTakeOnOff_RectTransform.gameObject.SetClickEvent(async () =>
                                {
                                    if (collectionElement.TypeCollectionElement != TypeCollectionElement.Equipment)
                                    {
                                        await UniTask.Yield();
                                        throw new Exception();
                                    }

                                    IEnumerable<DtoEquipment> equipments = CollectionProvider.GetCollectionEquipmentsFromCache();
                                    DtoEquipment equipment = equipments.FirstOrDefault(a => a.Id == collectionElement.Id);
                                    if (equipment == null || _Init_Collection.SelectedHeroId == Guid.Empty)
                                    {
                                        return;
                                    }

                                    DtoHero hero = CollectionProvider.GetCollectionHeroesFromCache().FirstOrDefault(a => a.Id == _Init_Collection.SelectedHeroId);
                                    if (hero == null)
                                    {
                                        return;
                                    }

                                    if (equipment.HeroId != null && equipment.SlotId != null)
                                    {
                                        _Init_Collection.ButtonTakeOnOff_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.TakeOff);
                                    }
                                    else if (equipment.HeroId == null && equipment.SlotId == null)
                                    {
                                        // Предмет ни на кого не одет
                                        _Init_Collection.ButtonTakeOnOff_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.TakeOn);
                                        int slotTypeId = equipment.BaseEquipment.EquipmentType.SlotTypeId;
                                        switch (slotTypeId)
                                        {
                                            case 1://Оружие
                                                break;
                                            case 14://Кольцо
                                                break;
                                            case 16://Аксессуар
                                                break;
                                            default:
                                                {
                                                    int slotId = Game03Client.GameData.Container.Slots.First(a => a.SlotTypeId == slotTypeId).Id;
                                                    DtoEquipment equipmentOnHero = equipments.FirstOrDefault(a => a.SlotId == slotId && a.HeroId == hero.Id);
                                                    if (equipmentOnHero != null)
                                                    {
                                                        // слот занят, через вебсокет снимаем

                                                    }
                                                    else
                                                    {


                                                    }
                                                    // надеваем экипировку на героя
                                                    // через вебсокет команда на сервер, на сервере такая же проверка так как не верим клиенту
                                                    // ждем ответ от сервера с токеном на 3 секунды
                                                    // по ответу ориентируемся одели шмотку или нет
                                                    break;
                                                }
                                        }


                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }


                                    string slotName = equipment.BaseEquipment.EquipmentType.SlotType.Name;
                                    if (Initializator.Slots1by1.Any(a => string.Compare(slotName, a, StringComparison.InvariantCultureIgnoreCase) == 0))
                                    {

                                    }

                                }, true);
                                await UniTask.Yield();
                            }


                            switch (_Init_Collection.CollectionMode)
                            {
                                case 1:
                                    await ShowHero(); break;
                                case 2:
                                    await ShowEquipment(); break;
                                case 3:
                                    switch (collectionElement.TypeCollectionElement)
                                    {
                                        case TypeCollectionElement.Hero:
                                            _Init_Collection.PanelSelectedHeroSetActive(true, true);
                                            await ShowHero();
                                            await _Init_Collection.InstantiateCollectionAsync();
                                            break;
                                        case TypeCollectionElement.Equipment:
                                            if (!_Init_Collection.PanelSelectedEquipmentIsActive)
                                            {
                                                _Init_Collection.PanelSelectedEquipmentSetActive(true, true);
                                            }
                                            await ShowEquipment();

                                            break;
                                        default:
                                            throw new Exception();
                                    }
                                    break;
                                default:
                                    throw new Exception();
                            }
                            await UniTask.Delay(1); // Заглушка для асинхронности
                        }
                        async UniTask OnPointerEnter()
                        {
                            imageRarity.sprite = AddressableCache.Rarityes[0];
                            await UniTask.Yield();
                        }
                        async UniTask OnPointerExit()
                        {
                            imageRarity.sprite = AddressableCache.Rarityes[collectionElement.Rarity];
                            await UniTask.Yield();
                        }
                        EventHelper.SetClickEvent(_prefabIconCollectionElement, OnClick, false);
                        EventHelper.AddHoverEvents(_prefabIconCollectionElement, OnPointerEnter, OnPointerExit);

                    }
                }


            }

            // Привязываем метод ToggleGroup к событию клика
            _DividerButton_Button.onClick.RemoveAllListeners();
            _DividerButton_Button.onClick.AddListener(ToggleGroup);

            // Если группа должна быть свернута по умолчанию, устанавливаем высоту в 0,
            // иначе сохраняем текущую высоту.
            if (!_Expanded)
            {
                // Установка начальной высоты в 0, но нужно сохранить полную высоту
                // Для корректного расчета полной высоты, сначала активируем объект
                //gameObjectCellsContainer.SetActive(true);
                // Принудительная перестройка макета для получения корректной высоты
                //LayoutRebuilder.ForceRebuildLayoutImmediate(_CellsContainer_RectTransform);
                //expandedHeight = _CellsContainer_RectTransform.rect.height;
                //_CellsContainer_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                //gameObjectCellsContainer.SetActive(false);
            }
            else
            {
                // Принудительная перестройка макета для получения корректной высоты
                //LayoutRebuilder.ForceRebuildLayoutImmediate(_CellsContainer_RectTransform);
                //expandedHeight = _CellsContainer_RectTransform.rect.height;
            }
            await UniTask.Delay(1); // Заглушка для асинхронности
            //_ = Task.Run(AnimateHeight);
            //UpdateDividerVisual(Expanded);
        }

        public void Resize()
        {
            float width = _Init_Collection.PanelCollection_RectTransform.sizeDelta.x;
            float coefHeight = G.GetCoefHeight();
            float heightButton = 45f * coefHeight;
            float height = heightButton;

            int scrollbarWidth = Mathf.FloorToInt(32f * coefHeight);

            float widthWithoutVertBar = width - scrollbarWidth;
            _DividerButton_RectTransform.sizeDelta = new Vector2(widthWithoutVertBar, heightButton);

            if (DividerButton_TextMeshProUGUI != null)
            {
                DividerButton_TextMeshProUGUI.fontSize = 24 * coefHeight;
            }

            if (_Expanded)
            {
                const float cellSize1080 = 140f;
                const float spacing1080 = 9f;
                const float padding1080 = 22.5f;
                //int paddingR = (int)(40f * coefHeight);
                float spacing = spacing1080 * coefHeight;
                float cellSize = cellSize1080 * coefHeight;
                int padding = (int)(padding1080 * coefHeight);
                //расчитываем сколько при этих параметрах войдет ячеек
                float widthWithoutPadding = widthWithoutVertBar - (padding * 2);
                int countCellInRow = (int)(widthWithoutPadding / cellSize);
                if (countCellInRow <= 0)
                {
                    countCellInRow = 1;
                }

                float needWidth = (countCellInRow * cellSize) + ((countCellInRow - 1) * spacing);
                float coefWidth = widthWithoutPadding / needWidth;
                spacing = ((int)(spacing * coefWidth * 10f)) / 10f;
                cellSize = ((int)(cellSize * coefWidth * 10f)) / 10f;

                _CellsContainer_GridLayoutGroup.padding.left = padding;
                _CellsContainer_GridLayoutGroup.padding.right = padding;
                _CellsContainer_GridLayoutGroup.padding.top = padding;
                _CellsContainer_GridLayoutGroup.padding.bottom = padding;
                _CellsContainer_GridLayoutGroup.spacing = new Vector2(spacing, spacing);
                _CellsContainer_GridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);


                // вычисляем количество строк
                int countCollectionElement = _listCollectionElement.Count();
                int countRows = (countCollectionElement / countCellInRow) + (countCollectionElement % countCellInRow == 0 ? 0 : 1);
                if (countRows < 1)
                {
                    countRows = 1;
                }

                float heightContainer = (countRows * cellSize) + ((countRows - 1) * spacing) + (padding * 4);// по сути нужно 2 но чтобы сделать низ длиннее поставил 4
                _CellsContainer_RectTransform.sizeDelta = new Vector2(widthWithoutVertBar, heightContainer);
                height += heightContainer;

                _CellsContainer_RectTransform.anchoredPosition = new Vector2(0, -45f * coefHeight);

                foreach (DataCollectionElement item in ListDataCollectionElement)
                {
                    item.textMeshPro.fontSize = 14f * coefHeight;
                }
            }
            _RectTransform.sizeDelta = new Vector2(widthWithoutVertBar, height);

        }

        /// <summary>
        /// Вызывается при уничтожении объекта для отмены всех активных задач.
        /// </summary>
        private void OnDestroy()
        {
            if (!_Destroying)
            {
                // позже удалить
            }
            _Destroying = true;
            //cancellationTokenSource?.Cancel();
            //cancellationTokenSource?.Dispose();
        }

    }
}
