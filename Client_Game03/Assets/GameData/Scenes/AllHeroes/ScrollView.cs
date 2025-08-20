using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    [SerializeField]
    private RectTransform content; // Контент ScrollView
    //[SerializeField] private bool isVertical = true; // Ось прокрутки (вертикальная или горизонтальная)


    /// <summary>
    /// Количество колонок в сетке.
    /// </summary>
    [SerializeField]
    private int columnCount = 8;

    /// <summary>
    /// Компонент ScrollRect, к которому привязан скрипт.
    /// </summary>
    private ScrollRect scrollRect;

    /// <summary>
    /// Компонент GridLayoutGroup в Content.
    /// </summary>
    private GridLayoutGroup gridLayout;

    /// <summary>
    /// Компонент RectTransform у Scroll View.
    /// </summary>
    private RectTransform scrollRectTransform;

    /// <summary>
    /// Компонент RectTransform у вертикальной полосы прокрутки.
    /// </summary>
    [SerializeField]
    private RectTransform verticalScrollbar;

    private float _lastHeight;
    private float _lastWidth;

    /// <summary>
    /// Выполняется при запуске. Настраивает размеры ячеек в GridLayoutGroup.
    /// </summary>
    private void Start()
    {
        OnResize();
    }
    private void Update()
    {
        // Проверяем изменение высоты Canvas
        if (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth))
        {
            OnResize();
        }
    }
    void OnResize() {
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        if (verticalScrollbar == null)
        {
            throw new MissingReferenceException("Поле verticalScrollbar должно быть назначено в инспекторе.");
        }
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        if (scrollRectTransform == null)
        {
            scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        }
        if (gridLayout == null)
        {
            gridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                throw new MissingComponentException("На объекте Content отсутствует компонент GridLayoutGroup.");
            }
        }


        // Устанавливаем Constraint как FixedColumnCount
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

      



        float scrollViewWidth = scrollRectTransform.rect.width * 0.95f;
        float scrollbarWidth = verticalScrollbar.rect.width;

        float percentWidthForImage = 0.9f;

        //float totalAvailableWidth = scrollViewWidth - scrollbarWidth - (horizontalPadding * 2) - (spacing * (columnCount - 1));
        float totalAvailableWidth = scrollViewWidth - scrollbarWidth;
        //Debug.Log("totalAvailableWidth=" + totalAvailableWidth);
        if (totalAvailableWidth < 200)
        {
            totalAvailableWidth = 200;
        }

        columnCount = totalAvailableWidth switch
        {
            <= 300 => 3,
            <= 800 => Mathf.RoundToInt(totalAvailableWidth / 100),
            _ => 8,
        };

        // Устанавливаем количество колонокя
        gridLayout.constraintCount = columnCount;
        float cellWidth = totalAvailableWidth / columnCount * percentWidthForImage;
        gridLayout.cellSize = new Vector2(cellWidth, cellWidth);

        // Отступ между элементами в пикселях.
        float spacing = totalAvailableWidth / (columnCount - 1) * (1 - percentWidthForImage);
        gridLayout.spacing = new Vector2(spacing, spacing);

        //scrollRect = GetComponentInParent<ScrollRect>();


        //настройки ScrollSensitivity так, чтобы при единичном повороте колеса мыши прокручивалась одна ячейка.
        scrollRect.scrollSensitivity = cellWidth + spacing;// / 6f / 2f;

    }
}
