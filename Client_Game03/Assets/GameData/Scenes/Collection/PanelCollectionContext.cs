using Assets.GameData.Prefabs;
using System;

namespace Assets.GameData.Scenes.Collection
{
    /// <summary>Связывает выбор элементов коллекции с панелями текущей сцены.</summary>
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
                case ECollectionMode.hero:
                    collectionSceneInitializator.panelSelectedHero__context.Show(elementId);
                    //_CollectionSceneInitializator.OnResized();
                    break;
                case ECollectionMode.equipment:
                    collectionSceneInitializator.panelSelectedEquipment__context.Show(elementId);
                    break;
            }
            collectionSceneInitializator.panelCollection__prefab__context.GetElement(elementId)?.SetSelected(true, clearOthers: true);
        }
    }
}
