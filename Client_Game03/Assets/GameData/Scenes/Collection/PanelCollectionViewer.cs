using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionViewer
    {

        public PanelCollection PanelCollection { get; }
        public PanelCollectionViewer(PanelCollection panelCollection ) {
            PanelCollection = panelCollection;
        }


        private readonly List<GroupDivider> _GroupDividers = new();
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


                OnResized();
                await UniTask.Yield();


                int max = Game03Client.Collection.CollectionProvider.PAGE_SIZE * PanelCollection.PageCurrent;

                UpdatePageMax();
                _GroupDividers.Clear();

                async UniTask LoadHeroes()
                {
                    if (PanelCollection.PageCurrent >= PanelCollection.PageMax)
                    {
                        max = Game03Client.Collection.CollectionProvider.GetCountHeroes();
                    }

                    IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = Game03Client.Collection.CollectionProvider.GetCollectionHeroesGroupedByGroupNames(PanelCollection.PageCurrent);
                    IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
                    foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
                    {
                        if (item.List.Count() > 0)
                        {
                            GameObject obj = AddressableCache.GroupDividerPrefabAddressableGameObject.SafeInstant();
                            GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                            obj.transform.SetParent(_CollectionContent_Transform, false);
                            _GroupDividers.Add(groupDivider);
                            await groupDivider.Init(item.Name, this, obj, item.List);
                        }
                    }
                }
                async UniTask LoadEquipmentes()
                {
                    if (PanelCollection.PageCurrent >= PanelCollection.PageMax)
                    {
                        max = Game03Client.Collection.CollectionProvider.GetCountEquipments();
                    }

                    IEnumerable<Game03Client.Collection.GroupCollectionElement> grouped = Game03Client.Collection.CollectionProvider.GetCollectionEquipmentesGroupByGroups(PanelCollection.PageCurrent);
                    IOrderedEnumerable<Game03Client.Collection.GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
                    foreach (Game03Client.Collection.GroupCollectionElement item in sorted)
                    {
                        if (item.List.Count() > 0)
                        {
                            GameObject groupDividerPrefab = AddressableCache.GroupDividerPrefabAddressableGameObject;
                            GameObject obj = groupDividerPrefab.SafeInstant();
                            GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                            obj.transform.SetParent(_CollectionContent_Transform, false);
                            _GroupDividers.Add(groupDivider);
                            await groupDivider.Init(item.Name, this, obj, item.List);
                        }
                    }
                }


                switch (initializator.CollectionMode)
                {
                    case 1:
                        PanelSelectedEquipmentSetActive(false, false);
                        await LoadHeroes(); break;
                    case 2:
                        PanelSelectedHeroSetActive(false, false);
                        await LoadEquipmentes(); break;
                    case 3:
                        {
                            if (PanelSelectedHeroIsActive)
                            {
                                await LoadEquipmentes();
                            }
                            else
                            {
                                _FilterButtonEquipments.SetActive(false);
                                PanelSelectedEquipmentIsActive = false;
                                await LoadHeroes();
                            }
                            break;
                        }

                    default:
                        throw new Exception();
                }


                _LabelRangePage_TextMeshProUGUI.text = $"{((PanelCollection.PageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {max}";

                OnResized();
            }
            finally
            {
                GameMessage.Close();
            }
        }

        private void UpdatePageMax()
        {
            int c = PanelScene.CollectionMode switch
            {
                CollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                CollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                CollectionMode.ChangingEquipment => PanelSelectedHeroIsActive
                    ? Game03Client.Collection.CollectionProvider.GetCountEquipments()
                    : Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                _ => throw new Exception(),
            };
            PanelCollection.PageMax = (c / Game03Client.Collection.CollectionProvider.PAGE_SIZE) + (c % Game03Client.Collection.CollectionProvider.PAGE_SIZE > 0 ? 1 : 0);
            if (PanelCollection.PageMax < 1)
            {
                PanelCollection.PageMax = 1;
            }
            if (PanelCollection.PageCurrent > PanelCollection.PageMax)
            {
                PanelCollection.PageCurrent = PanelCollection.PageMax;
            }
            _RangePanel_GameObject.SetActive(PanelCollection.PageMax > 1);
        }

        public void OnResized()
        {

        }
    }
}
