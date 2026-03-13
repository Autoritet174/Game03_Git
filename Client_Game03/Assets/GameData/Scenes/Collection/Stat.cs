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

        private readonly string _Name;
        private readonly int _PosY;
        private readonly GameObject _GameObject;
        private readonly RectTransform _RectTransform;
        private readonly TextMeshProUGUI _Value_TextMeshProUGUI;

        public Stat(string name, int posY, GameObject gameObject)
        {
            _Name = name;
            _PosY = posY;
            _GameObject = gameObject;
            _RectTransform = _GameObject.GetComponent<RectTransform>();
            TextMeshProUGUI desc = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelDesc", gameObject.transform);
            desc.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(name)));
            _Value_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelValue", gameObject.transform);
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            float height = HEIGHT * coefHeight;
            float spacing = SPACING * coefHeight;
            _RectTransform.sizeDelta = new Vector2(WIDTH * coefHeight, height);
            _RectTransform.anchoredPosition = new Vector2(spacing, -spacing - (height * (_PosY - 1)));
        }

        public void SetValue(string value)
        {
            _Value_TextMeshProUGUI.SetText(value);
        }

        public void SetValue(int value)
        {
            _Value_TextMeshProUGUI.SetText(value.ToString());
        }
        public void SetValue(long value)
        {
            _Value_TextMeshProUGUI.SetText(value.ToString());
        }
        public void SetValue1000(long value)
        {
            _Value_TextMeshProUGUI.SetText((value / 1000L).ToString());
        }
        public void SetValue(double value)
        {
            _Value_TextMeshProUGUI.SetText(value.ToString());
        }
        public void SetValue(float value)
        {
            _Value_TextMeshProUGUI.SetText(value.ToString());
        }
        public void SetValuePercent(double value)
        {
            _Value_TextMeshProUGUI.SetText($"{value:0.0}%");
        }
        public void SetValue1000Percent(long value)
        {
            _Value_TextMeshProUGUI.SetText($"{value / 1000d:0.0}%");
        }
    }
}
