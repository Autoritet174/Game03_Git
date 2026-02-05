using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    /// <summary>
    /// Кнопка фильтра.
    /// </summary>
    public class FilterButton
    {
        public const float SIZE = 86f;
        public const float SPACING = 5f;
        public const float SPACING_ADDITIONAL = SPACING * 5f;
        private const float BUTTON_SIZE = 77f;
        private const float LABEL_HEIGHT = 13f;
        private const float LABEL_FONTSIZE = 18f;

        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;
        private readonly RectTransform _Button_RectTransform;
        private readonly RectTransform _Label_RectTransform;
        private readonly TextMeshProUGUI _TextMeshProUGUILabel;

        public FilterButton(string name)
        {
            _RectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            _GameObject = _RectTransform.gameObject;
            _Button_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button", _RectTransform.transform);
            _Label_RectTransform = GameObjectFinder.FindByName<RectTransform>("Label", _RectTransform.transform);
            _TextMeshProUGUILabel = GameObjectFinder.FindByName<TextMeshProUGUI>("Label", _RectTransform.transform);
        }

        public void SetActive(bool active)
        {
            _GameObject.SetActive(active);
        }

        public void Show()
        {
            _GameObject.SetActive(true);
        }

        public void Hide()
        {
            _GameObject.SetActive(false);
        }

        public void OnResized(int position)
        {
            float coefHeight = G.GetCoefHeight();
            float size = SIZE * coefHeight;
            _RectTransform.sizeDelta.Set(size, size);
            float spacing = SPACING * coefHeight;

            float shiftX = position > 0 ? SPACING_ADDITIONAL : 0f;
            _RectTransform.anchoredPosition.Set(spacing + shiftX + (position * (size + spacing)), -spacing);

            float buttonSize = BUTTON_SIZE * coefHeight;
            _Button_RectTransform.sizeDelta.Set(buttonSize, buttonSize);

            _Label_RectTransform.sizeDelta.Set(size, LABEL_HEIGHT * coefHeight);
            _Label_RectTransform.anchoredPosition.Set(0f, -size);

            _TextMeshProUGUILabel.fontSize = LABEL_FONTSIZE * coefHeight;
        }
    }

}
