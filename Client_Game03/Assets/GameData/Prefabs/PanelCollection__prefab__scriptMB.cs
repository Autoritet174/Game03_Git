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
    public bool Initialized { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    private RectTransform _RectTransform;


    private readonly List<PanelGroupDivider__prefab__script> _GroupDividers = new();
    private readonly Dictionary<Guid, PanelIconCollectionElement> _Elements = new();

    public ECollectionMode CollectionMode { get; private set; } = ECollectionMode.Hero;

    public int PageCurrent { get; private set; } = 1;
    public int PageMax { get; private set; } = 1;
    public int MaxCollectionElements { get; private set; }
    //public IPanelCollectionViewerContext ViewerContext => _PanelCollectionViewerContext;


    public void Initialize()
    {
        _RectTransform = GetComponent<RectTransform>();

        PanelTopButtons_Initialize();
        PanelCollectionViewer_Initialize();
        Initialized = true;
    }


    #region ================ PanelTopButtons ================

    private GameObject PanelTopButtons__GameObject;
    private RectTransform PanelTopButtons__RectTransform;

    private FilterButton PanelTopButtons_FilterButtonHeroes;
    private FilterButton PanelTopButtons_FilterButtonEquipments;
    private FilterButton PanelTopButtons_FilterButtonFilter;
    private FilterButton PanelTopButtons_FilterButtonGroup;
    private FilterButton PanelTopButtons_FilterButtonSort;
    private RectTransform PanelTopButtons_RangePanel__RectTransform;
    private RectTransform PanelTopButtons_ButtonPrevPage__RectTransform;
    private RectTransform PanelTopButtons_ButtonNextPage__RectTransform;
    private RectTransform PanelTopButtons_LabelRangePage__RectTransform;
    private TextMeshProUGUI PanelTopButtons_LabelRangePage__TextMeshProUGUI;
    public float PanelTopButtons_Height { get; private set; }

    private void PanelTopButtons_Initialize() {
        PanelTopButtons__GameObject = GameObjectFinder.FindByName("PanelTopButtons", gameObject);
        PanelTopButtons__RectTransform = PanelTopButtons__GameObject.GetComponent<RectTransform>();

        PanelTopButtons_FilterButtonHeroes = new("ImageButtonHeroes", PanelTopButtons__GameObject.transform);
        PanelTopButtons_FilterButtonEquipments = new("ImageButtonEquipments", PanelTopButtons__GameObject.transform);
        PanelTopButtons_FilterButtonFilter = new("ImageButtonFilter", PanelTopButtons__GameObject.transform);
        PanelTopButtons_FilterButtonGroup = new("ImageButtonGroup", PanelTopButtons__GameObject.transform);
        PanelTopButtons_FilterButtonSort = new("ImageButtonSort", PanelTopButtons__GameObject.transform);

        // PanelRange
        {
            PanelTopButtons_RangePanel__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelRange", PanelTopButtons__GameObject);
            PanelTopButtons_ButtonPrevPage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonPrevPage", PanelTopButtons__GameObject);
            PanelTopButtons_ButtonNextPage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonNextPage", PanelTopButtons__GameObject);
            PanelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            PanelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() => PagePrev());
            PanelTopButtons_ButtonNextPage__RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() => PageNext());

            PanelTopButtons_LabelRangePage__RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelRangePage", PanelTopButtons__GameObject);
            PanelTopButtons_LabelRangePage__TextMeshProUGUI = PanelTopButtons_LabelRangePage__RectTransform.GetComponent<TextMeshProUGUI>();
        }
    }

    public void PanelTopButtons_SetPageDiapason()
    {
        PanelTopButtons_LabelRangePage__TextMeshProUGUI.text = $"{((PageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {MaxCollectionElements}";
    }

    public void PanelTopButtons_ResetPageCurrent()
    {
        PageCurrent = 1;
    }

    public void PanelTopButtons_UpdatePageMax()
    {
        int count = GetCollectionCount(CollectionMode);
        PageMax = (count / Game03Client.Collection.CollectionProvider.PAGE_SIZE) + (count % Game03Client.Collection.CollectionProvider.PAGE_SIZE > 0 ? 1 : 0);
        if (PageMax < 1)
        {
            PageMax = 1;
        }

        if (PageCurrent > PageMax)
        {
            PageCurrent = PageMax;
        }

        bool hasMultiplePages = PageMax > 1;
        PanelTopButtons_ButtonPrevPage__RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && PageCurrent > 1;
        PanelTopButtons_ButtonNextPage__RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && PageMax > PageCurrent;
    }

    private void PanelTopButtons_OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        PanelTopButtons_Height = 113f * coefHeight;

        PanelTopButtons__RectTransform.sizeDelta = new Vector2(Width, PanelTopButtons_Height);

        PanelTopButtons_FilterButtonHeroes.OnResized(0);
        PanelTopButtons_FilterButtonEquipments.OnResized(0);
        PanelTopButtons_FilterButtonFilter.OnResized(1);
        PanelTopButtons_FilterButtonGroup.OnResized(2);
        PanelTopButtons_FilterButtonSort.OnResized(3);

        float panelRangeLeft = (((FilterButton.SIZE + FilterButton.SPACING) * 4) + (FilterButton.SPACING_ADDITIONAL * 2)) * coefHeight;

        float rangePanelWidth = 230f * coefHeight;
        PanelTopButtons_RangePanel__RectTransform.anchoredPosition = new Vector2(panelRangeLeft, FilterButton.SPACING * coefHeight);
        PanelTopButtons_RangePanel__RectTransform.sizeDelta = new Vector2(rangePanelWidth, 90f * coefHeight);

        float buttonPageWidth = 100f * coefHeight;
        float buttonPageHeight = 60f * coefHeight;
        PanelTopButtons_ButtonPrevPage__RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        PanelTopButtons_ButtonNextPage__RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        PanelTopButtons_LabelRangePage__RectTransform.sizeDelta = new Vector2(rangePanelWidth, 30f * coefHeight);
        PanelTopButtons_LabelRangePage__TextMeshProUGUI.fontSize = 18f * coefHeight;
    }

    #endregion ================ PanelTopButtons ================


    #region ================ PanelCollectionViewer ================
    private RectTransform PanelCollectionViewer__RectTransform;
    private RectTransform PanelCollectionViewer_ScrollbarVertical__RectTransform;
    public Transform PanelCollectionViewer_Content__Transform { get; private set; }
    private VerticalLayoutGroup PanelCollectionViewer_Content__VerticalLayoutGroup;
    private RectTransform PanelCollectionViewer_ViewerViewport__RectTransform;

    private void PanelCollectionViewer_Initialize() {

        PanelCollectionViewer__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionViewer", gameObject);
        PanelCollectionViewer_ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", PanelCollectionViewer__RectTransform);
        PanelCollectionViewer_Content__Transform = GameObjectFinder.FindByName<RectTransform>("Content", PanelCollectionViewer__RectTransform);
        PanelCollectionViewer_Content__VerticalLayoutGroup = PanelCollectionViewer_Content__Transform.GetComponent<VerticalLayoutGroup>();
        PanelCollectionViewer_ViewerViewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", PanelCollectionViewer__RectTransform);
    }

    private void PanelCollectionViewer_OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        float scrollBarWidth = 32f * coefHeight;
        float viewportContentSpacing = 5f;

        PanelCollectionViewer__RectTransform.sizeDelta = new Vector2(Width, Height - PanelTopButtons_Height);

        PanelCollectionViewer_ScrollbarVertical__RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);
        PanelCollectionViewer_ViewerViewport__RectTransform.SetRight(scrollBarWidth);
        PanelCollectionViewer_Content__VerticalLayoutGroup.spacing = viewportContentSpacing * coefHeight;

        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.OnResized());
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
        _GroupDividers.ForEach(a => a.UnselectAll());
    }

    public void AddElement(PanelIconCollectionElement e)
    {
        _Elements.Add(e.Id, e);
    }

    public PanelIconCollectionElement GetElement(Guid id)
    {
        return _Elements.TryGetValue(id, out PanelIconCollectionElement element) ? element : null;
    }

    public void InstantiateCollection(ECollectionMode collectionMode)
    {
        CollectionMode = collectionMode;
        try
        {
            _GroupDividers.ForEach(a => a.Destroy());
            _GroupDividers.Clear();
            _Elements.Clear();

            //OnResized();

            MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * PageCurrent;


            // Переопределение максимального элемента в диапазоне на последней странице
            if (PageCurrent >= PageMax)
            {
                MaxCollectionElements = collectionMode switch
                {
                    ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                    ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                    _ => throw new NotImplementedException(),
                };
            }



            // Добавление GroupDividers
            IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = collectionMode switch
            {
                ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(PageCurrent),
                ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(PageCurrent),
                _ => throw new NotImplementedException(),
            };

            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
                .Where(static a => a.List.Count() > 0)
                .OrderByDescending(static a => a.Priority);

            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                _GroupDividers.Add(new(item, this));
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
        //if (_PanelCollectionContext != null && _PanelCollectionContext.ContextControlsRootSize)
        //{
        //    _RectTransform.SetHorizontalOffsets(0, right);
        //}
        //else
        //{

        //}

        //if (_PanelCollectionContext != null)
        //{
        //    Width = _RectTransform.rect.width;
        //    Height = _RectTransform.rect.height;
        //}


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
        if (PageCurrent > 1)
        {
            PageCurrent--;
            InstantiateCollection(CollectionMode);
        }
    }

    private void PageNext()
    {
        if (PageCurrent < PageMax)
        {
            PageCurrent++;
            InstantiateCollection(CollectionMode);
        }
    }

}
