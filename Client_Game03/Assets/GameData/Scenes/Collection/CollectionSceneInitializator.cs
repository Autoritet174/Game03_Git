using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.prefabs;
using Assets.GameData.Scripts;
using UnityEngine;

public class CollectionSceneInitializator : MonoBehaviour
{

    public static PanelScene PanelSceneInstance { get; private set; }
    public static PanelTop PanelTopInstance { get; set; }
    public static PanelCollection PanelCollectionInstance { get; private set; }
    public static PanelCollectionViewer__prefab__scriptMB PanelCollectionViewerInstance { get; private set; }
    public static PanelCollectionTopButtons PanelCollectionTopButtonsInstance { get; private set; }
    public static PanelSelectedHero__prefab__scriptMB PanelSelectedHeroInstance { get; private set; }
    public static PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipmentInstance { get; private set; }
    public static float Width { get; private set; } = 0f;
    public static float Height { get; private set; } = 0f;
    public static bool Initialized { get; private set; } = false;

    private async void Awake()
    {
        PanelSceneInstance = new();
        PanelTopInstance = new();
        PanelCollectionInstance = new();
        PanelCollectionViewerInstance = GameObjectFinder.FindByName("PanelCollectionViewer (id=ph1oh7dk)").GetComponent<PanelCollectionViewer__prefab__scriptMB>();
        PanelCollectionTopButtonsInstance = new();

        PanelSelectedHeroInstance = GameObjectFinder.FindByName("PanelSelectedHero").GetComponent<PanelSelectedHero__prefab__scriptMB>();
        PanelSelectedEquipmentInstance = GameObjectFinder.FindByName("PanelSelectedEquipment").GetComponent<PanelSelectedEquipment__prefab__scriptMB>();

        PanelCollectionViewerInstance.SetContext(new CollectionSceneViewerContext());
    }

    private async void Start()
    {
        await PanelCollectionViewerInstance.InstantiateCollectionAsync(PanelSceneInstance.CollectionMode);
        Initialized = true;
    }

    private void Update()
    {
        if (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width))
        {
            OnResized();
        }
    }

    public static void OnResized()
    {
        if (!Initialized)
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
