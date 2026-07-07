using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelCollection__prefab__scriptMB : MonoBehaviour, IPrefab
{
    public bool initialized { get; private set; }
    public float width { get; private set; }
    public float height { get; private set; }

    private RectTransform rectTransform;


    private readonly List<PanelGroupDivider__prefab__script> groupDividers = new();
    private readonly Dictionary<Guid, PanelIconCollectionElement> elements = new();

    public ECollectionMode collectionMode { get; private set; } = ECollectionMode.Hero;

    public int pageCurrent { get; private set; } = 1;
    public int pageMax { get; private set; } = 1;
    public int maxCollectionElements { get; private set; }

    public IPanelCollectionContext panelCollectionContext { get; set; }

    public void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        PanelTopButtons_Initialize();
        PanelCollectionViewer_Initialize();
        initialized = true;
    }


    #region ================ PanelTopButtons ================

    private GameObject panelTopButtons__GameObject;
    private RectTransform panelTopButtons__RectTransform;

    private FilterButton panelTopButtons_FilterButtonHeroes;
    private FilterButton panelTopButtons_FilterButtonEquipments;
    private FilterButton panelTopButtons_FilterButtonFilter;
    private FilterButton panelTopButtons_FilterButtonGroup;
    private FilterButton panelTopButtons_FilterButtonSort;
    private RectTransform panelTopButtons_RangePanel__RectTransform;
    private RectTransform panelTopButtons_ButtonPrevPage__RectTransform;
    private RectTransform panelTopButtons_ButtonNextPage__RectTransform;
    private RectTransform panelTopButtons_LabelRangePage__RectTransform;
    private TextMeshProUGUI panelTopButtons_LabelRangePage__TextMeshProUGUI;
    public float panelTopButtons_Height { get; private set; }

    private void PanelTopButtons_Initialize() {
        panelTopButtons__GameObject = GameObjectFinder.FindByName("PanelTopButtons", gameObject);
        panelTopButtons__RectTransform = panelTopButtons__GameObject.GetComponent<RectTransform>();

        panelTopButtons_FilterButtonHeroes = new("ImageButtonHeroes", panelTopButtons__GameObject.transform);
        panelTopButtons_FilterButtonEquipments = new("ImageButtonEquipments", panelTopButtons__GameObject.transform);
        panelTopButtons_FilterButtonFilter = new("ImageButtonFilter", panelTopButtons__GameObject.transform);
        panelTopButtons_FilterButtonGroup = new("ImageButtonGroup", panelTopButtons__GameObject.transform);
        panelTopButtons_FilterButtonSort = new("ImageButtonSort", panelTopButtons__GameObject.transform);

        // PanelRange
        {
            panelTopButtons_RangePanel__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelRange", panelTopButtons__GameObject);
            panelTopButtons_ButtonPrevPage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonPrevPage", panelTopButtons__GameObject);
            panelTopButtons_ButtonNextPage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonNextPage", panelTopButtons__GameObject);
            panelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            panelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() => PagePrev());
            panelTopButtons_ButtonNextPage__RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() => PageNext());

            panelTopButtons_LabelRangePage__RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelRangePage", panelTopButtons__GameObject);
            panelTopButtons_LabelRangePage__TextMeshProUGUI = panelTopButtons_LabelRangePage__RectTransform.GetComponent<TextMeshProUGUI>();
        }
    }

    public void PanelTopButtons_SetPageDiapason()
    {
        panelTopButtons_LabelRangePage__TextMeshProUGUI.text = $"{((pageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {maxCollectionElements}";
    }

    public void PanelTopButtons_ResetPageCurrent()
    {
        pageCurrent = 1;
    }

    public void PanelTopButtons_UpdatePageMax()
    {
        int count = GetCollectionCount(collectionMode);
        pageMax = (count / Game03Client.Collection.CollectionProvider.PAGE_SIZE) + (count % Game03Client.Collection.CollectionProvider.PAGE_SIZE > 0 ? 1 : 0);
        if (pageMax < 1)
        {
            pageMax = 1;
        }

        if (pageCurrent > pageMax)
        {
            pageCurrent = pageMax;
        }

        bool hasMultiplePages = pageMax > 1;
        panelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && pageCurrent > 1;
        panelTopButtons_ButtonNextPage__RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && pageMax > pageCurrent;
    }

    private void PanelTopButtons_OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        panelTopButtons_Height = 113f * coefHeight;

        panelTopButtons__RectTransform.sizeDelta = new Vector2(width, panelTopButtons_Height);

        panelTopButtons_FilterButtonHeroes.OnResized(0);
        panelTopButtons_FilterButtonEquipments.OnResized(0);
        panelTopButtons_FilterButtonFilter.OnResized(1);
        panelTopButtons_FilterButtonGroup.OnResized(2);
        panelTopButtons_FilterButtonSort.OnResized(3);

        float panelRangeLeft = (((FilterButton.SIZE + FilterButton.SPACING) * 4) + (FilterButton.SPACING_ADDITIONAL * 2)) * coefHeight;

        float rangePanelWidth = 230f * coefHeight;
        panelTopButtons_RangePanel__RectTransform.anchoredPosition = new Vector2(panelRangeLeft, FilterButton.SPACING * coefHeight);
        panelTopButtons_RangePanel__RectTransform.sizeDelta = new Vector2(rangePanelWidth, 90f * coefHeight);

        float buttonPageWidth = 100f * coefHeight;
        float buttonPageHeight = 60f * coefHeight;
        panelTopButtons_ButtonPrevPage__RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        panelTopButtons_ButtonNextPage__RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        panelTopButtons_LabelRangePage__RectTransform.sizeDelta = new Vector2(rangePanelWidth, 30f * coefHeight);
        panelTopButtons_LabelRangePage__TextMeshProUGUI.fontSize = 18f * coefHeight;
    }

    #endregion ================ PanelTopButtons ================


    #region ================ PanelCollectionViewer ================
    private RectTransform panelCollectionViewer__RectTransform;
    private RectTransform panelCollectionViewer_ScrollbarVertical__RectTransform;
    public Transform panelCollectionViewer_Content__Transform { get; private set; }
    public float panelCollectionViewer_Width { get; private set; }
    private VerticalLayoutGroup panelCollectionViewer_Content__VerticalLayoutGroup;
    private RectTransform panelCollectionViewer_ViewerViewport__RectTransform;

    private void PanelCollectionViewer_Initialize() {

        panelCollectionViewer__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionViewer", gameObject);
        panelCollectionViewer_ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", panelCollectionViewer__RectTransform);
        panelCollectionViewer_Content__Transform = GameObjectFinder.FindByName<Transform>("Content", panelCollectionViewer__RectTransform);
        panelCollectionViewer_Content__VerticalLayoutGroup = panelCollectionViewer_Content__Transform.GetComponent<VerticalLayoutGroup>();
        panelCollectionViewer_ViewerViewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", panelCollectionViewer__RectTransform);
    }

    private void PanelCollectionViewer_OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        float scrollBarWidth = 32f * coefHeight;
        float viewportContentSpacing = 5f;

        panelCollectionViewer__RectTransform.sizeDelta = new Vector2(width, height - panelTopButtons_Height);

        panelCollectionViewer_ScrollbarVertical__RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);

        panelCollectionViewer_Width = width - scrollBarWidth;
        panelCollectionViewer_ViewerViewport__RectTransform.sizeDelta = new Vector2(panelCollectionViewer_Width, 0);

        panelCollectionViewer_Content__VerticalLayoutGroup.spacing = viewportContentSpacing * coefHeight;

        if (groupDividers.Count > 0)
        {
            groupDividers.ForEach(a => a.OnResized());
        }
    }

    #endregion ================ PanelCollectionViewer ================



    public int GetCollectionCount(ECollectionMode collectionMode)
    {
        return collectionMode switch
        {
            ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
            ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
            _ => 0,
        };
    }
    public void UnselectAll()
    {
        groupDividers.ForEach(a => a.UnselectAll());
    }

    public List<Guid> GetSelectedElements() {
        return groupDividers.SelectMany(a => a.GetSelectedElements()).ToList();
    }

    public void AddElement(PanelIconCollectionElement e)
    {
        elements.Add(e.Id, e);
    }

    public PanelIconCollectionElement GetElement(Guid id)
    {
        return elements.TryGetValue(id, out PanelIconCollectionElement element) ? element : null;
    }

    public void InstantiateCollection(ECollectionMode collectionMode)
    {
        this.collectionMode = collectionMode;
        try
        {
            groupDividers.ForEach(a => a.Destroy());
            groupDividers.Clear();
            elements.Clear();

            //OnResized();

            maxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * pageCurrent;


            // Переопределение максимального элемента в диапазоне на последней странице
            if (pageCurrent >= pageMax)
            {
                maxCollectionElements = collectionMode switch
                {
                    ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                    ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                    _ => throw new NotImplementedException(),
                };
            }



            // Добавление GroupDividers
            IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = collectionMode switch
            {
                ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(pageCurrent),
                ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(pageCurrent),
                _ => throw new NotImplementedException(),
            };

            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
                .Where(static a => a.List.Count() > 0)
                .OrderByDescending(static a => a.Priority);

            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                groupDividers.Add(new(item, this));
            }



            PanelTopButtons_UpdatePageMax();
            PanelTopButtons_SetPageDiapason();
            //OnResized();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            GameMessage.Close();
        }
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        width = Screen.width - right;
        height = Screen.height - top;
        rectTransform.sizeDelta = new Vector2(width, height);
        PanelTopButtons_OnResized(coefHeight, top, buttom, left, right);
        PanelCollectionViewer_OnResized(coefHeight, top, buttom, left, right);
    }


    private static int GetPageMax(ECollectionElement collectionElementEnum)
    {
        int count = collectionElementEnum switch
        {
            ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
            ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
            _ => throw new NotImplementedException(),
        };

        int pageSize = Game03Client.Collection.CollectionProvider.PAGE_SIZE;
        return (count / pageSize) + (count % pageSize > 0 ? 1 : 0);
    }

    private void PagePrev()
    {
        if (pageCurrent > 1)
        {
            pageCurrent--;
            InstantiateCollection(collectionMode);
        }
    }

    private void PageNext()
    {
        if (pageCurrent < pageMax)
        {
            pageCurrent++;
            InstantiateCollection(collectionMode);
        }
    }

}
