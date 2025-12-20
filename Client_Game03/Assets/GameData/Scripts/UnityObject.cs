using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.GameData.Scripts
{
    internal class UnityObject
    {
        internal readonly GameObject gameObject;
        internal readonly RectTransform rectTransform;
        internal readonly TextMeshProUGUI textMeshProUGUI;
        internal readonly Image image;
        internal readonly Button button;

        internal UnityObject(string name, bool findRectTransform = false, bool findTextMeshProUGUI = false, bool findImage = false, bool findButton = false)
        {
            gameObject = GameObjectFinder.FindByName(name);
            rectTransform = findRectTransform ? GameObjectFinder.FindByName<RectTransform>(name) : null;
            textMeshProUGUI = findTextMeshProUGUI ? GameObjectFinder.FindByName<TextMeshProUGUI>(name) : null;
            image = findImage ? GameObjectFinder.FindByName<Image>(name) : null;
            button = findButton ? GameObjectFinder.FindByName<Button>(name) : null;
        }
        //internal void AddButtonListener(UnityAction action)
        //{
        //    if (button != null)
        //    {
        //        button.onClick.AddListener(action);
        //    }
        //}
        //internal void SetText(string text)
        //{
        //    if (textMeshProUGUI != null)
        //    {
        //        textMeshProUGUI.text = text;
        //    }
        //}
        //internal void SetImage(Image image) {
        //    if (this.image != null) {
        //        this.image.sprite = image.sprite;
        //    }
        //}
    }
}
