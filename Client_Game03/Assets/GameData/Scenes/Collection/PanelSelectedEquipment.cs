using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelSelectedEquipment
    {
        public PanelSelectedEquipment(PanelScene panelScene)
        {
            _PanelScene = panelScene;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipment (id=ta39338e)");
            _GameObject = _RectTransform.gameObject;
        }

        /// <summary>
        /// Ширина панели при разрешении 1920x1080.
        /// </summary>
        public const float WIDTH_BASE = 535f;
        public PanelScene _PanelScene;
        public Guid SelectedEquipmentId { get; private set; }
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                _GameObject.SetActive(value);
                if (value)
                {
                    SelectedEquipmentId = Guid.Empty;
                }

                _PanelScene.OnResized();
            }
        }
        private bool visible;
        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;
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
            _RectTransform.sizeDelta = new(w, h);
        }
    }
}
