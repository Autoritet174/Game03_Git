using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class CollectionSceneInitializator : MonoBehaviour
{
    public static PanelScene PanelSceneInstance { get; private set; }
    public static PanelTop PanelTopInstance { get; set; }
    public static PanelCollection__prefab__scriptMB PanelCollectionInstance { get; private set; }
    public static PanelSelectedHero__prefab__scriptMB PanelSelectedHeroInstance { get; private set; }
    public static PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipmentInstance { get; private set; }
    public static float Width { get; private set; } = 0f;
    public static float Height { get; private set; } = 0f;
    public static bool IsConfigured { get; private set; }
    public static bool Initialized { get; private set; }

    private void Awake()
    {
        IsConfigured = false;
        Initialized = false;

        PanelSceneInstance = new();
        PanelTopInstance = new();

        try
        {
            GameObject panelCollectionObject = GameObjectFinder.FindByName("PanelCollection (id=jcxwa01g)");
            PanelCollectionInstance = panelCollectionObject.GetComponent<PanelCollection__prefab__scriptMB>();
            if (PanelCollectionInstance == null)
            {
                throw new MissingComponentException(
                    "CollectionSceneInitializator: PanelCollection__prefab__scriptMB is missing on PanelCollection (id=jcxwa01g).");
            }

            PanelSelectedHeroInstance = GameObjectFinder.FindByName<PanelSelectedHero__prefab__scriptMB>("PanelSelectedHero");
            PanelSelectedEquipmentInstance = GameObjectFinder.FindByName<PanelSelectedEquipment__prefab__scriptMB>("PanelSelectedEquipment");

            PanelCollectionInstance.SetContext(new PanelCollectionContext());
            PanelCollectionInstance.SetTopButtonsContext(new CollectionSceneTopButtonsContext());
            PanelCollectionInstance.SetViewerContext(new PanelCollectionViewerContext());

            IsConfigured = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"CollectionSceneInitializator: scene configuration failed. {ex.Message}");
        }
    }

    private void Start()
    {
        this.RunAsync(StartAsync);
    }

    private async UniTask StartAsync(CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            return;
        }

        await PanelCollectionInstance.InstantiateCollectionAsync(PanelSceneInstance.CollectionMode);
        Initialized = true;
    }

    private void Update()
    {
        if (!IsConfigured || !Initialized)
        {
            return;
        }

        if (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width))
        {
            OnResized();
        }
    }

    public static void OnResized()
    {
        if (!IsConfigured || !Initialized)
        {
            return;
        }

        Height = Screen.height;
        Width = Screen.width;
        PanelSceneInstance.OnResized();
        PanelTopInstance.OnResized();
        PanelSelectedHeroInstance.OnResized();
        PanelSelectedEquipmentInstance.OnResized();
        PanelCollectionInstance.OnResized(PanelSelectedHeroInstance.Width + PanelSelectedEquipmentInstance.Width); 
    }
}
