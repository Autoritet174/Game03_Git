using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class Slot
    {
        private readonly string name;
        private readonly int posX;
        private readonly int posY;
        private readonly RectTransform _RectTransform;
        private readonly TextMeshProUGUI _TextMeshProUGUI;

        public Slot(string name, int posX, int posY, Transform parent, string suffix = "")
        {
            this.name = name;
            this.posX = posX;
            this.posY = posY;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>($"PanelSlot{name}{suffix}", parent);
            _TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelSlot", _RectTransform);
            string lKey = L.UI.Label.Slot.GetKey(name);
            string text = Game03Client.LocalizationManager.GetValue(lKey);
            if (suffix != "")
            {
                text += " " + suffix;
            }
            _TextMeshProUGUI.text = text;
        }


        public void Resize(float coefHeight)
        {
            const float _TopLeftBase = 10f;
            const float widthBase = 95f;
            const float heightBase = widthBase / 0.845693f; // (1f - 0.154307f);
            _RectTransform.sizeDelta = new Vector2(widthBase * coefHeight, heightBase * coefHeight);
            float x = (((widthBase + _TopLeftBase) * (posX - 1)) + _TopLeftBase) * coefHeight;
            float y = (((heightBase + _TopLeftBase) * (posY - 1)) + _TopLeftBase) * coefHeight;
            _RectTransform.anchoredPosition = new Vector2(x, -y);

            _TextMeshProUGUI.fontSize = 15f * coefHeight;
        }
    }

}
