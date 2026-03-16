using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class Stat
    {
        private const float WIDTH = 234.0576f;
        private const float HEIGHT = 28f;
        private const float SPACING = 5f;
        private const float VALUE_FONT_SIZE = 22f;
        private const float DESC_FONT_SIZE = 12f;

        private readonly string _Name;
        private readonly int _PosY;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly TextMeshProUGUI _Value_TextMeshProUGUI;
        private readonly TextMeshProUGUI _Desc_TextMeshProUGUI;

        public Stat(string name, int posY, GameObject gameObject)
        {
            _Name = name;
            _PosY = posY;
            _GameObject = gameObject;
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            TextMeshProUGUI desc = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelDesc", gameObject.transform);
            desc.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(name)));
            _Value_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue", gameObject.transform);
            _Desc_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelDesc", gameObject.transform);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            float height = HEIGHT * coefHeight;
            float spacing = SPACING * coefHeight;
            _RectTransform.sizeDelta = new Vector2(WIDTH * coefHeight, height);
            _RectTransform.anchoredPosition = new Vector2(spacing, -spacing - (height * (_PosY - 1)));
            _Value_TextMeshProUGUI.fontSize = VALUE_FONT_SIZE * coefHeight;
            _Desc_TextMeshProUGUI.fontSize = DESC_FONT_SIZE * coefHeight;
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
            _Value_TextMeshProUGUI.SetText(NumberToStringManager.ToStr3(value));
        }
        public void SetValuePercent(float value)
        {
            _Value_TextMeshProUGUI.SetText($"{NumberToStringManager.ToStr3(value)}%");
        }
    }
}
