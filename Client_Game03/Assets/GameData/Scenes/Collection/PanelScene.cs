using Assets.GameData.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Collection
{
    public enum CollectionModeEnum { Hero, Equipment//, ChangingEquipment
    }
    public enum CollectionElementEnum { Hero, Equipment }

    public class PanelScene : MonoBehaviour
    {
        private async void Start()
        {
            // Изображение заднего фона
            _Background_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=688x18dt)");
            if (_Background_Image != null && _Background_Image.sprite != null)
            {
                Texture2D texture = _Background_Image.sprite.texture;
                _ImageBackgroundCoef = texture.width / (float)texture.height;
            }
            else
            {
                throw new Exception("Изображение заднего фона некорректно.");
            }
            PanelTop = new(this);
            PanelCollection = new(this);
            PanelSelectedHero = new(this);
            PanelSelectedEquipment = new(this);
            await PanelCollection.PanelCollectionViewer.InstantiateCollectionAsync();
            initialized = true;
            OnResized();
        }
        public PanelTop PanelTop { get; private set; }
        public PanelCollection PanelCollection { get; private set; }
        public PanelSelectedHero PanelSelectedHero { get; private set; }
        public PanelSelectedEquipment PanelSelectedEquipment { get; private set; }

        public CollectionModeEnum CollectionMode { get; set; } = CollectionModeEnum.Hero;

        private float _Width, _Height;
        private float _ImageBackgroundCoef;
        private Image _Background_Image;
        private bool initialized = false;

        public void OnResized()
        {
            if (!initialized)
            {
                return;
            }

            _Height = Screen.height;
            _Width = Screen.width;

            // Изображение заднего фона
            if (_Width / _Height > _ImageBackgroundCoef)//_Width / _Height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
            {
                _Background_Image.rectTransform.sizeDelta = new Vector2(_Width, _Width / _ImageBackgroundCoef);
            }
            else
            {
                _Background_Image.rectTransform.sizeDelta = new Vector2(_Height * _ImageBackgroundCoef, _Height);
            }

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
