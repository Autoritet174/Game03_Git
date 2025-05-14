using General;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LoaderAllHeroes : MonoBehaviour {
    private List<HeroStats> allHeroes = new();
    public GameObject content;

    public GameObject imagePrefab;

    private async void Start() {
        await GetAllHeroes();
        AddAllImageOnContent();
    }

    private async Task GetAllHeroes() {
        using HttpClient client = new();
        StringContent content = new("", Encoding.UTF8, "application/json");

        client.Timeout = TimeSpan.FromSeconds(60);
        HttpResponseMessage response = await client.PostAsync(GlobalConsts.Uri_login, content);

        if (!response.IsSuccessStatusCode) {
            //_ = MessageBox.Show("Ошибка авторизации");
            return;
        }

        string result = await response.Content.ReadAsStringAsync();


        using JsonDocument doc = JsonDocument.Parse(result);

        if (!doc.RootElement.TryGetProperty("heroes", out JsonElement heroesElement) || heroesElement.ValueKind != JsonValueKind.Array) {
            throw new JsonException("Поле 'heroes' отсутствует или имеет неверный тип.");
        }

        allHeroes = JsonSerializer.Deserialize<List<HeroStats>>(heroesElement.GetRawText())
               ?? throw new JsonException("Не удалось десериализовать список героев.");
    }

    void AddAllImageOnContent() {
        for (int i = 0; i < allHeroes.Count; i++) {
            GameObject go_image = Instantiate(imagePrefab);

            Transform transform = go_image.transform;
            transform.SetParent(content.transform, false); // Устанавливаем родительский объект (Content)

            if (transform.TryGetComponent<Image>(out Image image)) {
                // Генерируем случайный цвет
                Color randomColor = new(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                // Применяем случайный цвет к Image
                image.color = randomColor;
            }

        }
    }
}
