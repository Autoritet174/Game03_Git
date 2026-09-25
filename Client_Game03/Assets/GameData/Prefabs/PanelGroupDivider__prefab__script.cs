using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Game03Client.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Prefabs
{
    /// <summary>
    /// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
    /// с асинхронной анимацией высоты.
    ///</summary>
    public class PanelGroupDivider__prefab__script
    {
        private const float DIVIDER_BUTTON_HEIGHT = 45f;
        private const float DIVIDER_BUTTON_FONTSIZE = 24f;
        private const float CELL_SIZE = 120f;
        private const float SPACING = 9f;
        private const float PADDING = 22.5f;
        public PanelGroupDivider__prefab__script(GroupCollectionElement groupCollectionElement, PanelCollection__prefab__scriptMB parent)
        {
            panelCollection = parent;
            collectionElementList = groupCollectionElement.List;
            groupName = groupCollectionElement.Name;

            gameObject = AddressablePrefabProvider.groupDividerPrefabAddressableGameObject.SafeInstant();
            gameObject.transform.SetParent(parent.panelCollectionViewer_Content__Transform, false);

            rectTransform = gameObject.GetComponent<RectTransform>();

            // Кнопка переключения видимости
            {
                dividerButton__GameObject = GameObjectFinder.FindByName("DividerButton", gameObject);
                dividerButton__RectTransform = dividerButton__GameObject.GetComponent<RectTransform>();
                dividerButton__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", dividerButton__GameObject);

                string text;
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    text = Game03Client.LocalizationManager.GetValue(L.UI.Label.NoGroup);
                    dividerButton__TextMeshProUGUI.fontStyle = FontStyles.Italic;
                }
                else
                {
                    text = groupName;
                    dividerButton__TextMeshProUGUI.fontStyle = FontStyles.Normal;
                }
                dividerButton__TextMeshProUGUI.text = $"{text} ({collectionElementList.Count()})";

                // Привязываем метод ToggleGroup к событию клика
                {
                    Button dividerButton_Button = dividerButton__GameObject.GetComponent<Button>();
                    dividerButton_Button.onClick.RemoveAllListeners();
                    dividerButton_Button.onClick.AddListener(ToggleGroup);
                }

                // Изображения - линии окантовки
                {
                    image_Arrow__Image = GameObjectFinder.FindByName<Image>("Image_Arrow", dividerButton__GameObject);
                    image_Up__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Up", dividerButton__GameObject);
                    image_Down__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Down", dividerButton__GameObject);
                    image_Left__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Left", dividerButton__GameObject);
                    image_Right__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Right", dividerButton__GameObject);
                }
            }

            // Контейнер контента
            {
                cellsContainer__GameObject = GameObjectFinder.FindByName("CellsContainer", gameObject.transform);
                cellsContainer__RectTransform = cellsContainer__GameObject.GetComponent<RectTransform>();
                cellsContainer__GridLayoutGroup = cellsContainer__GameObject.GetComponent<GridLayoutGroup>();
                cellsContainer__Transform = cellsContainer__GameObject.transform;
            }

            panelIconCollectionElementList = new();
            foreach (CollectionElement collectionElement in collectionElementList)
            {
                panelIconCollectionElementList.Add(new(this, collectionElement, parent));
            }

            OnResized();
        }

        public Transform cellsContainer__Transform { get; }

        private readonly PanelCollection__prefab__scriptMB panelCollection;
        private readonly string groupName;

        private readonly GameObject gameObject;
        private readonly RectTransform rectTransform;

        private readonly GameObject dividerButton__GameObject;
        private readonly RectTransform dividerButton__RectTransform;
        /// <summary>Кнопка, при клике на которую происходит сворачивание/разворачивание.</summary>
        //private readonly Button _DividerButton_Button;

        private readonly Image image_Arrow__Image;
        private readonly RectTransform image_Up__RectTransform;
        private readonly RectTransform image_Down__RectTransform;
        private readonly RectTransform image_Left__RectTransform;
        private readonly RectTransform image_Right__RectTransform;

        /// <summary>
        /// Контейнер, содержащий все ячейки инвентаря для этой группы.
        /// На этом объекте должен быть RectTransform.
        ///</summary>
        private readonly GameObject cellsContainer__GameObject;
        private readonly RectTransform cellsContainer__RectTransform;
        private readonly GridLayoutGroup cellsContainer__GridLayoutGroup;

        private readonly TextMeshProUGUI dividerButton__TextMeshProUGUI;

        private readonly IEnumerable<CollectionElement> collectionElementList;
        private readonly List<PanelIconCollectionElement> panelIconCollectionElementList;

        ///// <summary>
        ///// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
        /////</summary>
        //private bool _Destroying = false;

        /// <summary>Текущее состояние группы (true - развернута, false - свернута).</summary>
        private bool expanded = true;

        public List<Guid> GetSelectedElements()
        {
            return panelIconCollectionElementList.Where(a => a.selected).Select(a => a.id).ToList();
        }

        /// <summary>Переключает состояние группы и запускает анимацию.</summary>
        private void ToggleGroup()
        {
            //Debug.Log(1);
            expanded = !expanded;

            if (expanded)
            {
                //    // Разворачивание
                //    // Сначала активируем контейнер, чтобы он участвовал в макете, но с высотой 0
                cellsContainer__GameObject.SetActive(true);
                image_Arrow__Image.sprite = AddressablePrefabProvider.ui_button_with_arrow_v4;
                //_CellsContainer_RectTransform.sizeDelta = new Vector2();
                //    //await AnimateHeightAsync(0, expandedHeight, token);
            }
            else
            {
                //    // Сворачивание
                //    //await AnimateHeightAsync(expandedHeight, 0, token);
                //    // После завершения анимации деактивируем контейнер
                cellsContainer__GameObject.SetActive(false);
                image_Arrow__Image.sprite = AddressablePrefabProvider.ui_button_with_arrow_v4_reverse;
            }
            OnResized();
            //await UniTask.Delay(1); // Заглушка для асинхронности
            //UpdateDividerVisual(isExpanded);
        }

        public void OnResized()
        {
            float width = panelCollection.panelCollectionViewer_Width;
            float coefHeight = G.GetCoefHeight();
            float buttonHeight = DIVIDER_BUTTON_HEIGHT * coefHeight;
            float height = buttonHeight;

            dividerButton__RectTransform.sizeDelta = new(width, buttonHeight);
            dividerButton__TextMeshProUGUI.fontSize = DIVIDER_BUTTON_FONTSIZE * coefHeight;

            if (expanded)
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

                cellsContainer__GridLayoutGroup.padding.left = padding;
                cellsContainer__GridLayoutGroup.padding.right = padding;
                cellsContainer__GridLayoutGroup.padding.top = padding;
                cellsContainer__GridLayoutGroup.padding.bottom = padding;
                cellsContainer__GridLayoutGroup.spacing = new(spacing, spacing);
                cellsContainer__GridLayoutGroup.cellSize = new(cellSize, cellSize);

                // вычисляем количество строк
                int countCollectionElement = collectionElementList.Count();
                int countRows = (countCollectionElement / countCellInRow) + (countCollectionElement % countCellInRow == 0 ? 0 : 1);
                if (countRows < 1)
                {
                    countRows = 1;
                }

                float heightContainer = (countRows * cellSize) + ((countRows - 1) * spacing)
                    + (padding * 4);// по сути нужно 2 но чтобы сделать низ длиннее поставил 4
                cellsContainer__RectTransform.sizeDelta = new(width, heightContainer);
                cellsContainer__RectTransform.anchoredPosition = new(0f, -DIVIDER_BUTTON_HEIGHT * coefHeight);

                panelIconCollectionElementList.ForEach(a => a.OnResized());

                height += heightContainer;
            }

            rectTransform.sizeDelta = new(width, height);

            float sizeLine = 4 * coefHeight;
            image_Up__RectTransform.sizeDelta = new(0, sizeLine);
            image_Down__RectTransform.sizeDelta = new(0, sizeLine);
            image_Left__RectTransform.sizeDelta = new(sizeLine, 0);
            image_Right__RectTransform.sizeDelta = new(sizeLine, 0);
            image_Arrow__Image.rectTransform.sizeDelta = new(74 * coefHeight, 37 * coefHeight);
            image_Arrow__Image.rectTransform.anchoredPosition = new(-sizeLine, -sizeLine);
        }

        public void Destroy()
        {
            //_Destroying = true;
            UnityEngine.Object.Destroy(gameObject);
        }

        public void UnselectAll()
        {
            panelIconCollectionElementList.ForEach(a => a.SetSelected(false));
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
