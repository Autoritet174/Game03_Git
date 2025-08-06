using Assets.GameData.Scripts;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class Initializer : MonoBehaviour
{
    private static readonly List<HeroBaseEntity> allHeroes = new();
    public GameObject content;

    public GameObject prefabIconHero;


    private async void Start()
    {
        if (allHeroes.Count == 0)
        {
            await FullListAllHeroes();
        }
        AddAllImageOnContent();
    }


    private async Task FullListAllHeroes()
    {
        JObject jsonObject = await HttpRequester.GetResponceAsync(General.URLs.Uri_GetListAllHeroes);

        if (jsonObject == null)
        {
            throw new ArgumentNullException(nameof(jsonObject));
        }

        JToken heroesToken = jsonObject["heroes"];
        if (heroesToken is not JArray heroesArray)
        {
            throw new InvalidOperationException("Ключ 'heroes' отсутствует или не является массивом.");
        }

        allHeroes.Clear();
        foreach (JObject heroObj in heroesArray.Cast<JObject>())
        {
            string name = heroObj["name"]?.ToString();
            //Debug.Log(name);
            allHeroes.Add(new HeroBaseEntity(name));
        }
    }


    //private void AddAllImageOnContent()
    //{
    //    foreach (HeroBaseEntity heroStats in allHeroes)
    //    {
    //        GameObject _prefabIconHero = Instantiate(prefabIconHero);

    //        Transform transform = _prefabIconHero.transform;
    //        transform.SetParent(content.transform, false); // Устанавливаем родительский объект (Content)

    //        // Изображение
    //        Transform childImage = _prefabIconHero.transform.Find("ImageHero");
    //        if (childImage != null && childImage.TryGetComponent(out UnityEngine.UI.Image image))
    //        {
    //            //Color randomColor = new(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    //            //image.color = randomColor;

    //            // Загрузка PNG из Resources
    //            Sprite loadedSprite = Resources.Load<Sprite>($"Images/Heroes/{heroStats.Name}");

    //            if (loadedSprite != null)
    //            {
    //                image.sprite = loadedSprite; // Устанавливаем загруженный спрайт
    //                //image.color = Color.white;   // Сбрасываем цвет в белый (убираем случайный)
    //            }
    //            else
    //            {
    //                Debug.LogError($"Не удалось загрузить изображение {heroStats.Name} из Resources!");
    //                // Запасной вариант - случайный цвет
    //                image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    //            }
    //        }

    //        // Текст
    //        Transform childText = _prefabIconHero.transform.Find("TextHero");
    //        if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
    //        {
    //            textMeshPro.text = heroStats.Name;
    //        }
    //    }
    //}


    private async void AddAllImageOnContent()
    {
        foreach (HeroBaseEntity heroStats in allHeroes)
        {
            GameObject _prefabIconHero = Instantiate(prefabIconHero);
            Transform transform = _prefabIconHero.transform;
            transform.SetParent(content.transform, false);

            // Текст (может быть установлен сразу)
            Transform childText = _prefabIconHero.transform.Find("TextHero");
            if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
            {
                textMeshPro.text = heroStats.Name;
            }

            // Изображение (загружаем через Addressable)
            Transform childImage = _prefabIconHero.transform.Find("ImageHero");
            if (childImage != null && childImage.TryGetComponent(out UnityEngine.UI.Image image))
            {
                //string addressablePath = $"Assets/GameData/AddressableAssets/Images/Heroes/{heroStats.Name}.png";
                string addressableKey = $"HeroImage/{heroStats.Name.ToLower()}"; // Ключ без расширения .png

                try
                {
                    // Загружаем спрайт асинхронно
                    AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                    _ = await handle.Task;

                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                    {
                        image.sprite = handle.Result;
                    }
                    else
                    {
                        Debug.LogError($"Не удалось загрузить изображение {heroStats.Name} из Addressable Assets!");
                        image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    }

                    // Освобождаем handle после использования
                    Addressables.Release(handle);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка при загрузке изображения {heroStats.Name}: {ex.Message}");
                    image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                }
            }
        }
    }

}
