using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
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
            Width = Screen.width - PanelScene.PanelSelectedHero.Width - PanelScene.PanelSelectedEquipment.Width;

            _RectTransform.sizeDelta.Set(Width, Height);

            PanelCollectionTopButtons.OnResized();
            PanelCollectionViewer.OnResized();
        }
    }
}
