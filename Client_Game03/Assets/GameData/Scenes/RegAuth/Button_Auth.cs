using General.DataBaseModels;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.PackageManager.Requests;

public class Button_Auth : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button myButton;
    public async void Login() {
        myButton.interactable = false;

        try {
            await Task.Run(async () => {
                using HttpClient client = new();
                LoginRequest payload = new() { Username = Guid.NewGuid().ToString(), Password = "testPassword" };
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                client.Timeout = TimeSpan.FromSeconds(60);
                for (int i = 1; i <= 10; i++) {
                    //_ = log.AppendLine($"{i} попытка авторизоваться");
                    try {
                        HttpResponseMessage response = await client.PostAsync(GlobalConsts.Uri_login, content);
                        i = 99999;
                        if (!response.IsSuccessStatusCode) {
                            //_ = MessageBox.Show("Ошибка авторизации");
                            return;
                        }

                        string result = await response.Content.ReadAsStringAsync();
                        dynamic obj = JsonConvert.DeserializeObject(result) ?? "";
                        //_token = obj.token;
                        //_ = log.AppendLine("авторизован");
                    }
                    catch {

                    }
                }
            });
        }
        catch (System.Exception) {

            throw;
        }
        finally {
            myButton.interactable = true;
        }
    }


    public async void Test() {
        using var httpClient = new HttpClient();

        // Адрес API
        var url = "https://localhost:5001/api/test"; // Убедитесь, что порт и адрес совпадают с сервером

        // Данные для отправки
        var requestData = new {
            Username = "test_user",
            Password = "123456"
        };

        // Отправка POST-запроса с сериализацией в JSON
        var response = await httpClient.PostAsJsonAsync(url, requestData);

        // Обработка ответа
        if (response.IsSuccessStatusCode) {
            var result = await response.Content.ReadFromJsonAsync<TestResponse>();
            Console.WriteLine($"Ответ сервера: {result?.Message}");
        }
        else {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
        }
    }

}
