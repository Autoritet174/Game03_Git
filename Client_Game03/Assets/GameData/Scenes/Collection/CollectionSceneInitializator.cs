using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using UnityEngine;

public class CollectionSceneInitializator : MonoBehaviour
{

    public static PanelScene PanelSceneInstance { get; private set; }
    public static PanelTop PanelTopInstance { get; set; }
    public static PanelCollection PanelCollectionInstance { get; private set; }
    public static PanelCollectionViewer__prefab__script PanelCollectionViewerInstance { get; private set; }
    public static PanelCollectionTopButtons PanelCollectionTopButtonsInstance { get; private set; }
    public static PanelSelectedHero PanelSelectedHeroInstance { get; private set; }
    public static PanelSelectedEquipment PanelSelectedEquipmentInstance { get; private set; }
    public static float Width { get; private set; } = 0f;
    public static float Height { get; private set; } = 0f;
    public static bool _Initialized = false;

    private async void Start()
    {
        PanelSceneInstance = new();
        PanelTopInstance = new();
        PanelCollectionInstance = new();
        PanelCollectionViewerInstance = GameObjectFinder.FindByName("PanelCollectionViewer (id=ph1oh7dk)").GetComponent<PanelCollectionViewer__prefab__script>();
        PanelCollectionTopButtonsInstance = new();

        PanelSelectedHeroInstance = new();
        PanelSelectedEquipmentInstance = new();
        await PanelCollectionViewerInstance.InstantiateCollectionAsync(PanelSceneInstance.CollectionMode);
        _Initialized = true;
        OnResized();
    }

    private void Update()
    {
        if (_Initialized && (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width)))
        {
            OnResized();
        }
    }

    public static void OnResized()
    {
        if (!_Initialized)
        {
            return;
        }

        Height = Screen.height;
        Width = Screen.width;
        PanelSceneInstance.OnResized();
        PanelTopInstance.OnResized();
        PanelSelectedHeroInstance.OnResized();
        PanelSelectedEquipmentInstance.OnResized();
        PanelCollectionInstance.OnResized();
        PanelCollectionTopButtonsInstance.OnResized();
        PanelCollectionViewerInstance.OnResized();
    }
}
