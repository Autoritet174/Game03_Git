using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection.Prefabs
{
    /// <summary>Отображает название и форматированное значение характеристики.</summary>
    public class Stat__prefab__script
    {
        private const float WIDTH = 220;//234.0576f;
        private const float HEIGHT = 48f;
        private const float SPACING = 5f;
        private const float DESC_FONT_SIZE = 14f;
        private const float VALUE_FONT_SIZE = 20f;
        private const float VALUE2_FONT_SIZE = 16f;

        private readonly int posY;
        private readonly GameObject gameObject;
        private readonly RectTransform rectTransform;
        private readonly TextMeshProUGUI desc_TextMeshProUGUI;
        private readonly TextMeshProUGUI value_TextMeshProUGUI;
        private readonly TextMeshProUGUI value2_TextMeshProUGUI;

        public Stat__prefab__script(string name, int posY, GameObject gameObject)
        {
            this.posY = posY;
            this.gameObject = gameObject;
            rectTransform = this.gameObject.GetComponent<RectTransform>();
            desc_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelDesc", gameObject.transform);
            value_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue", gameObject.transform);
            value2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue2", gameObject.transform);

            desc_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(name)));
            value2_TextMeshProUGUI.SetText(string.Empty);
            OnResized();
        }

        public void RefreshName(string name)
        {
            desc_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(name)));
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            float height = HEIGHT * coefHeight;
            float spacing = SPACING * coefHeight;
            rectTransform.sizeDelta = new(WIDTH * coefHeight, height);
            rectTransform.anchoredPosition = new(0, -spacing - (height * (posY - 1)));
            desc_TextMeshProUGUI.fontSize = DESC_FONT_SIZE * coefHeight;
            value_TextMeshProUGUI.fontSize = VALUE_FONT_SIZE * coefHeight;
            value2_TextMeshProUGUI.fontSize = VALUE2_FONT_SIZE * coefHeight;
        }

        #region Отображение значений

        public void SetValue(string value)
        {
            value_TextMeshProUGUI.SetText(value);
        }

        public void SetValue(int value)
        {
            value_TextMeshProUGUI.SetText(value.ToString());
        }

        public void SetValue(float value)
        {
            value_TextMeshProUGUI.SetText(ToStringService.ToStr(value));
        }

        public void SetValuePercent(float value)
        {
            value_TextMeshProUGUI.SetText($"{ToStringService.ToStr(value)}%");
            value2_TextMeshProUGUI.SetText(string.Empty);
        }

        #endregion Отображение значений
    }
}
