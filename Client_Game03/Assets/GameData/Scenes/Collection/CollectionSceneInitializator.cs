using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.Prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General.DTO.Entities.Collection;
using System;
using UnityEngine;
using L = General.LocalizationKeys;

public class CollectionSceneInitializator : MonoBehaviour
{
    public bool initialized { get; private set; }
    public float width { get; private set; }
    public float height { get; private set; }
    public Background_BlueClouds_v1__prefab__scriptMB Background_BlueClouds_v1__prefab__context { get; private set; }
    public PanelTop__prefab__scriptMB PanelTop__prefab__context { get; private set; }
    public PanelSelectedHero__prefab__scriptMB PanelSelectedHero__context { get; private set; }
    public PanelSelectedEquipment__prefab__scriptMB PanelSelectedEquipment__context { get; private set; }
    public PanelCollection__prefab__scriptMB PanelCollection__prefab__context { get; private set; }


    private void Start()
    {
        Background_BlueClouds_v1__prefab__context = GameObjectFinder.FindByName<Background_BlueClouds_v1__prefab__scriptMB>("Background_BlueClouds_v1__prefab");
        Background_BlueClouds_v1__prefab__context.Initialize();
        
        //try
        //{

        // PanelTop
        {
            PanelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
            ButtonHeroes__TabButton = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", TabButtonHeroesOnClick);
            ButtonHeroes__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Heroes)}\r\n{Game03Client.Collection.CollectionProvider.GetCountHeroes()}");
            ButtonEquipment__TabButton = new("ButtonEquipment (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", TabButtonEquipmentOnClick);
            ButtonEquipment__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment)}\r\n{Game03Client.Collection.CollectionProvider.GetCountEquipments()}");
            PanelTop__prefab__context.Initialize();
            PanelTop__prefab__context.SetActionOnButtonClose(G.ButtonCloseOnClick);
        }


        PanelCollection__prefab__context = GameObjectFinder.FindByName("PanelCollection__prefab").GetComponent<PanelCollection__prefab__scriptMB>();
        PanelSelectedHero__context = GameObjectFinder.FindByName<PanelSelectedHero__prefab__scriptMB>("PanelSelectedHero__prefab");
        PanelSelectedEquipment__context = GameObjectFinder.FindByName<PanelSelectedEquipment__prefab__scriptMB>("PanelSelectedEquipment__prefab");

        // PanelSelectedHero
        {
            PanelSelectedHero__context.PanelCollection__prefab__context = PanelCollection__prefab__context;
            PanelSelectedHero__context.PanelSelectedEquipment__context = PanelSelectedEquipment__context;
            PanelSelectedHero__context.SceneOnResized = OnResized;
            PanelSelectedHero__context.Initialize();
        }


        // PanelSelectedEquipment
        {
            PanelSelectedEquipment__context.panelCollectionContext = PanelCollection__prefab__context;
            PanelSelectedEquipment__context.sceneOnResized = OnResized;
            PanelSelectedEquipment__context.panelSelectedHeroContext = PanelSelectedHero__context;
            PanelSelectedEquipment__context.tabButtonHeroesOnClick = TabButtonHeroesOnClick;
            PanelSelectedEquipment__context.Initialize();
        }


        // PanelCollection__prefab
        {
            PanelCollectionContext panelCollectionContext = new();
            panelCollectionContext.OnCollectionLoaded(this);
            PanelCollection__prefab__context.panelCollectionContext = panelCollectionContext;
            PanelCollection__prefab__context.Initialize();
            PanelCollection__prefab__context.InstantiateCollection(PanelCollection__prefab__context.collectionMode);
        }

        initialized = true;
        OnResized();
        //PanelTop__prefab__context.Initialize();
        //PanelTop__prefab__context.Initialize();
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"CollectionSceneInitializator: scene configuration failed. {ex.Message}");
        //    throw ex;
        //}
        //this.RunAsync(StartAsync);
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width))
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

        height = Screen.height;
        width = Screen.width;
        float coefHeight = G.GetCoefHeight();


        // ================ PanelTop ================
        {
            PanelTop__prefab__context.OnResized(coefHeight);
            // Кнопки вкладок
            float tabButtonWidth = 240f * coefHeight;
            float fontSize = 22f * coefHeight;

            ButtonHeroes__TabButton.rectTransform.sizeDelta = new Vector2(tabButtonWidth, PanelTop__prefab__context.height);
            ButtonHeroes__TabButton.textMeshProUGUI.fontSize = fontSize;

            ButtonEquipment__TabButton.rectTransform.sizeDelta = new Vector2(tabButtonWidth, PanelTop__prefab__context.height);
            ButtonEquipment__TabButton.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0f);
            ButtonEquipment__TabButton.textMeshProUGUI.fontSize = fontSize;
        }

        PanelSelectedHero__context.OnResized(coefHeight, top: PanelTop__prefab__context.height);
        PanelSelectedEquipment__context.OnResized(coefHeight, top: PanelTop__prefab__context.height, right: PanelSelectedHero__context.width);

        float PanelCollection__context_right = 0f;
        if (PanelSelectedHero__context.IsVisible)
        {
            PanelCollection__context_right += PanelSelectedHero__context.width + PanelSelectedHero__prefab__scriptMB.WIDTH_SPACING;
        }
        if (PanelSelectedEquipment__context.isVisible)
        {
            PanelCollection__context_right += PanelSelectedEquipment__context.width + PanelSelectedEquipment__prefab__scriptMB.WIDTH_SPACING;
        }
        PanelCollection__prefab__context.OnResized(coefHeight, top: PanelTop__prefab__context.height, right: PanelCollection__context_right);
    }

    private async UniTask ShowHeroByEquipmentAsync()
    {
        Equipment eq = Game03Client.Collection.CollectionProvider.GetEquipment(PanelSelectedEquipment__context.equipmentId);
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
        if (PanelCollection__prefab__context.collectionMode == ECollectionMode.Hero)
        {
            return;
        }

        ButtonEquipment__TabButton.image.color = ColorOffButton;
        ButtonHeroes__TabButton.image.color = Color.white;

        PanelCollection__prefab__context.InstantiateCollection(ECollectionMode.Hero);
        RestoreSelection();
    }

    /// <summary> Кнопка "Экипировка". </summary>
    private void TabButtonEquipmentOnClick()
    {
        if (PanelCollection__prefab__context.collectionMode == ECollectionMode.Equipment)
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
        Guid selectedId = GetSelectedElementId(PanelCollection__prefab__context.collectionMode);
        PanelCollection__prefab__context.GetElement(selectedId)?.SetSelected(true);
    }
    #endregion ================ PanelTop ================


    public Guid GetSelectedElementId(ECollectionMode collectionMode)
    {
        return collectionMode switch
        {
            ECollectionMode.Hero => PanelSelectedHero__context.HeroId,
            ECollectionMode.Equipment => PanelSelectedEquipment__context.equipmentId,
            _ => throw new NotImplementedException(),
        };
    }

}
