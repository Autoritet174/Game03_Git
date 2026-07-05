using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelTop__prefab__scriptMB : MonoBehaviour, IPrefab
{
    private RectTransform This__RectTransform;
    private RectTransform ButtonClose__RectTransform;

    public bool Initialized { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    public void Initialize()
    {
        This__RectTransform = GetComponent<RectTransform>();
        ButtonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", This__RectTransform.transform);
        Initialized = true;
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        if (!Initialized)
        {
            return;
        }

        Height = Screen.height;
        Width = Screen.width;

        float panelTop_Height = G.PANELTOP_HEIGHT * coefHeight;
        This__RectTransform.sizeDelta = new Vector2(Width, panelTop_Height);
        ButtonClose__RectTransform.sizeDelta = new Vector2(panelTop_Height, panelTop_Height);
    }

}
