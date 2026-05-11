using Assets.GameData.Scripts;
using UnityEngine;
using I = CollectionSceneInitializator;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollection
    {
        public PanelCollection()
        {
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
        }

        public float Width { get; private set; }
        public float Height { get; private set; }

        private readonly RectTransform _RectTransform;

        public void OnResized()
        {
            Height = Screen.height - I.PanelTopInstance.Height;
            float w1 = I.PanelSelectedEquipmentInstance.Width > 0 ? I.PanelSelectedEquipmentInstance.Width + (PanelSelectedEquipment.WIDTH_SPACING * 2) : 0;
            Width = Screen.width - I.PanelSelectedHeroInstance.Width - w1;

            _RectTransform.sizeDelta = new Vector2(Width, Height);
        }
    }
}
