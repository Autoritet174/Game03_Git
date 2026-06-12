using Assets.GameData.Prefabs;
using Assets.GameData.Scripts;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PanelCollection__prefab__scriptMB : MonoBehaviour
{
    private RectTransform _RectTransform;
    private IPanelCollectionContext _Context;
    private PanelCollectionTopButtons__prefab__scriptMB _TopButtons;
    private PanelCollectionViewer__prefab__scriptMB _Viewer;

    public float Width { get; private set; }
    public float Height { get; private set; }

    public PanelCollectionTopButtons__prefab__scriptMB TopButtons =>
        _TopButtons = _TopButtons != null ? _TopButtons : GameObjectFinder.FindByName<PanelCollectionTopButtons__prefab__scriptMB>("PanelCollectionTopButtons", gameObject);

    public PanelCollectionViewer__prefab__scriptMB Viewer
    {
        get
        {
            if (_Viewer == null)
            {
                _Viewer = GameObjectFinder.FindByName<PanelCollectionViewer__prefab__scriptMB>("PanelCollectionViewer", gameObject);
            }
            return _Viewer;
        }
    }

    private void Awake()
    {
        _RectTransform = GetComponent<RectTransform>();
    }

    public void SetContext(IPanelCollectionContext context)
    {
        _Context = context;
    }

    public void OnResized()
    {
        if (_Context != null && _Context.ContextControlsRootSize)
        {
            (Width, Height) = _Context.GetPanelSize();
            _RectTransform.sizeDelta = new Vector2(Width, Height);
        }
        else
        {
            Width = _RectTransform.rect.width;
            Height = _RectTransform.rect.height;
        }
    }
}
