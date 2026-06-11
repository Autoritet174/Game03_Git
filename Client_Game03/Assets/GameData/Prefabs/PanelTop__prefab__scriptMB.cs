using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelTop__prefab__scriptMB : MonoBehaviour
{
    private RectTransform panelTopPrefab__RectTransform;
    public RectTransform ButtonClose__RectTransform { get; private set; }
    private float _Width, _Height;
    private bool initialized = false;

    private void Start()
    {
        panelTopPrefab__RectTransform = GetComponent<RectTransform>();
        ButtonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", panelTopPrefab__RectTransform.transform);
        initialized = true;
        OnResized();
    }

    private void Update()
    {
        if (!Mathf.Approximately(Screen.height, _Height) || !Mathf.Approximately(Screen.width, _Width))
        {
            OnResized();
        }
    }

    public void OnResized()
    {
        if (!initialized)
        {
            return;
        }

        _Height = Screen.height;
        _Width = Screen.width;

        float coefHeight = G.GetCoefHeight();
        float panelTop_Height = G.PANELTOP_HEIGHT * coefHeight;
        panelTopPrefab__RectTransform.sizeDelta = new Vector2(_Width, panelTop_Height);
        ButtonClose__RectTransform.sizeDelta = new Vector2(panelTop_Height, panelTop_Height);
    }
}
