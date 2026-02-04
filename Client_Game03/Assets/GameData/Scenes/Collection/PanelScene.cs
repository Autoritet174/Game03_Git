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


        public float Width { get; set; }
        public float Height { get; set; }
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
            if (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width))
            {
                OnResized();
            }
        }

        public void OnResized()
        {
            Height = Screen.height;
            Width = Screen.width;

            // Вызваем OnResized у всех дочерних объектов
            PanelTop.OnResized();
            PanelSelectedHero.OnResized();
            PanelSelectedEquipment.OnResized();
            PanelCollection.OnResized();
        }
    }
}
