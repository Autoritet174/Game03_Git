using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scenes.Collection.Prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General.DTO.Entities.Collection;
using System;
using UnityEngine;
using L = General.LocalizationKeys;

/// <summary>Связывает панели героев, экипировки и списка коллекции.</summary>
public class CollectionSceneInitializator : MonoBehaviour
{
    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    public Background_BlueClouds_v1__prefab__scriptMB background_BlueClouds_v1__prefab__context { get; private set; }

    public PanelTop__prefab__scriptMB panelTop__prefab__context { get; private set; }

    public PanelSelectedHero__prefab__scriptMB panelSelectedHero__context { get; private set; }

    public PanelSelectedEquipment__prefab__scriptMB panelSelectedEquipment__context { get; private set; }

    public PanelCollection__prefab__scriptMB panelCollection__prefab__context { get; private set; }

    private void Start()
    {
        background_BlueClouds_v1__prefab__context = GameObjectFinder.FindByName<Background_BlueClouds_v1__prefab__scriptMB>("Background_BlueClouds_v1__prefab");
        background_BlueClouds_v1__prefab__context.Initialize();

        //try
        //{

        // Верхняя панель
        {
            panelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
            buttonHeroes__TabButton = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", TabButtonHeroesOnClick);
            buttonHeroes__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Heroes)}\r\n{Game03Client.Collection.CollectionProvider.GetCountHeroes()}");
            buttonEquipment__TabButton = new("ButtonEquipment (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", TabButtonEquipmentOnClick);
            buttonEquipment__TabButton.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment)}\r\n{Game03Client.Collection.CollectionProvider.GetCountEquipments()}");
            panelTop__prefab__context.Initialize();
            panelTop__prefab__context.SetActionOnButtonClose(G.ButtonCloseOnClick);
        }

        panelCollection__prefab__context = GameObjectFinder.FindByName("PanelCollection__prefab").GetComponent<PanelCollection__prefab__scriptMB>();
        panelSelectedHero__context = GameObjectFinder.FindByName<PanelSelectedHero__prefab__scriptMB>("PanelSelectedHero__prefab");
        panelSelectedEquipment__context = GameObjectFinder.FindByName<PanelSelectedEquipment__prefab__scriptMB>("PanelSelectedEquipment__prefab");

        // Панель выбранного героя
        {
            panelSelectedHero__context.panelCollection__prefab__context = panelCollection__prefab__context;
            panelSelectedHero__context.panelSelectedEquipment__context = panelSelectedEquipment__context;
            panelSelectedHero__context.sceneOnResized = OnResized;
            panelSelectedHero__context.Initialize();
        }

        // Панель выбранной экипировки
        {
            panelSelectedEquipment__context.panelCollectionContext = panelCollection__prefab__context;
            panelSelectedEquipment__context.sceneOnResized = OnResized;
            panelSelectedEquipment__context.panelSelectedHeroContext = panelSelectedHero__context;
            panelSelectedEquipment__context.tabButtonHeroesOnClick = TabButtonHeroesOnClick;
            panelSelectedEquipment__context.Initialize();
        }

        // Панель коллекции
        {
            PanelCollectionContext panelCollectionContext = new();
            panelCollectionContext.OnCollectionLoaded(this);
            panelCollection__prefab__context.panelCollectionContext = panelCollectionContext;
            panelCollection__prefab__context.Initialize();
            panelCollection__prefab__context.InstantiateCollection(panelCollection__prefab__context.collectionMode);
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

        // Вкладки коллекции
        {
            panelTop__prefab__context.OnResized(coefHeight);
            // Кнопки вкладок
            float tabButtonWidth = 240f * coefHeight;
            float fontSize = 22f * coefHeight;

            buttonHeroes__TabButton.rectTransform.sizeDelta = new(tabButtonWidth, panelTop__prefab__context.height);
            buttonHeroes__TabButton.textMeshProUGUI.fontSize = fontSize;

            buttonEquipment__TabButton.rectTransform.sizeDelta = new(tabButtonWidth, panelTop__prefab__context.height);
            buttonEquipment__TabButton.rectTransform.anchoredPosition = new(tabButtonWidth, 0f);
            buttonEquipment__TabButton.textMeshProUGUI.fontSize = fontSize;
        }

        panelSelectedHero__context.OnResized(coefHeight, top: panelTop__prefab__context.height);
        panelSelectedEquipment__context.OnResized(coefHeight, top: panelTop__prefab__context.height, right: panelSelectedHero__context.width);

        float panelCollection__context_right = 0f;
        if (panelSelectedHero__context.isVisible)
        {
            panelCollection__context_right += panelSelectedHero__context.width + PanelSelectedHero__prefab__scriptMB.WIDTH_SPACING;
        }
        if (panelSelectedEquipment__context.isVisible)
        {
            panelCollection__context_right += panelSelectedEquipment__context.width + PanelSelectedEquipment__prefab__scriptMB.WIDTH_SPACING;
        }
        panelCollection__prefab__context.OnResized(coefHeight, top: panelTop__prefab__context.height, right: panelCollection__context_right);
    }

    private async UniTask ShowHeroByEquipmentAsync()
    {
        Equipment eq = Game03Client.Collection.CollectionProvider.GetEquipment(panelSelectedEquipment__context.equipmentId);
        if (eq == null || eq.heroId == null)
        {
            return;
        }
        panelSelectedHero__context.Show(eq.heroId.Value);
    }

    #region Вкладки коллекции

    private readonly RectTransform panelTop__RectTransform;
    private readonly RectTransform panelTop_ButtonClose_RectTransform;
    private TabButton buttonHeroes__TabButton, buttonEquipment__TabButton;

    private static Color colorOffButton = new(100f / 255f, 100f / 255f, 100f / 255f);

    public void TabButtonHeroesOnClick()
    {
        if (panelCollection__prefab__context.collectionMode == ECollectionMode.hero)
        {
            return;
        }

        buttonEquipment__TabButton.image.color = colorOffButton;
        buttonHeroes__TabButton.image.color = Color.white;

        panelCollection__prefab__context.InstantiateCollection(ECollectionMode.hero);
        RestoreSelection();
    }

    /// <summary>Кнопка "Экипировка".</summary>
    private void TabButtonEquipmentOnClick()
    {
        if (panelCollection__prefab__context.collectionMode == ECollectionMode.equipment)
        {
            return;
        }
        buttonHeroes__TabButton.image.color = colorOffButton;
        buttonEquipment__TabButton.image.color = Color.white;

        panelCollection__prefab__context.InstantiateCollection(ECollectionMode.equipment);
        RestoreSelection();
    }

    private void RestoreSelection()
    {
        Guid selectedId = GetSelectedElementId(panelCollection__prefab__context.collectionMode);
        panelCollection__prefab__context.GetElement(selectedId)?.SetSelected(true);
    }
    #endregion Вкладки коллекции

    public Guid GetSelectedElementId(ECollectionMode collectionMode)
    {
        return collectionMode switch
        {
            ECollectionMode.hero => panelSelectedHero__context.heroId,
            ECollectionMode.equipment => panelSelectedEquipment__context.equipmentId,
            _ => throw new NotImplementedException(),
        };
    }

}
