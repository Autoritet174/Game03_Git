using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;

public class LoaderAllHeroes : MonoBehaviour
{
    private static readonly List<object> allHeroes = new();
    public GameObject content;

    public GameObject prefabIconHero;

    private async void Start()
    {
        if (allHeroes.Count == 0) {
            await GetAllHeroes();
        }
        AddAllImageOnContent();
    }

    


    private async Task GetAllHeroes()
    {
        using HttpClient client = new();
        StringContent content = new("", Encoding.UTF8, "application/json");

        client.Timeout = TimeSpan.FromSeconds(60);
        HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_login, content);

        if (!response.IsSuccessStatusCode)
        {
            //_ = MessageBox.Show("Ошибка авторизации");
            return;
        }

        string result = await response.Content.ReadAsStringAsync();


        using JsonDocument doc = JsonDocument.Parse(result);

        if (!doc.RootElement.TryGetProperty("heroes", out JsonElement heroesElement) || heroesElement.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("Поле 'heroes' отсутствует или имеет неверный тип.");
        }

        //allHeroes = JsonSerializer.Deserialize<List<HeroStats>>(heroesElement.GetRawText())
        //       ?? throw new JsonException("Не удалось десериализовать список героев.");
    }

    private void AddAllImageOnContent()
    {
        //foreach (HeroStats heroStats in allHeroes) {
        //    GameObject _prefabIconHero = Instantiate(prefabIconHero);

        //    Transform transform = _prefabIconHero.transform;
        //    transform.SetParent(content.transform, false); // Устанавливаем родительский объект (Content)

        //    // Изображение
        //    Transform childImage = _prefabIconHero.transform.Find("ImageHero");
        //    if (childImage != null && childImage.TryGetComponent(out Image image)) {
        //        Color randomColor = new(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        //        image.color = randomColor;
        //    }

        //    // Текст
        //    Transform childText = _prefabIconHero.transform.Find("TextHero");
        //    if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro)) {
        //        textMeshPro.text = heroStats.Name;
        //    }
        //}
    }
}
