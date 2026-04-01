using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class Stat
    {
        private const float WIDTH = 234.0576f;
        private const float HEIGHT = 48f;
        private const float SPACING = 5f;
        private const float DESC_FONT_SIZE = 14f;
        private const float VALUE_FONT_SIZE = 20f;
        private const float VALUE2_FONT_SIZE = 16f;

        private readonly string _Name;
        private readonly int _PosY;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly TextMeshProUGUI _Desc_TextMeshProUGUI;
        private readonly TextMeshProUGUI _Value_TextMeshProUGUI;
        private readonly TextMeshProUGUI _Value2_TextMeshProUGUI;

        public Stat(string name, int posY, GameObject gameObject)
        {
            _Name = name;
            _PosY = posY;
            _GameObject = gameObject;
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            _Desc_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelDesc", gameObject.transform);
            _Value_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue", gameObject.transform);
            _Value2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue2", gameObject.transform);

            _Desc_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(name)));
            _Value2_TextMeshProUGUI.SetText(string.Empty);
        }

        public void SetActive(bool active)
        {
            _GameObject.SetActive(active);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            float height = HEIGHT * coefHeight;
            float spacing = SPACING * coefHeight;
            _RectTransform.sizeDelta = new Vector2(WIDTH * coefHeight, height);
            _RectTransform.anchoredPosition = new Vector2(spacing, -spacing - (height * (_PosY - 1)));
            _Desc_TextMeshProUGUI.fontSize = DESC_FONT_SIZE * coefHeight;
            _Value_TextMeshProUGUI.fontSize = VALUE_FONT_SIZE * coefHeight;
            _Value2_TextMeshProUGUI.fontSize = VALUE2_FONT_SIZE * coefHeight;
        }

        public void SetDesc(string value)
        {
            _Desc_TextMeshProUGUI.SetText(value);
        }

        public void SetValue(string value)
        {
            _Value_TextMeshProUGUI.SetText(value);
        }
        public void SetValue(int value)
        {
            _Value_TextMeshProUGUI.SetText(value.ToString());
        }
        public void SetValue(float value)
        {
            _Value_TextMeshProUGUI.SetText(ToStringService.Get(value));
        }
        public void SetValuePercent(float value)
        {
            _Value_TextMeshProUGUI.SetText($"{ToStringService.Get(value)}%");
            _Value2_TextMeshProUGUI.SetText(string.Empty);
        }
    }
}
