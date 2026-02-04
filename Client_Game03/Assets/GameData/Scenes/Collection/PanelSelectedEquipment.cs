using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

        public PanelScene _PanelScene;
        public Guid EquipmentId { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        /// <summary>
        /// Ширина панели при разрешении 1920x1080.
        /// </summary>
        private const float WIDTH_BASE = 535f;

        private bool visible;
        private readonly RectTransform _RectTransform;
        private readonly GameObject _GameObject;

        public void Show(Guid equipmentId) {
            visible = true;
            EquipmentId = equipmentId;
            _GameObject.SetActive(true);
            _PanelScene.OnResized();
        }

        public void Hide() {
            visible = false;
            EquipmentId = Guid.Empty;
            _GameObject.SetActive(false);
            _PanelScene.OnResized();
        }

        public void OnResized()
        {
            if (!visible)
            {
                return;
            }

            float coefHeight = Screen.height / 1080f;
            Width = WIDTH_BASE * coefHeight;
            Height = Screen.height - _PanelScene.PanelTop.Height;
            _RectTransform.sizeDelta.Set(Width, Height);
        }
    }
}
