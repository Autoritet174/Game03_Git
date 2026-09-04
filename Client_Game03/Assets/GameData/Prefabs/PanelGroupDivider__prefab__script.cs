using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Game03Client.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Prefabs
{
    /// <summary>
    /// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
    /// с асинхронной анимацией высоты.
    /// </summary>
    public class PanelGroupDivider__prefab__script
    {
        private const float DIVIDER_BUTTON_HEIGHT = 45f;
        private const float DIVIDER_BUTTON_FONTSIZE = 24f;
        private const float CELL_SIZE = 120f;
        private const float SPACING = 9f;
        private const float PADDING = 22.5f;
        public PanelGroupDivider__prefab__script(GroupCollectionElement groupCollectionElement, PanelCollection__prefab__scriptMB parent)
        {
            _PanelCollection = parent;
            _CollectionElementList = groupCollectionElement.List;
            _GroupName = groupCollectionElement.Name;

            _GameObject = AddressablePrefabProvider.GroupDividerPrefabAddressableGameObject.SafeInstant();
            _GameObject.transform.SetParent(parent.panelCollectionViewer_Content__Transform, false);

            _RectTransform = _GameObject.GetComponent<RectTransform>();

            // Кнопка переключения видимости
            {
                _DividerButton__GameObject = GameObjectFinder.FindByName("DividerButton", _GameObject);
                _DividerButton__RectTransform = _DividerButton__GameObject.GetComponent<RectTransform>();
                _DividerButton__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _DividerButton__GameObject);

                string text;
                if (string.IsNullOrWhiteSpace(_GroupName))
                {
                    text = Game03Client.LocalizationManager.GetValue(L.UI.Label.NoGroup);
                    _DividerButton__TextMeshProUGUI.fontStyle = FontStyles.Italic;
                }
                else
                {
                    text = _GroupName;
                    _DividerButton__TextMeshProUGUI.fontStyle = FontStyles.Normal;
                }
                _DividerButton__TextMeshProUGUI.text = $"{text} ({_CollectionElementList.Count()})";

                // Привязываем метод ToggleGroup к событию клика
                {
                    Button dividerButton_Button = _DividerButton__GameObject.GetComponent<Button>();
                    dividerButton_Button.onClick.RemoveAllListeners();
                    dividerButton_Button.onClick.AddListener(ToggleGroup);
                }


                // Изображения - линии окантовки
                {
                    Image_Arrow__Image = GameObjectFinder.FindByName<Image>("Image_Arrow", _DividerButton__GameObject);
                    Image_Up__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Up", _DividerButton__GameObject);
                    Image_Down__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Down", _DividerButton__GameObject);
                    Image_Left__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Left", _DividerButton__GameObject);
                    Image_Right__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Right", _DividerButton__GameObject);
                }
            }

            // Контейнер контента
            {
                _CellsContainer__GameObject = GameObjectFinder.FindByName("CellsContainer", _GameObject.transform);
                _CellsContainer__RectTransform = _CellsContainer__GameObject.GetComponent<RectTransform>();
                _CellsContainer__GridLayoutGroup = _CellsContainer__GameObject.GetComponent<GridLayoutGroup>();
                CellsContainer__Transform = _CellsContainer__GameObject.transform;
            }


            _PanelIconCollectionElementList = new();
            foreach (CollectionElement collectionElement in _CollectionElementList)
            {
                _PanelIconCollectionElementList.Add(new(this, collectionElement, parent));
            }

            OnResized();
        }

        public Transform CellsContainer__Transform { get; }

        private readonly PanelCollection__prefab__scriptMB _PanelCollection;
        private readonly string _GroupName;

        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;

        private readonly GameObject _DividerButton__GameObject;
        private readonly RectTransform _DividerButton__RectTransform;
        /// <summary>
        /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
        /// </summary>
        //private readonly Button _DividerButton_Button;

        private readonly Image Image_Arrow__Image;
        private readonly RectTransform Image_Up__RectTransform;
        private readonly RectTransform Image_Down__RectTransform;
        private readonly RectTransform Image_Left__RectTransform;
        private readonly RectTransform Image_Right__RectTransform;

        /// <summary>
        /// Контейнер, содержащий все ячейки инвентаря для этой группы.
        /// На этом объекте должен быть RectTransform.
        /// </summary>
        private readonly GameObject _CellsContainer__GameObject;
        private readonly RectTransform _CellsContainer__RectTransform;
        private readonly GridLayoutGroup _CellsContainer__GridLayoutGroup;

        private readonly TextMeshProUGUI _DividerButton__TextMeshProUGUI;

        private readonly IEnumerable<CollectionElement> _CollectionElementList;
        private readonly List<PanelIconCollectionElement> _PanelIconCollectionElementList;

        ///// <summary>
        ///// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
        ///// </summary>
        //private bool _Destroying = false;

        /// <summary>
        /// Текущее состояние группы (true - развернута, false - свернута).
        /// </summary>
        private bool _Expanded = true;


        public List<Guid> GetSelectedElements() {
            return _PanelIconCollectionElementList.Where(a=>a.selected).Select(a => a.Id).ToList();
        }

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
                _CellsContainer__GameObject.SetActive(true);
                Image_Arrow__Image.sprite = AddressablePrefabProvider.UI_button_with_arrow_v4;
                //_CellsContainer_RectTransform.sizeDelta = new Vector2();
                //    //await AnimateHeightAsync(0, expandedHeight, token);
            }
            else
            {
                //    // Сворачивание
                //    //await AnimateHeightAsync(expandedHeight, 0, token);
                //    // После завершения анимации деактивируем контейнер
                _CellsContainer__GameObject.SetActive(false);
                Image_Arrow__Image.sprite = AddressablePrefabProvider.UI_button_with_arrow_v4_reverse;
            }
            OnResized();
            //await UniTask.Delay(1); // Заглушка для асинхронности
            //UpdateDividerVisual(isExpanded);
        }

        public void OnResized()
        {
            float width = _PanelCollection.panelCollectionViewer_Width;
            float coefHeight = G.GetCoefHeight();
            float buttonHeight = DIVIDER_BUTTON_HEIGHT * coefHeight;
            float height = buttonHeight;

            _DividerButton__RectTransform.sizeDelta = new Vector2(width, buttonHeight);
            _DividerButton__TextMeshProUGUI.fontSize = DIVIDER_BUTTON_FONTSIZE * coefHeight;

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

                _CellsContainer__GridLayoutGroup.padding.left = padding;
                _CellsContainer__GridLayoutGroup.padding.right = padding;
                _CellsContainer__GridLayoutGroup.padding.top = padding;
                _CellsContainer__GridLayoutGroup.padding.bottom = padding;
                _CellsContainer__GridLayoutGroup.spacing = new Vector2(spacing, spacing);
                _CellsContainer__GridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);


                // вычисляем количество строк
                int countCollectionElement = _CollectionElementList.Count();
                int countRows = (countCollectionElement / countCellInRow) + (countCollectionElement % countCellInRow == 0 ? 0 : 1);
                if (countRows < 1)
                {
                    countRows = 1;
                }

                float heightContainer = (countRows * cellSize) + ((countRows - 1) * spacing)
                    + (padding * 4);// по сути нужно 2 но чтобы сделать низ длиннее поставил 4
                _CellsContainer__RectTransform.sizeDelta = new Vector2(width, heightContainer);
                _CellsContainer__RectTransform.anchoredPosition = new Vector2(0f, -DIVIDER_BUTTON_HEIGHT * coefHeight);

                _PanelIconCollectionElementList.ForEach(a => a.OnResized());

                height += heightContainer;
            }

            _RectTransform.sizeDelta = new Vector2(width, height);

            float sizeLine = 4 * coefHeight;
            Image_Up__RectTransform.sizeDelta = new Vector2(0, sizeLine);
            Image_Down__RectTransform.sizeDelta = new Vector2(0, sizeLine);
            Image_Left__RectTransform.sizeDelta = new Vector2(sizeLine, 0);
            Image_Right__RectTransform.sizeDelta = new Vector2(sizeLine, 0);
            Image_Arrow__Image.rectTransform.sizeDelta = new Vector2(74*coefHeight, 37*coefHeight);
            Image_Arrow__Image.rectTransform.anchoredPosition = new Vector2(-sizeLine, -sizeLine);
        }

        public void Destroy()
        {
            //_Destroying = true;
            UnityEngine.Object.Destroy(_GameObject);
        }

        public void UnselectAll()
        {
            _PanelIconCollectionElementList.ForEach(_a => _a.SetSelected(false));
        }

        //private async UniTask ShowEquipment()
        //{


        //    _Init_Collection.ButtonTakeOnOff_RectTransform.gameObject.SetClickEvent(async () =>
        //    {
        //        if (collectionElement.TypeCollectionElement != TypeCollectionElement.Equipment)
        //        {
        //            await UniTask.Yield();
        //            throw new Exception();
        //        }

        //        IEnumerable<DtoEquipment> equipments = CollectionProvider.GetCollectionEquipmentsFromCache();
        //        DtoEquipment equipment = equipments.FirstOrDefault(a => a.Id == collectionElement.Id);
        //        if (equipment == null || _Init_Collection.SelectedHeroId == Guid.Empty)
        //        {
        //            return;
        //        }

        //        DtoHero hero = CollectionProvider.GetCollectionHeroesFromCache().FirstOrDefault(a => a.Id == _Init_Collection.SelectedHeroId);
        //        if (hero == null)
        //        {
        //            return;
        //        }

        //        if (equipment.HeroId != null && equipment.SlotId != null)
        //        {
        //            _Init_Collection.ButtonTakeOnOff_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.TakeOff);
        //        }
        //        else if (equipment.HeroId == null && equipment.SlotId == null)
        //        {
        //            // Предмет ни на кого не одет
        //            _Init_Collection.ButtonTakeOnOff_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.TakeOn);
        //            int slotTypeId = equipment.BaseEquipment.EquipmentType.SlotTypeId;
        //            switch (slotTypeId)
        //            {
        //                case 1://Оружие
        //                    break;
        //                case 14://Кольцо
        //                    break;
        //                case 16://Аксессуар
        //                    break;
        //                default:
        //                    {
        //                        int slotId = Game03Client.GameData.Container.Slots.First(a => a.SlotTypeId == slotTypeId).Id;
        //                        DtoEquipment equipmentOnHero = equipments.FirstOrDefault(a => a.SlotId == slotId && a.HeroId == hero.Id);
        //                        if (equipmentOnHero != null)
        //                        {
        //                            // слот занят, через вебсокет снимаем

        //                        }
        //                        else
        //                        {


        //                        }
        //                        // надеваем экипировку на героя
        //                        // через вебсокет команда на сервер, на сервере такая же проверка так как не верим клиенту
        //                        // ждем ответ от сервера с токеном на 3 секунды
        //                        // по ответу ориентируемся одели шмотку или нет
        //                        break;
        //                    }
        //            }


        //        }
        //        else
        //        {
        //            throw new Exception();
        //        }


        //        string slotName = equipment.BaseEquipment.EquipmentType.SlotType.Name;
        //        if (Initializator.Slots1by1.Any(a => string.Compare(slotName, a, StringComparison.InvariantCultureIgnoreCase) == 0))
        //        {

        //        }

        //    }, true);
        //    await UniTask.Yield();
        //}


    }
}
