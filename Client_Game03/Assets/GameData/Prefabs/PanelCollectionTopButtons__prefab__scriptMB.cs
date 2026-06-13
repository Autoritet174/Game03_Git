using Assets.GameData.Prefabs;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelCollectionTopButtons__prefab__scriptMB : MonoBehaviour
{
    public const float HEIGHT = 113f;
    private const float RANGE_PANEL_WIDTH = 230f;
    private const float RANGE_PANEL_HEIGHT = 90f;
    private const float BUTTON_PAGE_WIDTH = 100f;
    private const float BUTTON_PAGE_HEIGHT = 60f;
    private const float LABEL_HEIGHT = 30f;
    private const float LABEL_FONTSIZE = 18f;

    private const string FilterButtonHeroesName = "ImageButtonHeroes (id=pakco5ud)";
    private const string FilterButtonEquipmentsName = "ImageButtonEquipments (id=vuhjngaz)";
    private const string FilterButtonFilterName = "ImageButtonFilter (id=vjeqfzen)";
    private const string FilterButtonGroupName = "ImageButtonGroup (id=hbsaogwl)";
    private const string FilterButtonSortName = "ImageButtonSort (id=6nvcsrdm)";
    private const string PanelRangeName = "PanelRange (id=66z5bnzi)";
    private const string ButtonPrevPageName = "ButtonPrevPage (id=25alql62)";
    private const string ButtonNextPageName = "ButtonNextPage (id=k5moi57b)";
    private const string LabelRangePageName = "LabelRangePage (id=6jgz12bu)";

    private RectTransform _RectTransform;
    private IPanelCollectionTopButtonsContext _Context;

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

    public int PageCurrent { get; private set; } = 1;
    public int PageMax { get; private set; } = 1;
    public float Height { get; private set; }

    public IPanelCollectionTopButtonsContext Context => _Context;

    private void Awake()
    {
        _RectTransform = GetComponent<RectTransform>();

        _FilterButtonHeroes = new(FilterButtonHeroesName, transform);
        _FilterButtonEquipments = new(FilterButtonEquipmentsName, transform);
        _FilterButtonFilter = new(FilterButtonFilterName, transform);
        _FilterButtonGroup = new(FilterButtonGroupName, transform);
        _FilterButtonSort = new(FilterButtonSortName, transform);

        _RangePanel_RectTransform = GameObjectFinder.FindByName<RectTransform>(PanelRangeName, gameObject);
        _ButtonPrevPage_RectTransform = GameObjectFinder.FindByName<RectTransform>(ButtonPrevPageName, gameObject);
        _ButtonNextPage_RectTransform = GameObjectFinder.FindByName<RectTransform>(ButtonNextPageName, gameObject);
        _ButtonPrevPage_RectTransform.gameObject.SetClickEvent(PagePrev, true);
        _ButtonNextPage_RectTransform.gameObject.SetClickEvent(PageNext, true);

        _LabelRangePage_RectTransform = GameObjectFinder.FindByName<RectTransform>(LabelRangePageName, gameObject);
        _LabelRangePage_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(LabelRangePageName, gameObject);
    }

    public void SetContext(IPanelCollectionTopButtonsContext context)
    {
        _Context = context;
    }

    public void SetPageDiapason(int maxCollectionElements)
    {
        _LabelRangePage_TextMeshProUGUI.text = $"{((PageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {maxCollectionElements}";
    }

    public void ResetPageCurrent()
    {
        PageCurrent = 1;
    }

    public void OnResized()
    {
        float coefHeight = G.GetCoefHeight();
        Height = HEIGHT * coefHeight;

        if (_Context != null && _Context.ContextControlsRootSize)
        {
            _RectTransform.sizeDelta = new Vector2(_Context.GetPanelWidth(), Height);
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

    public void UpdatePageMax()
    {
        if (_Context == null)
        {
            PageMax = 1;
            return;
        }

        int count = _Context.GetCollectionCount(_Context.CollectionMode);
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
        if (_Context != null)
        {
            await _Context.OnPageChangedAsync(PageCurrent);
            _Context.OnLayoutChanged();
        }
    }
}
