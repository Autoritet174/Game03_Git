using Assets.GameData.Scripts;
using Game03Client.PlayerCollection;
using General;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
/// с асинхронной анимацией высоты.
/// </summary>
public class GroupDivider : MonoBehaviour
{
    private Init_Collection _Init_Collection;

    private string group_name;

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

    private IEnumerable<CollectionElement> _listCollectionElement;

    public class DataCollectionElement
    {
        public GameObject gameObject;
        public CollectionElement collectionCollectionElement;
        public TextMeshProUGUI textMeshPro;
        public bool Selected = false;
        public Image imageRarity;
        public RectTransform rectTransform;
    }
    public readonly List<DataCollectionElement> ListDataCollectionElement = new();


    /// <summary>
    /// Текущее состояние группы (true - развернута, false - свернута).
    /// </summary>
    private bool expanded = true;

    /// <summary>
    /// Переключает состояние группы и запускает анимацию.
    /// </summary>
    public async void ToggleGroup()
    {
        //Debug.Log(1);
        expanded = !expanded;

        if (expanded)
        {
            //    // Разворачивание
            //    // Сначала активируем контейнер, чтобы он участвовал в макете, но с высотой 0
            _CellsContainer_GameObject.SetActive(true);
            _DividerButton_Button.image.sprite = await Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2").Task;
            //_CellsContainer_RectTransform.sizeDelta = new Vector2();
            //    //await AnimateHeightAsync(0, expandedHeight, token);
        }
        else
        {
            //    // Сворачивание
            //    //await AnimateHeightAsync(expandedHeight, 0, token);
            //    // После завершения анимации деактивируем контейнер
            _CellsContainer_GameObject.SetActive(false);
            _DividerButton_Button.image.sprite = await Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2_reverse").Task;
        }
        Resize();
        //UpdateDividerVisual(isExpanded);
    }

    public async Task Init(string group_name, Init_Collection init_Collection, GameObject gameObjectInput, IEnumerable<CollectionElement> listCollectionElement)
    {
        _Init_Collection = init_Collection;
        this.group_name = group_name;
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
        TextMeshProUGUI dividerButton_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _DividerButton_GameObject.transform);
        Transform cellsContainer_Transform = _CellsContainer_GameObject.transform;
        if (group_name.IsEmpty())
        {
            dividerButton_TextMeshProUGUI.text = "---No Group---";
            dividerButton_TextMeshProUGUI.fontStyle = FontStyles.Italic;
        }
        else
        {
            dividerButton_TextMeshProUGUI.text = group_name;
        }


        ListDataCollectionElement.Clear();
        foreach (CollectionElement groupCollectionElement in _listCollectionElement)
        {
            GameObject gameObject = await Addressables.LoadAssetAsync<GameObject>("IconCollectionElement").Task;

            GameObject _prefabIconCollectionElement = gameObject.SafeInstant();
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
                        collectionCollectionElement = groupCollectionElement,
                        textMeshPro = textMeshPro,
                        imageRarity = imageRarity,
                        rectTransform = _prefabIconCollectionElement_Transform
                    };

                    ListDataCollectionElement.Add(dataCollectionElement);
                    textMeshPro.text = groupCollectionElement.Name.ToUpper1Char();
                    textMeshPro.fontSize = 14;

                    imageRarity.sprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{groupCollectionElement.Rarity}").Task;
                    imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
                    imageRarity.type = Image.Type.Simple; // Режим без растягивания;

