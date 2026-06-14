using Assets.GameData.Prefabs;
using System;
using System.Collections.Generic;

namespace Assets.GameData.Scenes.Collection
{
    public class CollectionSceneViewerContext : IPanelCollectionViewerContext
    {
        private PanelCollection__prefab__scriptMB _PanelCollection;

        public ECollectionMode CollectionMode => CollectionSceneInitializator.PanelSceneInstance.CollectionMode;

        public int PageCurrent => CollectionSceneInitializator.PanelCollectionInstance.PageCurrent;

        public int PageMax => CollectionSceneInitializator.PanelCollectionInstance.PageMax;

        public void OnCollectionLoaded(PanelCollection__prefab__scriptMB panelCollection, int maxCollectionElements)
        {
            _PanelCollection = panelCollection;
            CollectionSceneInitializator.PanelCollectionInstance.UpdatePageMax();
            CollectionSceneInitializator.PanelCollectionInstance.SetPageDiapason(maxCollectionElements);
        }

        public Guid? GetSelectedElementId(ECollectionMode collectionMode)
        {
            return collectionMode switch
            {
                ECollectionMode.Hero when CollectionSceneInitializator.PanelSelectedHeroInstance is { IsVisible: true } heroPanel
                    => heroPanel.HeroId,
                ECollectionMode.Equipment when CollectionSceneInitializator.PanelSelectedEquipmentInstance is { IsVisible: true } equipmentPanel
                    => equipmentPanel.EquipmentId,
                _ => null,
            };
        }

        public void OnElementSelected(Guid elementId, ECollectionMode collectionMode)
        {
            switch (collectionMode)
            {
                case ECollectionMode.Hero:
                    CollectionSceneInitializator.PanelSelectedHeroInstance.Show(elementId);
                    break;

                case ECollectionMode.Equipment:
                    CollectionSceneInitializator.PanelSelectedEquipmentInstance.Show(elementId);
                    break;

                default:
                    throw new NotImplementedException();
            }

            _PanelCollection?.GetElement(elementId)?.Selected(true, clearOthers: true);
        }

        public bool LoadAllPages => false;

        public bool ContextControlsRootSize => true;

        public List<Action> Actions => throw new NotImplementedException();

        public void OnLayoutChanged()
        {
            CollectionSceneInitializator.OnResized();
        }

        public (float width, float height) GetViewerSize()
        {
            float width = CollectionSceneInitializator.PanelCollectionInstance.Width;
            float height = CollectionSceneInitializator.PanelCollectionInstance.Height
                - CollectionSceneInitializator.PanelCollectionInstance.TopButtonsHeight;
            return (width, height);
        }
    }
}
