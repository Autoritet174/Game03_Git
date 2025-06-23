using Assets.GameData.Scripts;
using General.DataBaseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Button_Auth : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Button myButton;

    [SerializeField]
    private TMP_InputField login;

    [SerializeField]
    private TMP_InputField password;

    public async void Login()
    {
        int l1 = login.text.Length;
        string loginString = General.GF.GetEmailOrNull(login.text.Trim());
        if (loginString == null)
        {
            // тут нужно сообщение об ошибке
            return;
        }

        try
        {
            myButton.interactable = false;
            await Task.Run(async () =>
            {
                using HttpClient client = new();
                General.Requests.Login payload = new()
                {
                    Email = loginString,
                    Password = password.text.Trim()
                };
                int l = payload.Email.Length;
                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                client.Timeout = TimeSpan.FromSeconds(60);
                //_ = log.AppendLine($"{i} попытка авторизоваться");
                try
                {
                    HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_login, content);

                    if (!response.IsSuccessStatusCode)
                    {
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
                catch (Exception ex)
                {
                    GF.Log(ex.Message);
                }

            });
        }
        catch (Exception ex)
        {
            GF.Log(ex.Message);
        }
        finally
        {
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
