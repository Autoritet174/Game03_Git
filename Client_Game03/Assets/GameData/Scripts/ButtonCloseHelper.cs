using UnityEngine;

namespace Assets.GameData.Scripts
{
    internal static class ButtonCloseHelper
    {
        private const float LENGHT = 43;
        internal static void UpdateSize(float w, float h, RectTransform rectTransform)
        {
            float buttonClose_coefW = w / 1920f;
            float buttonClose_coefH = h / 1080f;
            float buttonClose_coef = buttonClose_coefW > buttonClose_coefH ? buttonClose_coefW : buttonClose_coefH;
            float pos = -LENGHT * buttonClose_coef;
            float size = LENGHT * 2 * buttonClose_coef;
            rectTransform.anchoredPosition = new Vector2(pos, pos);
            rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
}
