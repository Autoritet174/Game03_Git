using Assets.GameData.Prefabs;
using System;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionViewerContext : IPanelCollectionViewerContext
    {
        private CollectionSceneInitializator _CollectionSceneInitializator;
        public void OnCollectionLoaded(CollectionSceneInitializator collectionSceneInitializator)
        {
            _CollectionSceneInitializator = collectionSceneInitializator;
        }

        public void OnElementSelected(Guid elementId, ECollectionMode collectionMode)
        {
            switch (collectionMode)
            {
                case ECollectionMode.Hero:
                    _CollectionSceneInitializator.PanelSelectedHero__context.Show(elementId);
                    //_CollectionSceneInitializator.OnResized();
                    break;
                case ECollectionMode.Equipment:
                    _CollectionSceneInitializator.PanelSelectedEquipment__context.Show(elementId);
                    break;
            }
            _CollectionSceneInitializator.PanelCollection__prefab__context.GetElement(elementId)?.Selected(true, clearOthers: true);
        }
    }
}
