using Assets.GameData.Scripts;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollection
    {
        public PanelCollection(PanelScene panelScene)
        {
            PanelScene = panelScene;
            PanelCollectionTopButtons = new(this);
            PanelCollectionViewer = new(this);
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
        }
        public PanelScene PanelScene { get; private set; }
        public PanelCollectionTopButtons PanelCollectionTopButtons { get; }
        public PanelCollectionViewer PanelCollectionViewer { get; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        private readonly RectTransform _RectTransform;


        public void OnResized()
        {
            Height = Screen.height - PanelScene.PanelTop.Height;
            float w1 = PanelScene.PanelSelectedEquipment.Width > 0 ? PanelScene.PanelSelectedEquipment.Width + (PanelSelectedEquipment.WIDTH_SPACING * 2) : 0;
            Width = Screen.width - PanelScene.PanelSelectedHero.Width - w1;

            _RectTransform.sizeDelta = new Vector2(Width, Height);

            PanelCollectionTopButtons.OnResized();
            PanelCollectionViewer.OnResized();
        }
    }
}
