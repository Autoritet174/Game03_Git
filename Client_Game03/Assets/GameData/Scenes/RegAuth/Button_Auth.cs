using General.DataBaseModels;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using Assets.GameData.Scripts;
using Newtonsoft.Json.Linq;

public class Button_Auth : MonoBehaviour {
    [SerializeField]
    UnityEngine.UI.Button myButton;

    [SerializeField]
    TextMeshProUGUI login;

    [SerializeField]
    TextMeshProUGUI password;

    public async void Login() {
        string loginString = General.Functions.GetEmailOrNull(login.text.Trim());
        if (loginString == null) {
            // тут нужно сообщение об ошибке
            return;
        }

        try {
            myButton.interactable = false;
            await Task.Run(async () => {
                using HttpClient client = new();
                LoginRequest payload = new() { 
                    Username = loginString, 
                    Password = password.text.Trim()
                };
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                client.Timeout = TimeSpan.FromSeconds(5);
                //_ = log.AppendLine($"{i} попытка авторизоваться");
                try {
                    HttpResponseMessage response = await client.PostAsync(GlobalConsts.Uri_login, content);
                       
                    if (!response.IsSuccessStatusCode) {
                        //_ = MessageBox.Show("Ошибка авторизации");
                        return;
                    }

                    string result = await response.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(result) ?? "";
                    //GlobalVariables.Jwt_token = obj.token;
                   // var obj = JObject.Parse(result);
                    //GlobalVariables.Jwt_token = obj["token"]?.ToString() ?? "";

                    //_ = log.AppendLine("авторизован");
                }
                catch {

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


    //public async void Test() {
    //    using var httpClient = new HttpClient();

    //    // Адрес API
    //    var url = "https://localhost:5001/api/test"; // Убедитесь, что порт и адрес совпадают с сервером

    //    // Данные для отправки
    //    var requestData = new {
    //        Username = "test_user",
    //        Password = "123456"
    //    };

    //    // Отправка POST-запроса с сериализацией в JSON
    //    var response = await httpClient.PostAsJsonAsync(url, requestData);

    //    // Обработка ответа
    //    if (response.IsSuccessStatusCode) {
    //        var result = await response.Content.ReadFromJsonAsync<TestResponse>();
    //        Console.WriteLine($"Ответ сервера: {result?.Message}");
    //    }
    //    else {
    //        Console.WriteLine($"Ошибка: {response.StatusCode}");
    //    }
    //}

}