                    string addressableKey = _Init_Collection.CollectionMode switch
                    {
                        1 => $"hero-image-{groupCollectionElement.Name.ToLower()}_face",
                        2 => $"equipment-image-{groupCollectionElement.Name.ToLower()}_icon",
                        _ => throw new Exception("CollectionMode > 2"),
                    };
                    imageCollectionElement.sprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).Task;
                    imageCollectionElement.preserveAspect = true; // Сохраняет пропорции изображения
                    imageCollectionElement.type = Image.Type.Simple; // Режим без растягивания;

                    async Task OnClick()
                    {
                        ListDataCollectionElement.ForEach(a => a.Selected = false);
                        dataCollectionElement.Selected = true;

                        _Init_Collection.PanelSelectedHero_GameObject.SetActive(true);
                        string name = dataCollectionElement.collectionCollectionElement.Name;
                        _Init_Collection.SelectedHeroTop_TextMeshProUGUI.text = name.ToUpper1Char();

                        string addressableKey = _Init_Collection.CollectionMode switch
                        {
                            1 => $"hero-image-{name.ToLower()}",
                            2 => $"",
                            _ => throw new Exception("CollectionMode > 2"),
                        };
                        _Init_Collection.SelectedHero_Image.sprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).Task;
                        _Init_Collection.SelectedHero_Image.preserveAspect = true; // Сохраняет пропорции изображения

                        _Init_Collection.SelectedHeroRarity_Image.sprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{dataCollectionElement.collectionCollectionElement.Rarity}").Task;

                        ListDataCollectionElement.ForEach(a =>
                        {
                            if (a.Selected)
                            {
                                a.rectTransform.localScale = Init_Collection.Vector3Selected;
                                //a.imageRarity.sprite = Init_Collection.Rarityes[0];
                            }
                            else
                            {
                                a.rectTransform.localScale = Vector3.one;
                                //a.imageRarity.sprite = Init_Collection.Rarityes[(int)a.collectionHero.HeroBase.Rarity];
                            }
                        });

                    }
                    void OnPointerEnter()
                    {
                        imageRarity.sprite = _Init_Collection.Rarityes[0];
                    }
                    void OnPointerExit()
                    {
                        imageRarity.sprite = _Init_Collection.Rarityes[dataCollectionElement.collectionCollectionElement.Rarity];
                    }
                    EventHelper.AddClickEvent(_prefabIconCollectionElement, OnClick, false);
                    EventHelper.AddHoverEvents(_prefabIconCollectionElement, OnPointerEnter, OnPointerExit);

                }
            }
        }

        // Привязываем метод ToggleGroup к событию клика
        _DividerButton_Button.onClick.RemoveAllListeners();
        _DividerButton_Button.onClick.AddListener(ToggleGroup);

        // Если группа должна быть свернута по умолчанию, устанавливаем высоту в 0,
        // иначе сохраняем текущую высоту.
        if (!expanded)
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

        //_ = Task.Run(AnimateHeight);
        //UpdateDividerVisual(Expanded);
    }

    public void Resize()
    {
        float width = _Init_Collection.PanelCollection_RectTransform.sizeDelta.x;
        float coefHeight = Screen.height / 1080f;
        float heightButton = 45f * coefHeight;
        float height = heightButton;
        if (expanded)
        {
            float cellSize1080 = 90f;
            float spacing1080 = 9f;
            int paddingR = (int)(40f * coefHeight);
            float spacing = spacing1080 * coefHeight;
            float cellSize = cellSize1080 * coefHeight;
            //расчитываем сколько при этих параметрах войдет ячеек
            float widthWithoutPadding = width - (paddingR + spacing);
            int countCellInRow = (int)(widthWithoutPadding / cellSize);
            if (countCellInRow <= 0)
            {
                countCellInRow = 1;
            }
            float spacingDelta = widthWithoutPadding / ((countCellInRow * (cellSize1080 / spacing1080)) + (countCellInRow - 1));
            float cellSizeDelta = spacingDelta * cellSize1080 / spacing1080;


            _CellsContainer_GridLayoutGroup.padding.left = (int)spacingDelta;
            _CellsContainer_GridLayoutGroup.padding.right = paddingR;
            _CellsContainer_GridLayoutGroup.padding.top = (int)spacingDelta;
            _CellsContainer_GridLayoutGroup.padding.bottom = (int)spacingDelta;
            _CellsContainer_GridLayoutGroup.spacing = new Vector2(spacingDelta, spacingDelta);
            _CellsContainer_GridLayoutGroup.cellSize = new Vector2(cellSizeDelta, cellSizeDelta);



            // вычисляем количество строк
            int countCollectionElement = _listCollectionElement.Count();
            int countRows = (countCollectionElement / countCellInRow) + (countCollectionElement % countCellInRow == 0 ? 0 : 1);
            if (countRows < 1)
            {
                countRows = 1;
            }

            float heightContainer = (countRows * cellSizeDelta) + ((countRows + 1) * spacingDelta);

            _DividerButton_RectTransform.sizeDelta = new Vector2(width, heightButton);
            _CellsContainer_RectTransform.sizeDelta = new Vector2(width, heightContainer);// в зависимости от героев и отступов
            height += heightContainer;

            _CellsContainer_RectTransform.anchoredPosition = new Vector2(0, -45f * coefHeight);


            foreach (DataCollectionElement item in ListDataCollectionElement)
            {
                item.textMeshPro.fontSize = 14f * coefHeight;
            }
        }
        _RectTransform.sizeDelta = new Vector2(width, height);
    }


    // --- Публичные поля, которые настраиваются в Инспекторе ---

    //[SerializeField]
    //private RectTransform rectTransformDividerButton

    //[SerializeField]
    //private RectTransform rectTransformGroupDivider;

    /// <summary>
    /// Скорость анимации (единиц в секунду).
    /// </summary>
    //[Tooltip("Скорость анимации в единицах высоты в секунду.")]
    //[SerializeField]
    //private float animationSpeed = 1000f;


    /// <summary>
    /// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
    /// </summary>
    private bool destroying = false;


    //private RectTransform cellsRectTransform;
    //private float expandedHeight; // Сохраняемая полная высота контейнера
    //private CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameGroup"></param>
    /// <param name="data"></param>
    /// <param name="countInRow">Количество ячеек в строке.</param>
    public void Initialize(string nameGroup, JObject data, int countInRow = 12)
    {

        //cellsRectTransform = gameObjectCellsContainer.GetComponent<RectTransform>();

        //if (cellsRectTransform == null)
        //{
        //    throw new MissingComponentException("cellsContainer не имеет компонента RectTransform.");
        //}


    }


    /// <summary>
    /// Вызывается при уничтожении объекта для отмены всех активных задач.
    /// </summary>
    private void OnDestroy()
    {
        if (!destroying)
        {
            // позже удалить
        }
        destroying = true;
        //cancellationTokenSource?.Cancel();
        //cancellationTokenSource?.Dispose();
    }

    // --- Логика Группировки ---



    private void AnimateHeight()
    {
        //while (true)
        //{
        //    if (destroying) {
        //        return;
        //    }
        //    // Вычисляем смещение
        //    float delta = direction * animationSpeed * Time.deltaTime;
        //    currentHeight += delta;

        //    // Ограничиваем значение конечной точкой, чтобы избежать перескока
        //    if (direction > 0 && currentHeight > endHeight)
        //    {
        //        currentHeight = endHeight;
        //    }
        //    else if (direction < 0 && currentHeight < endHeight)
        //    {
        //        currentHeight = endHeight;
        //    }

        //    SetHeight(currentHeight);


        //    // Асинхронная задержка до следующего кадра (аналог yield return null в корутинах)
        //    await Task.Yield();
        //}
        ////SetHeight(endHeight);
    }
    private void SetHeight(float h)
    {
        //cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        //rectTransformGroupDivider.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h + 45.36f);

        //// Принудительно перестраиваем родительский макет после каждого шага
        //// Это необходимо, чтобы ScrollView и Vertical Layout Group реагировали на изменение высоты.
        //LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Обновляет визуальное представление разделителя (например, меняет иконку-стрелку).
    /// </summary>
    /// <param name="expanded">Текущее состояние: true - развернута, false - свернута.</param>
    //private void UpdateDividerVisual(bool expanded){
    // Здесь можно добавить логику для изменения иконки-стрелки на кнопке
    // Например: arrow.transform.localRotation = expanded ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
    //}
}
