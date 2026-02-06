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
    public class PanelGroupDivider
    {
        private const float DIVIDER_BUTTON_HEIGHT = 45f;
        private const float DIVIDER_BUTTON_FONTSIZE = 24f;
        private const float CELL_SIZE = 140f;
        private const float SPACING = 9f;
        private const float PADDING = 22.5f;
        private const float COLLECTION_ELEMENT_FONTSIZE = 14f;

        public PanelGroupDivider(PanelCollectionViewer panelCollectionViewer, GroupCollectionElement groupCollectionElement)
        {
            PanelCollectionViewer = panelCollectionViewer;
            _PanelScene = PanelCollectionViewer.PanelCollection.PanelScene;

            _GameObject = AddressableCache.GroupDividerPrefabAddressableGameObject.SafeInstant();
            _GameObject.transform.SetParent(panelCollectionViewer.CollectionContent_Transform, false);

            _GroupName = groupCollectionElement.Name;
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _DividerButton_GameObject = GameObjectFinder.FindByName("DividerButton", _GameObject.transform);
            _DividerButton_RectTransform = _DividerButton_GameObject.GetComponent<RectTransform>();
            _CellsContainer_GameObject = GameObjectFinder.FindByName("CellsContainer", _GameObject.transform);
            _CellsContainer_RectTransform = _CellsContainer_GameObject.GetComponent<RectTransform>();
            _CellsContainer_GridLayoutGroup = _CellsContainer_GameObject.GetComponent<GridLayoutGroup>();
            _DividerButton_Button = _DividerButton_GameObject.GetComponent<Button>();
            _CollectionElementList = groupCollectionElement.List;

            OnResized();

            //DividerButton
            _DividerButton_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _DividerButton_GameObject.transform);
            if (string.IsNullOrWhiteSpace(_GroupName))
            {
                _DividerButton_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Label.NoGroup);
                _DividerButton_TextMeshProUGUI.fontStyle = FontStyles.Italic;
            }
            else
            {
                _DividerButton_TextMeshProUGUI.text = _GroupName;
                _DividerButton_TextMeshProUGUI.fontStyle = FontStyles.Normal;
            }

            // Привязываем метод ToggleGroup к событию клика
            _DividerButton_Button.onClick.RemoveAllListeners();
            _DividerButton_Button.onClick.AddListener(ToggleGroup);

            CellsContainer_Transform = _CellsContainer_GameObject.transform;

            _CollectionElementDataList = new();
            foreach (CollectionElement collectionElement in _CollectionElementList)
            {
                _CollectionElementDataList.Add(new(this, collectionElement));
            }



            // Если группа должна быть свернута по умолчанию, устанавливаем высоту в 0,
            // иначе сохраняем текущую высоту.
            //if (!_Expanded)
            //{
            //    // Установка начальной высоты в 0, но нужно сохранить полную высоту
            //    // Для корректного расчета полной высоты, сначала активируем объект
            //    //gameObjectCellsContainer.SetActive(true);
            //    // Принудительная перестройка макета для получения корректной высоты
            //    //LayoutRebuilder.ForceRebuildLayoutImmediate(_CellsContainer_RectTransform);
            //    //expandedHeight = _CellsContainer_RectTransform.rect.height;
            //    //_CellsContainer_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            //    //gameObjectCellsContainer.SetActive(false);
            //}
            //else
            //{
            //    // Принудительная перестройка макета для получения корректной высоты
            //    //LayoutRebuilder.ForceRebuildLayoutImmediate(_CellsContainer_RectTransform);
            //    //expandedHeight = _CellsContainer_RectTransform.rect.height;
            //}
        }
        public Transform CellsContainer_Transform { get;}

        public PanelCollectionViewer PanelCollectionViewer { get;}
        private readonly PanelScene _PanelScene;
        private readonly string _GroupName;

        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;

        private readonly GameObject _DividerButton_GameObject;
        private readonly RectTransform _DividerButton_RectTransform;
        /// <summary>
        /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
        /// </summary>
        private readonly Button _DividerButton_Button;

        /// <summary>
        /// Контейнер, содержащий все ячейки инвентаря для этой группы.
        /// На этом объекте должен быть RectTransform.
        /// </summary>
        private readonly GameObject _CellsContainer_GameObject;
        private readonly RectTransform _CellsContainer_RectTransform;
        private readonly GridLayoutGroup _CellsContainer_GridLayoutGroup;

        private readonly TextMeshProUGUI _DividerButton_TextMeshProUGUI;

        private readonly IEnumerable<CollectionElement> _CollectionElementList;
        private readonly List<PanelIconCollectionElement> _CollectionElementDataList;

        /// <summary>
        /// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
        /// </summary>
        private bool _Destroying = false;

        /// <summary>
        /// Текущее состояние группы (true - развернута, false - свернута).
        /// </summary>
        private bool _Expanded = true;



        /// <summary>
        /// Переключает состояние группы и запускает анимацию.
        /// </summary>
        private void ToggleGroup()
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
            OnResized();
            //await UniTask.Delay(1); // Заглушка для асинхронности
            //UpdateDividerVisual(isExpanded);
        }

        public void OnResized()
        {
            float width = PanelCollectionViewer.Width;
            float coefHeight = G.GetCoefHeight();
            float buttonHeight = DIVIDER_BUTTON_HEIGHT * coefHeight;
            float height = buttonHeight;

            _DividerButton_RectTransform.sizeDelta.Set(width, buttonHeight);
            _DividerButton_TextMeshProUGUI.fontSize = DIVIDER_BUTTON_FONTSIZE * coefHeight;

            if (_Expanded)
            {
                float spacing = SPACING * coefHeight;
                float cellSize = CELL_SIZE * coefHeight;
                int padding = (int)(PADDING * coefHeight);
                //расчитываем сколько при этих параметрах войдет ячеек
                float widthWithoutPadding = width - (padding * 2);
                int countCellInRow = (int)(widthWithoutPadding / cellSize);
                if (countCellInRow < 1)
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
                _CellsContainer_GridLayoutGroup.spacing.Set(spacing, spacing);
                _CellsContainer_GridLayoutGroup.cellSize.Set(cellSize, cellSize);


                // вычисляем количество строк
                int countCollectionElement = _CollectionElementList.Count();
                int countRows = (countCollectionElement / countCellInRow) + (countCollectionElement % countCellInRow == 0 ? 0 : 1);
                if (countRows < 1)
                {
                    countRows = 1;
                }

                float heightContainer = (countRows * cellSize) + ((countRows - 1) * spacing) + (padding * 4);// по сути нужно 2 но чтобы сделать низ длиннее поставил 4
                _CellsContainer_RectTransform.sizeDelta .Set(width, heightContainer);
                _CellsContainer_RectTransform.anchoredPosition.Set(0f, -DIVIDER_BUTTON_HEIGHT * coefHeight);

                height += heightContainer;

                float collectionElementFontsize = COLLECTION_ELEMENT_FONTSIZE * coefHeight;
                _CollectionElementDataList.ForEach(a=>a.textMeshPro.fontSize = collectionElementFontsize);
            }

            _RectTransform.sizeDelta.Set(width, height);
        }

        public void Destroy()
        {
            _Destroying = true;
            UnityEngine.Object.Destroy(_GameObject);
        }

        public void UnselectAll() {

        }
        async UniTask ShowEquipment()
        {
           

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
        
      
    }
}
