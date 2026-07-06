using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General.DTO.Entities.Collection;
using System;
using UnityEngine;
using L = General.LocalizationKeys;

public class CollectionSceneInitializator : MonoBehaviour
{
    public bool Initialized { get; private set; } = false;
    public float Width { get; private set; } = 0f;
    public float Height { get; private set; } = 0f;
    public Background_BlueClouds_v1__prefab__scriptMB Background_BlueClouds_v1__prefab__context { get; private set; }
    public PanelTop__prefab__scriptMB PanelTop__prefab__context { get; private set; }
    public PanelSelectedHero__prefab__scriptMB PanelSelectedHero__context { get; private set; }
    public PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipment__context { get; private set; }
    public PanelCollection__prefab__scriptMB PanelCollection__prefab__context { get; private set; }



    private void Start()
    {
        Background_BlueClouds_v1__prefab__context = GameObjectFinder.FindByName<Background_BlueClouds_v1__prefab__scriptMB>("Background_BlueClouds_v1__prefab");
        Background_BlueClouds_v1__prefab__context.Initialize();

        try
        {
            // PanelTop
            {
                PanelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
                ButtonHeroes__TabButton = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", TabButtonHeroesOnClick);
                ButtonHeroes__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Heroes)}\r\n{Game03Client.Collection.CollectionProvider.GetCountHeroes()}");
                ButtonEquipment__TabButton = new("ButtonEquipment (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", TabButtonEquipmentOnClick);
                ButtonEquipment__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment)}\r\n{Game03Client.Collection.CollectionProvider.GetCountEquipments()}");
                PanelTop__prefab__context.Initialize();
            }


            // PanelSelectedHero
            {
                PanelSelectedHero__context = GameObjectFinder.FindByName<PanelSelectedHero__prefab__scriptMB>("PanelSelectedHero__prefab");
                PanelSelectedHero__context.SceneOnResized = OnResized;
                PanelSelectedHero__context.PanelCollection__prefab__context = PanelCollection__prefab__context;
                PanelSelectedHero__context.Initialize();
            }


            // PanelSelectedEquipment
            {
                PanelSelectedEquipment__context = GameObjectFinder.FindByName<PanelSelectedEquipment__prefab__scriptMB>("PanelSelectedEquipment__prefab");
                PanelSelectedEquipment__context.SceneOnResized = OnResized;
                PanelSelectedEquipment__context.PanelCollection__prefab__context = PanelCollection__prefab__context;
                PanelSelectedEquipment__context.PanelSelectedHero__context = PanelSelectedHero__context;
                PanelSelectedEquipment__context.TabButtonHeroesOnClick = TabButtonHeroesOnClick;
                PanelSelectedEquipment__context.Initialize();
            }


            // PanelCollection__prefab
            {
                PanelCollection__prefab__context = GameObjectFinder.FindByName("PanelCollection__prefab").GetComponent<PanelCollection__prefab__scriptMB>();
                PanelCollectionViewerContext panelCollectionViewerContext = new();
                PanelCollection__prefab__context.PanelCollectionViewerContext = panelCollectionViewerContext;
                PanelCollection__prefab__context.Initialize();
                PanelCollection__prefab__context.InstantiateCollection(PanelCollection__prefab__context.CollectionMode);
                panelCollectionViewerContext.OnCollectionLoaded(this);
            }
            Initialized = true;
            OnResized();
            //PanelTop__prefab__context.Initialize();
            //PanelTop__prefab__context.Initialize();
        }
        catch (Exception ex)
        {
            Debug.LogError($"CollectionSceneInitializator: scene configuration failed. {ex.Message}");
            throw ex;
        }
        //this.RunAsync(StartAsync);
    }


    //private async UniTask StartAsync(CancellationToken cancellationToken)
    //{
    //    if (!IsConfigured)
    //    {
    //        return;
    //    }

    //    await PanelCollectionInstance.InstantiateCollectionAsync(PanelSceneInstance.CollectionMode);
    //    Initialized = true;
    //}

    private void Update()
    {
        if (!Initialized)
        {
            return;
        }

        if (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width))
        {
            OnResized();
        }
    }

    public void OnResized()
    {
        if (!Initialized)
        {
            return;
        }

        Height = Screen.height;
        Width = Screen.width;
        float coefHeight = G.GetCoefHeight();


        // ================ PanelTop ================
        {
            PanelTop__prefab__context.OnResized(coefHeight);
            // Кнопки вкладок
            float tabButtonWidth = 240f * coefHeight;
            float fontSize = 22f * coefHeight;

            ButtonHeroes__TabButton.rectTransform.sizeDelta = new Vector2(tabButtonWidth, PanelTop__prefab__context.Height);
            ButtonHeroes__TabButton.textMeshProUGUI.fontSize = fontSize;

            ButtonEquipment__TabButton.rectTransform.sizeDelta = new Vector2(tabButtonWidth, PanelTop__prefab__context.Height);
            ButtonEquipment__TabButton.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0f);
            ButtonEquipment__TabButton.textMeshProUGUI.fontSize = fontSize;
        }

        PanelSelectedHero__context.OnResized(coefHeight, top: PanelTop__prefab__context.Height);
        PanelSelectedEquipment__context.OnResized(coefHeight, top: PanelTop__prefab__context.Height, right: PanelSelectedHero__context.Width);

        float PanelCollection__context_right = 0f;
        if (PanelSelectedHero__context.IsVisible)
        {
            PanelCollection__context_right += PanelSelectedHero__context.Width + PanelSelectedHero__prefab__scriptMB.WIDTH_SPACING;
        }
        if (PanelSelectedEquipment__context.IsVisible)
        {
            PanelCollection__context_right += PanelSelectedEquipment__context.Width + PanelSelectedEquipment__prefab__scriptMB.WIDTH_SPACING;
        }
        PanelCollection__prefab__context.OnResized(coefHeight,top: PanelTop__prefab__context.Height, right: PanelCollection__context_right);
    }

    private async UniTask ShowHeroByEquipmentAsync()
    {
        Equipment eq = Game03Client.Collection.CollectionProvider.GetEquipment(PanelSelectedEquipment__context.EquipmentId);
        if (eq == null || eq.HeroId == null)
        {
            return;
        }
        PanelSelectedHero__context.Show(eq.HeroId.Value);
    }



    #region ================ PanelTop ================

    private readonly RectTransform PanelTop__RectTransform;
    private readonly RectTransform PanelTop_ButtonClose_RectTransform;
    private TabButton ButtonHeroes__TabButton, ButtonEquipment__TabButton;

    private static Color ColorOffButton = new(100f / 255f, 100f / 255f, 100f / 255f);

    public void TabButtonHeroesOnClick()
    {
        if (PanelCollection__prefab__context.CollectionMode == ECollectionMode.Hero)
        {
            return;
        }
        ButtonEquipment__TabButton.image.color = ColorOffButton;
        ButtonHeroes__TabButton.image.color = Color.white;
        PanelCollection__prefab__context.InstantiateCollection(ECollectionMode.Hero);
    }

    /// <summary> Кнопка "Экипировка". </summary>
    private void TabButtonEquipmentOnClick()
    {
        if (PanelCollection__prefab__context.CollectionMode == ECollectionMode.Equipment)
        {
            return;
        }
        ButtonHeroes__TabButton.image.color = ColorOffButton;
        ButtonEquipment__TabButton.image.color = Color.white;

        PanelCollection__prefab__context.InstantiateCollection(ECollectionMode.Equipment);
        RestoreSelection();
    }

    private void RestoreSelection()
    {
        Guid selectedId = GetSelectedElementId(PanelCollection__prefab__context.CollectionMode);
        PanelCollection__prefab__context.GetElement(selectedId)?.Selected(true);
    }
    #endregion ================ PanelTop ================


    public Guid GetSelectedElementId(ECollectionMode collectionMode)
    {
        return collectionMode switch
        {
            ECollectionMode.Hero => PanelSelectedHero__context.HeroId,
            ECollectionMode.Equipment => PanelSelectedEquipment__context.EquipmentId,
            _ => throw new NotImplementedException(),
        };
    }

}
