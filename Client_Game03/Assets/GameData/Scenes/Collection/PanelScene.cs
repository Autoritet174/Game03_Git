using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public enum CollectionMode { Hero, Equipment, ChangingEquipment }

    public class PanelScene : MonoBehaviour
    {
        public PanelScene()
        {
            PanelTop = new(this);
            PanelCollection = new(this);
            PanelSelectedHero = new(this);
            PanelSelectedEquipment = new(this);
        }
        public PanelTop PanelTop { get; }
        public PanelCollection PanelCollection { get; }
        public PanelSelectedHero PanelSelectedHero { get; }
        public PanelSelectedEquipment PanelSelectedEquipment { get; }

        public static CollectionMode CollectionMode { get; set; } = CollectionMode.Hero;

        private float _Width, _Height;

        public void OnResized()
        {
            _Height = Screen.height;
            _Width = Screen.width;

            // Вызваем OnResized у всех дочерних объектов
            PanelTop.OnResized();
            PanelSelectedHero.OnResized();
            PanelSelectedEquipment.OnResized();
            PanelCollection.OnResized();
        }

        private void Update()
        {
            //bool resize = false;
            //if (!resize && (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width)))
            //{
            //    resize = true;
            //}
            //if (resize)
            //{
            //    OnResized();
            //}
            if (!Mathf.Approximately(Screen.height, _Height) || !Mathf.Approximately(Screen.width, _Width))
            {
                OnResized();
            }
        }

    }
}
