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
            _PanelScene = panelScene;
            PanelCollectionTopButtons = new(this);
            PanelCollectionViewer = new(this);
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
        }
        public PanelCollectionTopButtons PanelCollectionTopButtons { get; }
        public PanelCollectionViewer PanelCollectionViewer { get; }

        public RectTransform _RectTransform { get; private set; }
        private readonly PanelScene _PanelScene;

        private RectTransform _ButtonPrevPage_RectTransform;
        private RectTransform _ButtonNextPage_RectTransform;
        private RectTransform _LabelRangePage_RectTransform;
        private TextMeshProUGUI _LabelRangePage_TextMeshProUGUI;

        public int PageCurrent { get; set; } = 1;
        public int PageMax { get; set; } = 1;
        void Init()
        {

            _ButtonPrevPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonPrevPage (id=25alql62)");
            _ButtonNextPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonNextPage (id=k5moi57b)");
            _LabelRangePage_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelRangePage (id=6jgz12bu)");
            _LabelRangePage_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelRangePage (id=6jgz12bu)");
            _ButtonPrevPage_RectTransform.gameObject.SetClickEvent(PagePrev, true);
            _ButtonNextPage_RectTransform.gameObject.SetClickEvent(PageNext, true);
        }
        private async UniTask PagePrev()
        {
            if (PageCurrent > 1)
            {
                PageCurrent--;
                await PanelCollectionViewer.InstantiateCollectionAsync();
            }
        }
        private async UniTask PageNext()
        {
            if (PageCurrent < PageMax)
            {
                PageCurrent++;
                await PanelCollectionViewer.InstantiateCollectionAsync();
            }
        }

        public float Width { get; set; }
        public float Height { get; set; }
        public void OnResized()
        {
            float coefHeight = Screen.height / 1080f;
            Height = (1080f - PanelTop.HEIGHT_BASE) * coefHeight;
            Width = (1920f-((_PanelScene.PanelSelectedHero.Visible ? PanelSelectedHero .WIDTH_BASE: 0)
                    + (_PanelScene.PanelSelectedEquipment.Visible ? PanelSelectedEquipment.WIDTH_BASE : 0)))* coefHeight;
            _RectTransform.sizeDelta = new(Width, Height);
            PanelCollectionTopButtons.OnResized();
            PanelCollectionViewer.OnResized();
        }
    }
}
