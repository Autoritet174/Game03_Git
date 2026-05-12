using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelTopPrefabScript : MonoBehaviour
{
    private RectTransform panelTopPrefab__RectTransform;
    private RectTransform buttonClose__RectTransform;
    private float _Width, _Height;
    private bool initialized = false;

    private void Start()
    {
        panelTopPrefab__RectTransform = GetComponent<RectTransform>();
        buttonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", panelTopPrefab__RectTransform.transform);
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
        buttonClose__RectTransform.sizeDelta = new Vector2(panelTop_Height, panelTop_Height);
    }
}
