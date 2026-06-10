using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using I = CollectionSceneInitializator;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionViewer
    {
        private const float SCROLLBAR_WIDTH = 32f;
        private const float VIEWPORT_CONTENT_SPACING = 5f;

        public PanelCollectionViewer()
        {
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionViewer (id=ph1oh7dk)");
            _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
            CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;
            _Content_VerticalLayoutGroup = CollectionContent_Transform.GetComponent<VerticalLayoutGroup>();
        }
        public Transform CollectionContent_Transform { get; }
        public int MaxCollectionElements { get; private set; }
        public float Width { get; private set; }

        private readonly RectTransform _RectTransform;
        private readonly RectTransform _ScrollbarVertical_RectTransform;
        private readonly VerticalLayoutGroup _Content_VerticalLayoutGroup;
        private readonly List<PanelGroupDivider> _GroupDividers = new();
        private readonly Dictionary<Guid, PanelIconCollectionElement> _Elements = new();

        public void AddElement(PanelIconCollectionElement e)
        {
            _Elements.Add(e.Id, e);
        }

        public PanelIconCollectionElement GetElement(Guid id)
        {
            return _Elements.TryGetValue(id, out PanelIconCollectionElement element) ? element : null;
        }


        public async UniTask InstantiateCollectionAsync()
        {
            try
            {
                _GroupDividers.ForEach(a => a.Destroy());
                _GroupDividers.Clear();
                _Elements.Clear();

                I.OnResized();
                await UniTask.Yield();

                MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * I.PanelCollectionTopButtonsInstance.PageCurrent;

                I.PanelCollectionTopButtonsInstance.UpdatePageMax();

                switch (I.PanelSceneInstance.CollectionMode)
                {
                    case CollectionModeEnum.Hero:
                        await LoadCollectionElement(CollectionElementEnum.Hero);
                        if (I.PanelSelectedHeroInstance != null && I.PanelSelectedHeroInstance.IsVisible)
                        {
                            GetElement(I.PanelSelectedHeroInstance.HeroId)?.Selected(true);
                        }
                        break;

                    case CollectionModeEnum.Equipment:
                        await LoadCollectionElement(CollectionElementEnum.Equipment);
                        if (I.PanelSelectedEquipmentInstance != null && I.PanelSelectedEquipmentInstance.IsVisible)
                        {
                            GetElement(I.PanelSelectedEquipmentInstance.EquipmentId)?.Selected(true);
                        }
                        break;

                    default:
                        throw new Exception();
                }

                I.PanelCollectionTopButtonsInstance.SetPageDiapason();
                I.OnResized();
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

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();


            float height = I.PanelCollectionInstance.Height - I.PanelCollectionTopButtonsInstance.Height;

            _RectTransform.sizeDelta = new Vector2(I.PanelCollectionInstance.Width, height);

            // ScrollbarVertical для коллекции героев
            float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;
            _ScrollbarVertical_RectTransform.sizeDelta = new Vector2(scrollBarWidth, 0);

            Width = I.PanelCollectionInstance.Width - scrollBarWidth;


            _Content_VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

            // groupDividers
            if (_GroupDividers.Count > 0)
            {
                _GroupDividers.ForEach(a => a.OnResized());
            }
        }

        public void UnselectAll()
        {
            _GroupDividers.ForEach(a => a.UnselectAll());
        }

        private async UniTask LoadCollectionElement(CollectionElementEnum collectionElementEnum)
        {
            if (I.PanelCollectionTopButtonsInstance.PageCurrent >= I.PanelCollectionTopButtonsInstance.PageMax)
            {
                MaxCollectionElements = collectionElementEnum switch
                {
                    CollectionElementEnum.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                    CollectionElementEnum.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                    _ => throw new NotImplementedException(),
                };
            }

            IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = collectionElementEnum switch
            {
                CollectionElementEnum.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(I.PanelCollectionTopButtonsInstance.PageCurrent),
                CollectionElementEnum.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(I.PanelCollectionTopButtonsInstance.PageCurrent),
                _ => throw new NotImplementedException(),
            };

            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
                .Where(static a => a.List.Count() > 0)
                .OrderByDescending(static a => a.Priority);

            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                _GroupDividers.Add(new(item));
            }
        }
    }
}
