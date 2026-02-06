using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionViewer
    {
        private const float SCROLLBAR_WIDTH = 32f;
        private const float VIEWPORT_CONTENT_SPACING = 5f;

        public PanelCollectionViewer(PanelCollection panelCollection)
        {
            PanelCollection = panelCollection;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
            _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
            CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;
            _Content_VerticalLayoutGroup = CollectionContent_Transform.GetComponent<VerticalLayoutGroup>();
            _PanelCollectionTopButtons = panelCollection.PanelCollectionTopButtons;
        }

        public PanelCollection PanelCollection { get; }
        public Transform CollectionContent_Transform { get; }
        public int MaxCollectionElements { get; private set; }
        public float Width { get; private set; }

        private readonly PanelCollectionTopButtons _PanelCollectionTopButtons;
        private readonly RectTransform _RectTransform;
        private readonly RectTransform _ScrollbarVertical_RectTransform;
        private readonly VerticalLayoutGroup _Content_VerticalLayoutGroup;
        private readonly List<PanelGroupDivider> _GroupDividers = new();

        public async UniTask InstantiateCollectionAsync()
        {
            try
            {
                _GroupDividers.ForEach(a => a.Destroy());
                _GroupDividers.Clear();

                PanelCollection.PanelScene.OnResized();
                await UniTask.Yield();

                MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * _PanelCollectionTopButtons.PageCurrent;

                _PanelCollectionTopButtons.UpdatePageMax();

                switch (PanelCollection.PanelScene.CollectionMode)
                {
                    case CollectionModeEnum.Hero:
                        PanelCollection.PanelScene.PanelSelectedEquipment.Hide();
                        await LoadCollectionElement(CollectionElementEnum.Hero); break;

                    case CollectionModeEnum.Equipment:
                        PanelCollection.PanelScene.PanelSelectedHero.Hide();
                        await LoadCollectionElement(CollectionElementEnum.Equipment); break;

                    case CollectionModeEnum.ChangingEquipment:
                        {
                            bool h = PanelCollection.PanelScene.PanelSelectedHero.IsVisible;
                            //bool e = PanelCollection.PanelScene.PanelSelectedEquipment.IsVisible;
                            if (h)
                            {
                                await LoadCollectionElement(CollectionElementEnum.Equipment);
                            }
                            else
                            {
                                await LoadCollectionElement(CollectionElementEnum.Hero);
                            }
                            break;
                        }

                    default:
                        throw new Exception();
                }

                _PanelCollectionTopButtons.SetPageDiapason();
                OnResized();
            }
            finally
            {
                GameMessage.Close();
            }
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();

            Width = PanelCollection.Width;
            float height = PanelCollection.Height - _PanelCollectionTopButtons.Height;

            _RectTransform.sizeDelta.Set(Width, height);

            // ScrollbarVertical для коллекции героев
            float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;
            _ScrollbarVertical_RectTransform.sizeDelta.Set(scrollBarWidth, height);

            // Scroll View для коллекции героев
            float viewportWidth = Width - scrollBarWidth;
            _RectTransform.sizeDelta.Set(viewportWidth, height);

            _Content_VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

            // groupDividers
            if (_GroupDividers.Count > 0)
            {
                _GroupDividers.ForEach(a => a.OnResized());
            }
        }

        public void UnselectAll() {
            _GroupDividers.ForEach(a => a.UnselectAll());
        }
        private async UniTask LoadCollectionElement(CollectionElementEnum collectionElementEnum)
        {
            if (_PanelCollectionTopButtons.PageCurrent >= _PanelCollectionTopButtons.PageMax)
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
                CollectionElementEnum.Hero => Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(_PanelCollectionTopButtons.PageCurrent),
                CollectionElementEnum.Equipment => Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(_PanelCollectionTopButtons.PageCurrent),
                _ => throw new NotImplementedException(),
            };

            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped
                .Where(static a => a.List.Count() > 0)
                .OrderByDescending(static a => a.Priority);

            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                _GroupDividers.Add(new(this, item));
            }
        }
    }
}
