//using Assets.GameData.Prefabs;
//using System;
//using System.Collections.Generic;

//namespace Assets.GameData.Scenes.Collection
//{
//    public class PanelCollectionViewerContext : IPanelCollectionViewerContext
//    {
//        public int PageCurrent => CollectionSceneInitializator.PanelCollection__prefab__context.PageCurrent;

//        public int PageMax => CollectionSceneInitializator.PanelCollection__prefab__context.PageMax;

//        public void OnCollectionLoaded(PanelCollection__prefab__scriptMB panelCollection, int maxCollectionElements)
//        {
//            _PanelCollection = panelCollection;
//            CollectionSceneInitializator.PanelCollection__prefab__context.PanelTopButtons_UpdatePageMax();
//            //CollectionSceneInitializator.PanelCollection__prefab__context.TopButtons_SetPageDiapason(maxCollectionElements);
//        }

//        public Guid? GetSelectedElementId(ECollectionMode collectionMode)
//        {
//            return collectionMode switch
//            {
//                ECollectionMode.Hero when CollectionSceneInitializator.PanelSelectedHero__context is { IsVisible: true } heroPanel
//                    => heroPanel.HeroId,
//                ECollectionMode.Equipment when CollectionSceneInitializator.PanelSelectedEquipment__context is { IsVisible: true } equipmentPanel
//                    => equipmentPanel.EquipmentId,
//                _ => null,
//            };
//        }

//        public void OnElementSelected(Guid elementId, ECollectionMode collectionMode)
//        {
//            switch (collectionMode)
//            {
//                case ECollectionMode.Hero:
//                    CollectionSceneInitializator.PanelSelectedHero__context.Show(elementId);
//                    break;

//                case ECollectionMode.Equipment:
//                    CollectionSceneInitializator.PanelSelectedEquipment__context.Show(elementId);
//                    break;

//                default:
//                    throw new NotImplementedException();
//            }

//            _PanelCollection?.GetElement(elementId)?.Selected(true, clearOthers: true);
//        }

//        public bool LoadAllPages => false;

//        public bool ContextControlsRootSize => true;

//        public List<Action> Actions => throw new NotImplementedException();

//        public void OnLayoutChanged()
//        {
//            IPrefab.OnResized();
//        }

//        public (float width, float height) GetViewerSize()
//        {
//            float width = CollectionSceneInitializator.PanelCollection__prefab__context.Width;
//            float height = CollectionSceneInitializator.PanelCollection__prefab__context.Height
//                - CollectionSceneInitializator.PanelCollection__prefab__context.PanelTopButtons_Height;
//            return (width, height);
//        }
//    }
//}
