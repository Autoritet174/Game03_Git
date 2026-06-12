using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelCollectionViewer__prefab__scriptMB : MonoBehaviour
{
    [SerializeField]
    private float SCROLLBAR_WIDTH = 32f;

    [SerializeField]
    private float VIEWPORT_CONTENT_SPACING = 5f;

    private RectTransform _RectTransform;
    private RectTransform Viewport__RectTransform;
    private RectTransform _ScrollbarVertical__RectTransform;
    private VerticalLayoutGroup _Content__VerticalLayoutGroup;
    private readonly List<PanelGroupDivider__prefab__script> _GroupDividers = new();
    private readonly Dictionary<Guid, PanelIconCollectionElement> _Elements = new();

    public Transform Content_Transform { get; private set; }
    public int MaxCollectionElements { get; private set; }
    public float Width { get; private set; }

    private void Awake()
    {
        _RectTransform = gameObject.GetComponent<RectTransform>();
        _ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", gameObject.transform);
        Content_Transform = GameObjectFinder.FindByName<RectTransform>("Content", gameObject.transform);
        _Content__VerticalLayoutGroup = Content_Transform.GetComponent<VerticalLayoutGroup>();
        Viewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", gameObject.transform);
    }

    public IPanelCollectionViewerContext Context { get; private set; }

    public void SetContext(IPanelCollectionViewerContext context)
    {
        Context = context;
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

            int pageCurrent = Context?.PageCurrent ?? 1;
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

            Context?.OnCollectionLoaded(this, MaxCollectionElements);
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

    private void RestoreSelection(ECollectionMode collectionMode)
    {
        Guid? selectedId = Context?.GetSelectedElementId(collectionMode);
        if (selectedId.HasValue)
        {
            GetElement(selectedId.Value)?.Selected(true);
        }
    }

    private async UniTask LoadCollectionElement(ECollectionElement collectionElementEnum)
    {
        if (Context?.LoadAllPages == true)
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

        int pageCurrent = Context?.PageCurrent ?? 1;
        int pageMaxSingle = Context?.PageMax ?? 1;

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

    public void OnResized()
    {
        float coefHeight = G.GetCoefHeight();
        float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;

        if (Context != null && Context.ContextControlsRootSize)
        {
            (float width, float height) = Context.GetViewerSize();
            _RectTransform.sizeDelta = new Vector2(width, height);
            Width = width - scrollBarWidth;
        }
        else
        {
            Width = _RectTransform.rect.width - scrollBarWidth;
        }

        _ScrollbarVertical__RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);
        Viewport__RectTransform.SetRight(scrollBarWidth);
        _Content__VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.OnResized());
        }
    }
}
