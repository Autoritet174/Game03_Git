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
using UnityEngine.UI;
public class Initializer : MonoBehaviour
{
    const string heroImageNull = "hero-image-null";
    private static readonly List<HeroBaseEntity> allHeroes = new();
    public GameObject content;

    public GameObject prefabIconHero;


    private async void Start()
    {
        if (allHeroes.Count == 0)
        {
            await FullListAllHeroes();
        }
        await AddAllImageOnContent();
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

    private async Task AddAllImageOnContent()
    {
        foreach (HeroBaseEntity heroStats in allHeroes)
        {
            await LoadHeroByName(heroStats.Name);
        }
    }

    private async Task LoadHeroByName(string heroName)
    {
        GameObject _prefabIconHero = Instantiate(prefabIconHero);
        _prefabIconHero.name = heroName;

        Transform transform = _prefabIconHero.transform;
        transform.SetParent(content.transform, false);

        // Текст (может быть установлен сразу)
        Transform childText = _prefabIconHero.transform.Find("TextHero");
        if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
        {
            textMeshPro.text = heroName;
        }

        // Изображение (загружаем через Addressable)
        Transform childImage = _prefabIconHero.transform.Find("ImageHero");
        if (childImage != null && childImage.TryGetComponent(out UnityEngine.UI.Image image))
        {
            string addressableKey = $"hero-image-{heroName.ToLower()}-face"; // Ключ без расширения .png

            try
            {
                AsyncOperationHandle<Sprite> handle;
                bool loadNull = true;
                bool loadColor = true;
                if (await AddressableHelper.CheckIfKeyExists(addressableKey))
                {
                    handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                    _ = await handle.Task;
                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                    {
                        SetImage(handle, image);
                        loadNull = false;
                        loadColor = false;
                        //Addressables.Release(handle);
                    }
                }


                if (loadNull && await AddressableHelper.CheckIfKeyExists(heroImageNull))
                {
                    handle = Addressables.LoadAssetAsync<Sprite>(heroImageNull);
                    _ = await handle.Task;
                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                    {
                        SetImage(handle, image);
                        loadColor = false;
                        //Addressables.Release(handle);
                    }
                }


                if (loadColor)
                {
                    Debug.LogError($"Не удалось загрузить изображение {heroName} из Addressable Assets!");
                    image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке изображения {heroName}: {ex.Message}");
                image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }
        }
    }

    void SetImage(AsyncOperationHandle<Sprite> handle, UnityEngine.UI.Image image) {
        image.sprite = handle.Result;

        // Настройка отображения:
        image.preserveAspect = true; // Сохраняет пропорции изображения
        image.type = Image.Type.Simple; // Режим без растягивания

        //// Настройки RectTransform
        //RectTransform rt = image.rectTransform;

        //// 1. Растягиваем по вертикали на 100%
        //rt.anchorMin = new Vector2(0f, 0.12f); // Якорь снизу по центру
        //rt.anchorMax = new Vector2(1f, 1f); // Якорь сверху по центру
        //rt.sizeDelta = new Vector2(0, 0);     // Растягиваем по якорям

        //// 2. Фиксируем ширину по пропорциям текстуры
        //float aspectRatio = (float)handle.Result.texture.width / handle.Result.texture.height;
        //float targetWidth = rt.rect.height * aspectRatio;
        //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);

        //// 3. Центрируем
        //rt.pivot = new Vector2(0.5f, 0.5f);
        //rt.anchoredPosition = Vector2.zero;
        ////image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }

}
