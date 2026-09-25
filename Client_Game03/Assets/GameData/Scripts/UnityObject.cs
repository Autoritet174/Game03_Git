using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scripts
{
    /// <summary>Находит объект интерфейса и запрошенные компоненты по имени.</summary>
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
    }
}
