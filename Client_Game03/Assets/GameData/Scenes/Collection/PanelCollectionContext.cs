using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection.prefabs;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionContext : IPanelCollectionContext
    {
        public bool ContextControlsRootSize => true;

        public (float width, float height) GetPanelSize()
        {
            float height = Screen.height - CollectionSceneInitializator.PanelTopInstance.Height;
            float equipmentPanelWidth = CollectionSceneInitializator.PanelSelectedEquipmentInstance.Width > 0
                ? CollectionSceneInitializator.PanelSelectedEquipmentInstance.Width + (PanelSelectedEquipment__prefab__scriptMB.WIDTH_SPACING * 2)
                : 0f;
            float width = Screen.width - CollectionSceneInitializator.PanelSelectedHeroInstance.Width - equipmentPanelWidth;
            return (width, height);
        }

        public void OnLayoutChanged()
        {
            CollectionSceneInitializator.OnResized();
        }
    }
}
