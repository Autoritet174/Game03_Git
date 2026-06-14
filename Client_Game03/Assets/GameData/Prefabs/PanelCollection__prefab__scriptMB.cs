using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelCollection__prefab__scriptMB : MonoBehaviour
{
    public const float TOP_BUTTONS_HEIGHT = 113f;

    private const float RANGE_PANEL_WIDTH = 230f;
    private const float RANGE_PANEL_HEIGHT = 90f;
    private const float BUTTON_PAGE_WIDTH = 100f;
    private const float BUTTON_PAGE_HEIGHT = 60f;
    private const float LABEL_HEIGHT = 30f;
    private const float LABEL_FONTSIZE = 18f;

    private const string TopButtonsObjectName = "PanelCollectionTopButtons";
    private const string ViewerObjectName = "PanelCollectionViewer";
    private const string FilterButtonHeroesName = "ImageButtonHeroes (id=pakco5ud)";
    private const string FilterButtonEquipmentsName = "ImageButtonEquipments (id=vuhjngaz)";
    private const string FilterButtonFilterName = "ImageButtonFilter (id=vjeqfzen)";
    private const string FilterButtonGroupName = "ImageButtonGroup (id=hbsaogwl)";
    private const string FilterButtonSortName = "ImageButtonSort (id=6nvcsrdm)";
    private const string PanelRangeName = "PanelRange (id=66z5bnzi)";
    private const string ButtonPrevPageName = "ButtonPrevPage (id=25alql62)";
    private const string ButtonNextPageName = "ButtonNextPage (id=k5moi57b)";
    private const string LabelRangePageName = "LabelRangePage (id=6jgz12bu)";

    [SerializeField]
    private float SCROLLBAR_WIDTH = 32f;

    [SerializeField]
    private float VIEWPORT_CONTENT_SPACING = 5f;

    private RectTransform _RectTransform;
    private IPanelCollectionContext _PanelCollectionContext;
    private IPanelCollectionTopButtonsContext _PanelCollectionTopButtonsContext;
    private IPanelCollectionViewerContext _PanelCollectionViewerContext;

    private GameObject _TopButtons_GameObject;
    private RectTransform _TopButtons_RectTransform;
    private FilterButton _FilterButtonHeroes;
    private FilterButton _FilterButtonEquipments;
    private FilterButton _FilterButtonFilter;
    private FilterButton _FilterButtonGroup;
    private FilterButton _FilterButtonSort;
    private RectTransform _RangePanel_RectTransform;
    private RectTransform _ButtonPrevPage_RectTransform;
    private RectTransform _ButtonNextPage_RectTransform;
    private RectTransform _LabelRangePage_RectTransform;
    private TextMeshProUGUI _LabelRangePage_TextMeshProUGUI;

    private RectTransform _Viewer_RectTransform;
    private RectTransform _ViewerViewport_RectTransform;
    private RectTransform _ScrollbarVertical_RectTransform;
    private VerticalLayoutGroup _Content_VerticalLayoutGroup;
    private readonly List<PanelGroupDivider__prefab__script> _GroupDividers = new();
    private readonly Dictionary<Guid, PanelIconCollectionElement> _Elements = new();

    public float Width { get; private set; }
    public float Height { get; private set; }
    public float TopButtonsHeight { get; private set; }
    public float ViewerWidth { get; private set; }
    public int PageCurrent { get; private set; } = 1;
    public int PageMax { get; private set; } = 1;
    public Transform Content_Transform { get; private set; }
    public int MaxCollectionElements { get; private set; }
    public IPanelCollectionViewerContext ViewerContext => _PanelCollectionViewerContext;

    private void Awake()
    {
        _RectTransform = GetComponent<RectTransform>();

        _TopButtons_GameObject = GameObjectFinder.FindByName(TopButtonsObjectName, gameObject);
        _TopButtons_RectTransform = _TopButtons_GameObject.GetComponent<RectTransform>();

        _FilterButtonHeroes = new(FilterButtonHeroesName, _TopButtons_GameObject.transform);
        _FilterButtonEquipments = new(FilterButtonEquipmentsName, _TopButtons_GameObject.transform);
        _FilterButtonFilter = new(FilterButtonFilterName, _TopButtons_GameObject.transform);
        _FilterButtonGroup = new(FilterButtonGroupName, _TopButtons_GameObject.transform);
        _FilterButtonSort = new(FilterButtonSortName, _TopButtons_GameObject.transform);

        _RangePanel_RectTransform = GameObjectFinder.FindByName<RectTransform>(PanelRangeName, _TopButtons_GameObject);
        _ButtonPrevPage_RectTransform = GameObjectFinder.FindByName<RectTransform>(ButtonPrevPageName, _TopButtons_GameObject);
        _ButtonNextPage_RectTransform = GameObjectFinder.FindByName<RectTransform>(ButtonNextPageName, _TopButtons_GameObject);
        _ButtonPrevPage_RectTransform.gameObject.SetClickEvent(PagePrev, true);
        _ButtonNextPage_RectTransform.gameObject.SetClickEvent(PageNext, true);

        _LabelRangePage_RectTransform = GameObjectFinder.FindByName<RectTransform>(LabelRangePageName, _TopButtons_GameObject);
        _LabelRangePage_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(LabelRangePageName, _TopButtons_GameObject);

        _Viewer_RectTransform = GameObjectFinder.FindByName<RectTransform>(ViewerObjectName, gameObject);
        _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", _Viewer_RectTransform);
        Content_Transform = GameObjectFinder.FindByName<RectTransform>("Content", _Viewer_RectTransform);
        _Content_VerticalLayoutGroup = Content_Transform.GetComponent<VerticalLayoutGroup>();
        _ViewerViewport_RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", _Viewer_RectTransform);
    }

    public void SetContext(IPanelCollectionContext context)
    {
        _PanelCollectionContext = context;
    }

    public void SetTopButtonsContext(IPanelCollectionTopButtonsContext context)
    {
        _PanelCollectionTopButtonsContext = context;
    }

    public void SetViewerContext(IPanelCollectionViewerContext context)
    {
        _PanelCollectionViewerContext = context;
    }

    public void SetPageDiapason(int maxCollectionElements)
    {
        _LabelRangePage_TextMeshProUGUI.text = $"{((PageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {maxCollectionElements}";
    }

    public void ResetPageCurrent()
    {
        PageCurrent = 1;
    }

    public void UpdatePageMax()
    {
        if (_PanelCollectionTopButtonsContext == null)
        {
            PageMax = 1;
            return;
        }

        int count = _PanelCollectionTopButtonsContext.GetCollectionCount(_PanelCollectionTopButtonsContext.CollectionMode);
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
        _ButtonPrevPage_RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && PageCurrent > 1;
        _ButtonNextPage_RectTransform.gameObject.GetComponent<Button>().interactable = hasMultiplePages && PageMax > PageCurrent;
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

    public async UniTask InstantiateCollectionAsync(ECollectionMode collectionMode)
    {
        try
        {
            _GroupDividers.ForEach(a => a.Destroy());
            _GroupDividers.Clear();
            _Elements.Clear();

            OnResized();
            await UniTask.Yield();

            int pageCurrent = _PanelCollectionViewerContext?.PageCurrent ?? 1;
            MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * pageCurrent;

            switch (collectionMode)
            {
                case ECollectionMode.Hero:
                    await LoadCollectionElement(ECollectionElement.Hero);
                    RestoreSelection(ECollectionMode.Hero);
                    break;

                case ECollectionMode.Equipment:
                    await LoadCollectionElement(ECollectionElement.Equipment);
                    RestoreSelection(ECollectionMode.Equipment);
                    break;

                default:
                    throw new Exception();
            }

            _PanelCollectionViewerContext?.OnCollectionLoaded(this, MaxCollectionElements);
            OnResized();
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

    public void OnResized(float right = 0)
    {
        if (_PanelCollectionContext != null && _PanelCollectionContext.ContextControlsRootSize)
        {
            _RectTransform.SetHorizontalOffsets(0, right);
        }
        else
        {
            Width = _RectTransform.rect.width;
            Height = _RectTransform.rect.height;
        }

        OnTopButtonsResized();
        OnViewerResized();
    }

    private void OnTopButtonsResized()
    {
        float coefHeight = G.GetCoefHeight();
        TopButtonsHeight = TOP_BUTTONS_HEIGHT * coefHeight;

        if (_PanelCollectionTopButtonsContext != null && _PanelCollectionTopButtonsContext.ContextControlsRootSize)
        {
            _TopButtons_RectTransform.sizeDelta = new Vector2(_PanelCollectionTopButtonsContext.GetPanelWidth(), TopButtonsHeight);
        }

        _FilterButtonHeroes.OnResized(0);
        _FilterButtonEquipments.OnResized(0);
        _FilterButtonFilter.OnResized(1);
        _FilterButtonGroup.OnResized(2);
        _FilterButtonSort.OnResized(3);

        float panelRangeLeft = (((FilterButton.SIZE + FilterButton.SPACING) * 4) + (FilterButton.SPACING_ADDITIONAL * 2)) * coefHeight;

        _RangePanel_RectTransform.anchoredPosition = new Vector2(panelRangeLeft, FilterButton.SPACING * coefHeight);
        _RangePanel_RectTransform.sizeDelta = new Vector2(RANGE_PANEL_WIDTH * coefHeight, RANGE_PANEL_HEIGHT * coefHeight);

        float buttonPageWidth = BUTTON_PAGE_WIDTH * coefHeight;
        float buttonPageHeight = BUTTON_PAGE_HEIGHT * coefHeight;
        _ButtonPrevPage_RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        _ButtonNextPage_RectTransform.sizeDelta = new Vector2(buttonPageWidth, buttonPageHeight);
        _LabelRangePage_RectTransform.sizeDelta = new Vector2(RANGE_PANEL_WIDTH * coefHeight, LABEL_HEIGHT * coefHeight);
        _LabelRangePage_TextMeshProUGUI.fontSize = LABEL_FONTSIZE * coefHeight;
    }

    private void OnViewerResized()
    {
        float coefHeight = G.GetCoefHeight();
        float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;

        ViewerWidth = _Viewer_RectTransform.rect.width - (TOP_BUTTONS_HEIGHT * coefHeight);
        _Viewer_RectTransform.SetTop(TOP_BUTTONS_HEIGHT * coefHeight);

        _ScrollbarVertical_RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);
        _ViewerViewport_RectTransform.SetRight(scrollBarWidth);
        _Content_VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.OnResized());
        }
    }

    private void RestoreSelection(ECollectionMode collectionMode)
    {
        Guid? selectedId = _PanelCollectionViewerContext?.GetSelectedElementId(collectionMode);
        if (selectedId.HasValue)
        {
            GetElement(selectedId.Value)?.Selected(true);
        }
    }

    private async UniTask LoadCollectionElement(ECollectionElement collectionElementEnum)
    {
        if (_PanelCollectionViewerContext?.LoadAllPages == true)
        {
            int pageMax = GetPageMax(collectionElementEnum);
            MaxCollectionElements = collectionElementEnum switch
            {
                ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                _ => throw new NotImplementedException(),
            };

            for (int page = 1; page <= pageMax; page++)
            {
                await AppendCollectionElementPage(collectionElementEnum, page);
            }

            return;
        }

        int pageCurrent = _PanelCollectionViewerContext?.PageCurrent ?? 1;
        int pageMaxSingle = _PanelCollectionViewerContext?.PageMax ?? 1;

        if (pageCurrent >= pageMaxSingle)
        {
            MaxCollectionElements = collectionElementEnum switch
            {
                ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                _ => throw new NotImplementedException(),
            };
        }

        await AppendCollectionElementPage(collectionElementEnum, pageCurrent);
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

    private async UniTask AppendCollectionElementPage(ECollectionElement collectionElementEnum, int page)
    {
        IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = collectionElementEnum switch
        {
            ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(page),
            ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(page),
            _ => throw new NotImplementedException(),
        };

        IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
            .Where(static a => a.List.Count() > 0)
            .OrderByDescending(static a => a.Priority);

        foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
        {
            _GroupDividers.Add(new(item, this));
        }

        await UniTask.Yield();
    }

    private async UniTask PagePrev()
    {
        if (PageCurrent > 1)
        {
            PageCurrent--;
            await NotifyPageChangedAsync();
        }
    }

    private async UniTask PageNext()
    {
        if (PageCurrent < PageMax)
        {
            PageCurrent++;
            await NotifyPageChangedAsync();
        }
    }

    private async UniTask NotifyPageChangedAsync()
    {
        if (_PanelCollectionTopButtonsContext != null)
        {
            await _PanelCollectionTopButtonsContext.OnPageChangedAsync(PageCurrent);
            _PanelCollectionTopButtonsContext.OnLayoutChanged();
        }
    }
}
