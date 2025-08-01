using Assets.GameData.Scripts;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    private static List<HeroBaseEntity> allHeroes = new();
    public GameObject content;

    public GameObject prefabIconHero;


    private async void Start()
    {
        if (allHeroes.Count == 0) {
             await FullListAllHeroes();
        }
        AddAllImageOnContent();
    }


    private async Task FullListAllHeroes()
    {
        JObject jsonObject = await HttpRequester.GetResponceAsync(General.URLs.Uri_GetListAllHeroes);

        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        JToken heroesToken = jsonObject["heroes"];
        if (heroesToken is not JArray heroesArray)
            throw new InvalidOperationException("Ключ 'heroes' отсутствует или не является массивом.");

        allHeroes.Clear();
        foreach (JObject heroObj in heroesArray.Cast<JObject>())
        {
            string name = heroObj["name"]?.ToString();
            //Debug.Log(name);
            allHeroes.Add(new HeroBaseEntity(name));
        }
    }


    private void AddAllImageOnContent()
    {
        foreach (HeroBaseEntity heroStats in allHeroes)
        {
            GameObject _prefabIconHero = Instantiate(prefabIconHero);

            Transform transform = _prefabIconHero.transform;
            transform.SetParent(content.transform, false); // Устанавливаем родительский объект (Content)

            // Изображение
            Transform childImage = _prefabIconHero.transform.Find("ImageHero");
            if (childImage != null && childImage.TryGetComponent(out UnityEngine.UI.Image image))
            {
                //Color randomColor = new(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                //image.color = randomColor;

                // Загрузка PNG из Resources
                Sprite loadedSprite = Resources.Load<Sprite>($"Images/Heroes/{heroStats.Name}");

                if (loadedSprite != null)
                {
                    image.sprite = loadedSprite; // Устанавливаем загруженный спрайт
                    //image.color = Color.white;   // Сбрасываем цвет в белый (убираем случайный)
                }
                else
                {
                    Debug.LogError("Не удалось загрузить изображение warrior.png из Resources!");
                    // Запасной вариант - случайный цвет
                    image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                }
            }

            // Текст
            Transform childText = _prefabIconHero.transform.Find("TextHero");
            if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
            {
                textMeshPro.text = heroStats.Name;
            }
        }
    }
}
