using Assets.GameData.Prefabs;
using Assets.GameData.Scripts;
using UnityEngine;

public class PanelCollection__prefab__scriptMB : MonoBehaviour
{
    private const string TopButtonsObjectName = "PanelCollectionTopButtons (id=gmzb0h9f)";
    private const string ViewerObjectName = "PanelCollectionViewer (id=ph1oh7dk)";

    private RectTransform _RectTransform;
    private IPanelCollectionContext _Context;
    private PanelCollectionTopButtons__prefab__scriptMB _TopButtons;
    private PanelCollectionViewer__prefab__scriptMB _Viewer;

    public float Width { get; private set; }
    public float Height { get; private set; }

    public PanelCollectionTopButtons__prefab__scriptMB TopButtons =>
        _TopButtons ??= GameObjectFinder.FindByName<PanelCollectionTopButtons__prefab__scriptMB>(TopButtonsObjectName, gameObject);

    public PanelCollectionViewer__prefab__scriptMB Viewer =>
        _Viewer ??= GameObjectFinder.FindByName<PanelCollectionViewer__prefab__scriptMB>(ViewerObjectName, gameObject);

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
