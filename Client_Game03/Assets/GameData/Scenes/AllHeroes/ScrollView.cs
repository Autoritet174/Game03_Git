using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{

    [Header("Настройки ScrollView")]
    [SerializeField] private RectTransform content; // Контент ScrollView
    //[SerializeField] private bool isVertical = true; // Ось прокрутки (вертикальная или горизонтальная)


    /// <summary>
    /// Количество колонок в сетке.
    /// </summary>
    private readonly int columnCount = 8;

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
    public RectTransform verticalScrollbar;

    /// <summary>
    /// Выполняется при запуске. Настраивает размеры ячеек в GridLayoutGroup.
    /// </summary>
    private void Start()
    {

        //Screen.width;

        // Внешний отступ (слева и справа) в пикселях.




        scrollRect = GetComponent<ScrollRect>();
        scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        gridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();

        if (gridLayout == null)
        {
            throw new MissingComponentException("На объекте Content отсутствует компонент GridLayoutGroup.");
        }

        // Устанавливаем Constraint как FixedColumnCount
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        // Устанавливаем количество колонок
        gridLayout.constraintCount = columnCount;


        if (verticalScrollbar == null)
        {
            throw new MissingReferenceException("Поле verticalScrollbar должно быть назначено в инспекторе.");
        }

        float scrollViewWidth = scrollRectTransform.rect.width;
        float scrollbarWidth = verticalScrollbar.rect.width;

        float percentWidthForImage = 0.9f;

        //float totalAvailableWidth = scrollViewWidth - scrollbarWidth - (horizontalPadding * 2) - (spacing * (columnCount - 1));
        float totalAvailableWidth = scrollViewWidth - scrollbarWidth;
        if (totalAvailableWidth <= 0)
        {
            throw new System.Exception("Недостаточно ширины для размещения элементов.");
        }

        float cellWidth = totalAvailableWidth / columnCount * percentWidthForImage;
        gridLayout.cellSize = new Vector2(cellWidth, cellWidth);
        // Отступ между элементами в пикселях.
        float spacing = totalAvailableWidth / (columnCount - 1) * (1 - percentWidthForImage);
        gridLayout.spacing = new Vector2(spacing, spacing);

        scrollRect = GetComponentInParent<ScrollRect>();

        if (scrollRect != null)
        {
            //Метод для настройки ScrollSensitivity так, чтобы при единичном повороте колеса мыши прокручивалась одна ячейка.
            scrollRect.scrollSensitivity = (cellWidth + spacing);// / 6f / 2f;
        }
    }
}
