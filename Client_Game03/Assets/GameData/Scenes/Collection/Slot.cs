using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class Slot
    {
        private const float PANELSLOT_WIDTH = 95f;
        private const float PANELSLOT_HEIGHT = 112f;
        private const float PANELSLOT_LEFT = 10f;
        private const float PANELSLOT_TOP = 10f;
        public const float PANELSLOT_SPACING = 10f;
        private const float PANELSLOTLABEL_FONTSIZE = 13f;

        public string Name { get; private set; }
        private readonly int posX;
        private readonly int posY;
        private readonly RectTransform _RectTransform;
        private readonly RectTransform _Image_RectTransform;
        private readonly RectTransform _LabelSlot_RectTransform;
        private readonly TextMeshProUGUI _TextMeshProUGUI;

        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Left { get; private set; }
        public float Top { get; private set; }

        public Slot(string name, int posX, int posY, Transform parent, string suffix = "")
        {
            this.Name = name;
            this.posX = posX;
            this.posY = posY;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>($"PanelSlot{name}{suffix}", parent);
            _Image_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image", _RectTransform);
            _LabelSlot_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelSlot", _RectTransform);

            _TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelSlot", _RectTransform);
            string lKey = L.UI.Label.Slot.GetKey(name);
            string text = Game03Client.LocalizationManager.GetValue(lKey);
            if (suffix != "")
            {
                text += " " + suffix;
            }
            _TextMeshProUGUI.text = text;
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();

            Left = (((PANELSLOT_WIDTH + PANELSLOT_SPACING) * (posX - 1)) + PANELSLOT_LEFT) * coefHeight;
            Top = (((PANELSLOT_HEIGHT + PANELSLOT_SPACING) * (posY - 1)) + PANELSLOT_TOP) * coefHeight;
            _RectTransform.anchoredPosition = new Vector2(Left, -Top);
            Width = PANELSLOT_WIDTH * coefHeight;
            Height = PANELSLOT_HEIGHT * coefHeight;
            _RectTransform.sizeDelta = new Vector2(Width, Height);
            _TextMeshProUGUI.fontSize = PANELSLOTLABEL_FONTSIZE * coefHeight;

            _Image_RectTransform.anchoredPosition = new Vector2(0f, 0f);
            _Image_RectTransform.sizeDelta = new Vector2(Width, Width);

            _LabelSlot_RectTransform.anchoredPosition = new Vector2(0f, -Width);
            _LabelSlot_RectTransform.sizeDelta = new Vector2(Width, (PANELSLOT_HEIGHT- PANELSLOT_WIDTH) * coefHeight);
        }
    }

}
