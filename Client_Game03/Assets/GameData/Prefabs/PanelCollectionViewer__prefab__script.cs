using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelCollectionViewer__prefab__script : MonoBehaviour
{
    [SerializeField]
    private float SCROLLBAR_WIDTH = 32f;

    [SerializeField]
    private float VIEWPORT_CONTENT_SPACING = 5f;

    private RectTransform _RectTransform;
    private RectTransform _ScrollbarVertical_RectTransform;
    private VerticalLayoutGroup _Content_VerticalLayoutGroup;
    private readonly List<PanelGroupDivider__prefab__script> _GroupDividers = new();
    private readonly Dictionary<Guid, PanelIconCollectionElement> _Elements = new();
    private Transform _Content_Transform;
    public int MaxCollectionElements { get; private set; }
    public float Width { get; private set; }

    private void Start()
    {
        _RectTransform = gameObject.GetComponent<RectTransform>();
        _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", gameObject.transform);
        _Content_Transform = GameObjectFinder.FindByName("Content", gameObject.transform).transform;
        _Content_VerticalLayoutGroup = _Content_Transform.GetComponent<VerticalLayoutGroup>();
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

            MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * CollectionSceneInitializator.PanelCollectionTopButtonsInstance.PageCurrent;

            CollectionSceneInitializator.PanelCollectionTopButtonsInstance.UpdatePageMax();

            switch (collectionMode)
            {
                case ECollectionMode.Hero:
                    await LoadCollectionElement(ECollectionElement.Hero);
                    if (CollectionSceneInitializator.PanelSelectedHeroInstance != null && CollectionSceneInitializator.PanelSelectedHeroInstance.IsVisible)
                    {
                        GetElement(CollectionSceneInitializator.PanelSelectedHeroInstance.HeroId)?.Selected(true);
                    }
                    break;

                case ECollectionMode.Equipment:
                    await LoadCollectionElement(ECollectionElement.Equipment);
                    if (CollectionSceneInitializator.PanelSelectedEquipmentInstance != null && CollectionSceneInitializator.PanelSelectedEquipmentInstance.IsVisible)
                    {
                        GetElement(CollectionSceneInitializator.PanelSelectedEquipmentInstance.EquipmentId)?.Selected(true);
                    }
                    break;

                default:
                    throw new Exception();
            }

            CollectionSceneInitializator.PanelCollectionTopButtonsInstance.SetPageDiapason();
            CollectionSceneInitializator.OnResized();
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



    private async UniTask LoadCollectionElement(ECollectionElement collectionElementEnum)
    {
        if (CollectionSceneInitializator.PanelCollectionTopButtonsInstance.PageCurrent >= CollectionSceneInitializator.PanelCollectionTopButtonsInstance.PageMax)
        {
            MaxCollectionElements = collectionElementEnum switch
            {
                ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                _ => throw new NotImplementedException(),
            };
        }

        IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = collectionElementEnum switch
        {
            ECollectionElement.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(CollectionSceneInitializator.PanelCollectionTopButtonsInstance.PageCurrent),
            ECollectionElement.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(CollectionSceneInitializator.PanelCollectionTopButtonsInstance.PageCurrent),
            _ => throw new NotImplementedException(),
        };

        IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
            .Where(static a => a.List.Count() > 0)
            .OrderByDescending(static a => a.Priority);

        foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
        {
            _GroupDividers.Add(new(item, _Content_Transform));
        }
    }

    public void OnResized()
    {
        float coefHeight = G.GetCoefHeight();


        float height = CollectionSceneInitializator.PanelCollectionInstance.Height - CollectionSceneInitializator.PanelCollectionTopButtonsInstance.Height;

        _RectTransform.sizeDelta = new Vector2(CollectionSceneInitializator.PanelCollectionInstance.Width, height);

        // ScrollbarVertical для коллекции героев
        float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;
        _ScrollbarVertical_RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);

        Width = CollectionSceneInitializator.PanelCollectionInstance.Width - scrollBarWidth;


        _Content_VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

        // groupDividers
        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.OnResized());
        }
    }
}
