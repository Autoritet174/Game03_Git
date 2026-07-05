using Assets.GameData.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Background_BlueClouds_v1__prefab__scriptMB : MonoBehaviour
{
    private float _BackgroundImageCoef;
    private Image _Background_Image;

    public void Initialize()
    {
        _Background_Image = GameObjectFinder.FindByName<Image>("Background_Image");
        if (_Background_Image == null || _Background_Image.sprite == null)
        {
            throw new Exception("Изображение заднего фона некорректно.");
        }
        Texture2D texture = _Background_Image.sprite.texture;
        _BackgroundImageCoef = texture.width / (float)texture.height;
    }

    public void OnResized()
    {
        //_Width / _Height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
        // Изображение заднего фона
        _Background_Image.rectTransform.sizeDelta = Screen.width / Screen.height > _BackgroundImageCoef
            ? new Vector2(Screen.width, Screen.width / _BackgroundImageCoef)
            : new Vector2(Screen.height * _BackgroundImageCoef, Screen.height);
    }
}
