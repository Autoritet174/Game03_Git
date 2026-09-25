using Assets.GameData.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Подгоняет фоновое изображение под размеры экрана.</summary>
public class Background_BlueClouds_v1__prefab__scriptMB : MonoBehaviour
{
    private float backgroundImageCoef;
    private Image background_Image;

    public void Initialize()
    {
        background_Image = GameObjectFinder.FindByName<Image>("Background_Image");
        if (background_Image == null || background_Image.sprite == null)
        {
            throw new Exception("Изображение заднего фона некорректно.");
        }
        Texture2D texture = background_Image.sprite.texture;
        backgroundImageCoef = texture.width / (float)texture.height;
    }

    public void OnResized()
    {
        //_Width / _Height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
        // Изображение заднего фона
        background_Image.rectTransform.sizeDelta = Screen.width / Screen.height > backgroundImageCoef
            ? new Vector2(Screen.width, Screen.width / backgroundImageCoef)
            : new Vector2(Screen.height * backgroundImageCoef, Screen.height);
    }
}
