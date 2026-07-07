using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelTop__prefab__scriptMB : MonoBehaviour, IPrefab
{
    private RectTransform This__RectTransform;
    private RectTransform ButtonClose__RectTransform;

    public bool initialized { get; private set; }
    public float width { get; private set; }
    public float height { get; private set; }

    public void Initialize()
    {
        This__RectTransform = GetComponent<RectTransform>();
        ButtonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", This__RectTransform.transform);
        initialized = true;
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        if (!initialized)
        {
            return;
        }

        width = Screen.width;
        height = G.PANELTOP_HEIGHT * coefHeight;

        This__RectTransform.sizeDelta = new Vector2(width, height);
        ButtonClose__RectTransform.sizeDelta = new Vector2(height, height);
    }

}
