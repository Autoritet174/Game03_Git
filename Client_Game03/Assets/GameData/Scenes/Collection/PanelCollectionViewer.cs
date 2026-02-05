using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionViewer
    {
        private const float SCROLLBAR_WIDTH = 32f;
        private const float VIEWPORT_CONTENT_SPACING = 5f;
        public PanelCollection PanelCollection { get; }
        public PanelCollectionViewer(PanelCollection panelCollection ) {
            PanelCollection = panelCollection;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
            _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
            _CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;
            _Content_VerticalLayoutGroup = _CollectionContent_Transform.GetComponent<VerticalLayoutGroup>();
        }

        private readonly RectTransform _RectTransform;
        private readonly RectTransform _ScrollbarVertical_RectTransform;
        private readonly Transform _CollectionContent_Transform;
        private readonly VerticalLayoutGroup _Content_VerticalLayoutGroup;

        private readonly List<GroupDivider> _GroupDividers = new();
        public int MaxCollectionElements { get; private set; }
        public async UniTask InstantiateCollectionAsync()
        {
            try
            {
                if (_GroupDividers.Count > 0)
                {
                    foreach (GroupDivider item in _GroupDividers)
                    {
                        UnityEngine.Object.Destroy(item.gameObject);
                    }
                }

                PanelCollection.PanelScene.OnResized();
                await UniTask.Yield();

                MaxCollectionElements = Game03Client.Collection.CollectionProvider.PAGE_SIZE * PanelCollection.PanelCollectionTopButtons.PageCurrent;

                PanelCollection.PanelCollectionTopButtons.UpdatePageMax();
                _GroupDividers.Clear();

                switch (PanelCollection.PanelScene.CollectionMode)
                {
                    case CollectionMode.Hero:
                        PanelCollection.PanelScene.PanelSelectedEquipment.Hide();
                        await LoadHeroes(); break;
                    case CollectionMode.Equipment:
                        PanelCollection.PanelScene.PanelSelectedHero.Hide();
                        await LoadEquipmentes(); break;
                    case CollectionMode.ChangingEquipment:
                        {
                            bool h = PanelCollection.PanelScene.PanelSelectedHero.IsVisible;
                            //bool e = PanelCollection.PanelScene.PanelSelectedEquipment.IsVisible;
                            if (h)
                            {
                                await LoadEquipmentes();
                            }
                            else
                            {
                                await LoadHeroes();
                            }
                            break;
                        }

                    default:
                        throw new Exception();
                }

                PanelCollection.PanelCollectionTopButtons.SetPageDiapason();
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

            float width = PanelCollection.Width;
            float height = PanelCollection.Height - PanelCollection.PanelCollectionTopButtons.Height;

            _RectTransform.sizeDelta.Set(width, height);

            // ScrollbarVertical для коллекции героев
            float scrollBarWidth = SCROLLBAR_WIDTH * coefHeight;
            _ScrollbarVertical_RectTransform.sizeDelta.Set(scrollBarWidth, height);

            // Scroll View для коллекции героев
            float viewportWidth = width - scrollBarWidth;
            _RectTransform.sizeDelta.Set(viewportWidth, height);

            _Content_VerticalLayoutGroup.spacing = VIEWPORT_CONTENT_SPACING * coefHeight;

            // groupDividers
            if (_GroupDividers.Count > 0)
            {
                _GroupDividers.ForEach(a => a.Resize());
            }
        }

        private async UniTask LoadHeroes()
        {
            if (PanelCollection.PanelCollectionTopButtons.PageCurrent >= PanelCollection.PanelCollectionTopButtons.PageMax)
            {
                MaxCollectionElements = Game03Client.Collection.CollectionProvider.GetCountHeroes();
            }

            IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(PanelCollection.PanelCollectionTopButtons.PageCurrent);
            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                if (item.List.Count() > 0)
                {
                    GameObject obj = AddressableCache.GroupDividerPrefabAddressableGameObject.SafeInstant();
                    obj.transform.SetParent(_CollectionContent_Transform, false);
                    //GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                    GroupDivider groupDivider = new(this, item.Name);
                    _GroupDividers.Add(groupDivider);
                    //await groupDivider.Init(item.Name, this, obj, item.List);
                }
            }
        }
        private async UniTask LoadEquipmentes()
        {
            if (PanelCollection.PanelCollectionTopButtons.PageCurrent >= PanelCollection.PanelCollectionTopButtons.PageMax)
            {
                MaxCollectionElements = Game03Client.Collection.CollectionProvider.GetCountEquipments();
            }

            IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(PanelCollection.PanelCollectionTopButtons.PageCurrent);
            IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
            foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
            {
                if (item.List.Count() > 0)
                {
                    GameObject obj = AddressableCache.GroupDividerPrefabAddressableGameObject.SafeInstant();
                    obj.transform.SetParent(_CollectionContent_Transform, false);
                    //GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                    GroupDivider groupDivider = new(this, item.Name);
                    _GroupDividers.Add(groupDivider);
                    //await groupDivider.Init(item.Name, this, obj, item.List);
                }
            }
        }
    }
}
