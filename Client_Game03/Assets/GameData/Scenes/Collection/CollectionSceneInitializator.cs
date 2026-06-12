using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CollectionSceneInitializator : MonoBehaviour
{
    public static PanelScene PanelSceneInstance { get; private set; }
    public static PanelTop PanelTopInstance { get; set; }
    public static PanelCollection__prefab__scriptMB PanelCollectionInstance { get; private set; }
    public static PanelCollectionViewer__prefab__scriptMB PanelCollectionViewerInstance { get; private set; }
    public static PanelCollectionTopButtons__prefab__scriptMB PanelCollectionTopButtonsInstance { get; private set; }
    public static PanelSelectedHero__prefab__scriptMB PanelSelectedHeroInstance { get; private set; }
    public static PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipmentInstance { get; private set; }
    public static float Width { get; private set; } = 0f;
    public static float Height { get; private set; } = 0f;
    public static bool Initialized { get; private set; } = false;

    private void Awake()
    {
        PanelSceneInstance = new();
        PanelTopInstance = new();

        var panelCollectionObject = GameObjectFinder.FindByName("PanelCollection (id=jcxwa01g)");
        if (panelCollectionObject == null)
        {
            Debug.LogError("CollectionSceneInitializator: PanelCollection (id=jcxwa01g) not found.");
            return;
        }

        PanelCollectionInstance = panelCollectionObject.GetComponent<PanelCollection__prefab__scriptMB>();
        if (PanelCollectionInstance == null)
        {
            Debug.LogError("CollectionSceneInitializator: PanelCollection__prefab__scriptMB is missing on PanelCollection (id=jcxwa01g).");
            return;
        }

        PanelCollectionTopButtonsInstance = PanelCollectionInstance.TopButtons;
        PanelCollectionViewerInstance = PanelCollectionInstance.Viewer;

        PanelSelectedHeroInstance = GameObjectFinder.FindByName("PanelSelectedHero").GetComponent<PanelSelectedHero__prefab__scriptMB>();
        PanelSelectedEquipmentInstance = GameObjectFinder.FindByName("PanelSelectedEquipment").GetComponent<PanelSelectedEquipment__prefab__scriptMB>();

        PanelCollectionInstance.SetContext(new CollectionSceneCollectionContext());
        PanelCollectionTopButtonsInstance.SetContext(new CollectionSceneTopButtonsContext());
        PanelCollectionViewerInstance.SetContext(new CollectionSceneViewerContext());
    }

    private void Start()
    {
        this.RunAsync(StartAsync);
    }

    private async UniTask StartAsync(CancellationToken cancellationToken)
    {
        if (PanelCollectionViewerInstance == null)
        {
            return;
        }

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
