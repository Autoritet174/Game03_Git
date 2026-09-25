using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Prefabs
{
    /// <summary>Кнопка фильтра внутри PanelCollectionTopButtons.</summary>
    public class FilterButton
    {
        public const float SIZE = 86f;
        public const float SPACING = 5f;
        public const float SPACING_ADDITIONAL = SPACING * 5f;
        private const float BUTTON_SIZE = 77f;
        private const float LABEL_HEIGHT = 13f;
        private const float LABEL_FONTSIZE = 18f;

        private readonly RectTransform rectTransform;
        private readonly GameObject gameObject;
        private readonly RectTransform button_RectTransform;
        private readonly RectTransform label_RectTransform;
        private readonly TextMeshProUGUI textMeshProUGUILabel;

        public FilterButton(string name, Transform parent)
        {
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name, parent);
            gameObject = rectTransform.gameObject;
            button_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button", rectTransform.transform);
            label_RectTransform = GameObjectFinder.FindByName<RectTransform>("Label", rectTransform.transform);
            textMeshProUGUILabel = GameObjectFinder.FindByName<TextMeshProUGUI>("Label", rectTransform.transform);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void OnResized(int position)
        {
            float coefHeight = G.GetCoefHeight();
            float size = SIZE * coefHeight;
            rectTransform.sizeDelta = new(size, size);
            float spacing = SPACING * coefHeight;

            float shiftX = position > 0 ? SPACING_ADDITIONAL : 0f;
            rectTransform.anchoredPosition = new(spacing + shiftX + (position * (size + spacing)), -spacing);

            float buttonSize = BUTTON_SIZE * coefHeight;
            button_RectTransform.sizeDelta = new(buttonSize, buttonSize);

            label_RectTransform.sizeDelta = new(size, LABEL_HEIGHT * coefHeight);
            label_RectTransform.anchoredPosition = new(0f, -size);

            textMeshProUGUILabel.fontSize = LABEL_FONTSIZE * coefHeight;
        }
    }
}
