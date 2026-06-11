using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// Устанавливает левый отступ
    /// </summary>
    public static void SetLeft(this RectTransform rect, float left)
    {
        Vector2 offsetMin = rect.offsetMin;
        offsetMin.x = left;
        rect.offsetMin = offsetMin;
    }

    /// <summary>
    /// Устанавливает правый отступ
    /// </summary>
    public static void SetRight(this RectTransform rect, float right)
    {
        Vector2 offsetMax = rect.offsetMax;
        offsetMax.x = -right;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Устанавливает верхний отступ
    /// </summary>
    public static void SetTop(this RectTransform rect, float top)
    {
        Vector2 offsetMax = rect.offsetMax;
        offsetMax.y = -top;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Устанавливает нижний отступ
    /// </summary>
    public static void SetBottom(this RectTransform rect, float bottom)
    {
        Vector2 offsetMin = rect.offsetMin;
        offsetMin.y = bottom;
        rect.offsetMin = offsetMin;
    }

    /// <summary>
    /// Получает левый отступ
    /// </summary>
    public static float GetLeft(this RectTransform rect)
    {
        return rect.offsetMin.x;
    }

    /// <summary>
    /// Получает правый отступ
    /// </summary>
    public static float GetRight(this RectTransform rect)
    {
        return -rect.offsetMax.x;
    }

    /// <summary>
    /// Получает верхний отступ
    /// </summary>
    public static float GetTop(this RectTransform rect)
    {
        return -rect.offsetMax.y;
    }

    /// <summary>
    /// Получает нижний отступ
    /// </summary>
    public static float GetBottom(this RectTransform rect)
    {
        return rect.offsetMin.y;
    }

    /// <summary>
    /// Устанавливает все отступы сразу
    /// </summary>
    public static void SetOffsets(this RectTransform rect, float left, float right, float top, float bottom)
    {
        Vector2 offsetMin = rect.offsetMin;
        Vector2 offsetMax = rect.offsetMax;

        offsetMin.x = left;
        offsetMin.y = bottom;
        offsetMax.x = -right;
        offsetMax.y = -top;

        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Устанавливает горизонтальные отступы
    /// </summary>
    public static void SetHorizontalOffsets(this RectTransform rect, float left, float right)
    {
        Vector2 offsetMin = rect.offsetMin;
        Vector2 offsetMax = rect.offsetMax;

        offsetMin.x = left;
        offsetMax.x = -right;

        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Устанавливает вертикальные отступы
    /// </summary>
    public static void SetVerticalOffsets(this RectTransform rect, float top, float bottom)
    {
        Vector2 offsetMin = rect.offsetMin;
        Vector2 offsetMax = rect.offsetMax;

        offsetMin.y = bottom;
        offsetMax.y = -top;

        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Получает все отступы в виде кортежа (left, right, top, bottom)
    /// </summary>
    public static (float left, float right, float top, float bottom) GetOffsets(this RectTransform rect)
    {
        return (
            rect.offsetMin.x,
            -rect.offsetMax.x,
            -rect.offsetMax.y,
            rect.offsetMin.y
        );
    }

    /// <summary>
    /// Добавляет значение к правому отступу (не меняя левый)
    /// </summary>
    public static void AddToRight(this RectTransform rect, float delta)
    {
        Vector2 offsetMax = rect.offsetMax;
        offsetMax.x -= delta;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Добавляет значение к левому отступу
    /// </summary>
    public static void AddToLeft(this RectTransform rect, float delta)
    {
        Vector2 offsetMin = rect.offsetMin;
        offsetMin.x += delta;
        rect.offsetMin = offsetMin;
    }

    /// <summary>
    /// Добавляет значение к верхнему отступу
    /// </summary>
    public static void AddToTop(this RectTransform rect, float delta)
    {
        Vector2 offsetMax = rect.offsetMax;
        offsetMax.y -= delta;
        rect.offsetMax = offsetMax;
    }

    /// <summary>
    /// Добавляет значение к нижнему отступу
    /// </summary>
    public static void AddToBottom(this RectTransform rect, float delta)
    {
        Vector2 offsetMin = rect.offsetMin;
        offsetMin.y += delta;
        rect.offsetMin = offsetMin;
    }
}
