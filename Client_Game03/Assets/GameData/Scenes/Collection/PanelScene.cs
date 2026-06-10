using Assets.GameData.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;
using I = CollectionSceneInitializator;

namespace Assets.GameData.Scenes.Collection
{
    public enum ECollectionMode
    {
        Hero, Equipment
    }

    public enum ECollectionElement { Hero, Equipment }

    public class PanelScene
    {
        public PanelScene()
        {
            // Изображение заднего фона
            _Background_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=688x18dt)");
            if (_Background_Image == null || _Background_Image.sprite == null)
            {
                throw new Exception("Изображение заднего фона некорректно.");
            }
            else
            {
                Texture2D texture = _Background_Image.sprite.texture;
                _ImageBackgroundCoef = texture.width / (float)texture.height;
            }
        }

        public ECollectionMode CollectionMode { get; set; } = ECollectionMode.Hero;

        private readonly float _ImageBackgroundCoef;
        private readonly Image _Background_Image;

        public void OnResized()
        {
            // Изображение заднего фона
            if (I.Width / I.Height > _ImageBackgroundCoef)//_Width / _Height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
            {
                _Background_Image.rectTransform.sizeDelta = new Vector2(I.Width, I.Width / _ImageBackgroundCoef);
            }
            else
            {
                _Background_Image.rectTransform.sizeDelta = new Vector2(I.Height * _ImageBackgroundCoef, I.Height);
            }
        }

    }
}
