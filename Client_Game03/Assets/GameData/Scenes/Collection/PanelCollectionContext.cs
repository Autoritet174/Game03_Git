using Assets.GameData.Prefabs;
using System;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionContext : IPanelCollectionContext
    {
        private CollectionSceneInitializator collectionSceneInitializator;
        public void OnCollectionLoaded(CollectionSceneInitializator collectionSceneInitializator)
        {
            this.collectionSceneInitializator = collectionSceneInitializator;
        }

        public void OnClick(Guid elementId, ECollectionMode collectionMode)
        {
            switch (collectionMode)
            {
                case ECollectionMode.Hero:
                    collectionSceneInitializator.PanelSelectedHero__context.Show(elementId);
                    //_CollectionSceneInitializator.OnResized();
                    break;
                case ECollectionMode.Equipment:
                    collectionSceneInitializator.PanelSelectedEquipment__context.Show(elementId);
                    break;
            }
            collectionSceneInitializator.PanelCollection__prefab__context.GetElement(elementId)?.SetSelected(true, clearOthers: true);
        }
    }
}
