using UnityEngine;

namespace Assets.GameData.Scripts
{
    /// <summary>Пересчитывает размеры и положение кнопки закрытия.</summary>
    internal static class ButtonCloseHelper
    {
        private const float LENGHT = 90;
        internal static void UpdateSize(RectTransform rectTransform)
        {
            float coefHeight = G.GetCoefHeight();
            float size = LENGHT * coefHeight;
            rectTransform.sizeDelta = new(size, size);
        }
    }
}
