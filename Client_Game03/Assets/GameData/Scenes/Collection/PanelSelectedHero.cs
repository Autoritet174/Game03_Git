using Assets.GameData.Scripts;
using System;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelSelectedHero
    {
        public PanelSelectedHero(PanelScene panelScene)
        {
            _PanelScene = panelScene;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
            _GameObject = _RectTransform.gameObject;
        }
        /// <summary>
        /// Ширина панели при разрешении 1920x1080.
        /// </summary>
        public const float WIDTH_BASE = 535f;
        public PanelScene _PanelScene;

        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                _GameObject.SetActive(value);
                if (value)
                {
                    SelectedHeroId = Guid.Empty;
                }

                _PanelScene.OnResized();
            }
        }
        private bool visible;
        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;
        public Guid SelectedHeroId { get; private set; }

        public void OnResized()
        {
            if (!Visible)
            {
                return;
            }

            float coefHeight = Screen.height / 1080f;
            float w = WIDTH_BASE * coefHeight;
            float h = (1080f - PanelTop.HEIGHT_BASE) * coefHeight;
            _RectTransform.sizeDelta = new(w, h);
        }
    }
}
