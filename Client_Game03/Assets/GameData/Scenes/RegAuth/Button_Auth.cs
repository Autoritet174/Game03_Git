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
    TMP_InputField login;

    [SerializeField]
    TMP_InputField password;

    public async void Login() {
        int l1 = login.text.Length;
        string loginString = General.GF.GetEmailOrNull(login.text.Trim());
        if (loginString == null) {
            // тут нужно сообщение об ошибке
            return;
        }

        try {
            myButton.interactable = false;
            await Task.Run(async () => {
                using HttpClient client = new();
                RequestLogin payload = new() { 
                    Username = loginString, 
                    Password = password.text.Trim()
                };
                int l = payload.Username.Length;
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                client.Timeout = TimeSpan.FromSeconds(60);
                //_ = log.AppendLine($"{i} попытка авторизоваться");
                try {
                    HttpResponseMessage response = await client.PostAsync(GC.Uri_login, content);
                       
                    if (!response.IsSuccessStatusCode) {
                        //_ = MessageBox.Show("Ошибка авторизации");
                        return;
                    }

                    string result = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(result);
                    //dynamic obj = JsonConvert.DeserializeObject(result) ?? "";
                    GV.Jwt_token = obj["token"]?.ToString() ?? "";
                    //GV.Jwt_token = obj["token"]?.ToString() ?? "";

                    //_ = log.AppendLine("авторизован");
                    GF.Log("авторизован");
                }
                catch (Exception ex){
                    GF.Log(ex.Message);
                }
                
            });
        }
        catch (Exception ex) {
            GF.Log(ex.Message);
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
