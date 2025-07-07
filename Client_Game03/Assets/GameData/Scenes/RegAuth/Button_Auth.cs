using Assets.GameData.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger2 = Assets.GameData.Scripts.Logger2;
public class Button_Auth : MonoBehaviour
{
    [SerializeField]
    private Button buttonLogin;

    [SerializeField]
    private TMP_InputField textEmail;

    [SerializeField]
    private TMP_InputField textPassword;

    private WindowMessageController windowMessageController;


    public async void Login()
    {
        if (windowMessageController == null)
        {
            GameObject windowMessage = GameObjectFinder.FindByTag(WindowMessageInitializator.PrefabTag);
            windowMessageController = windowMessage.GetComponent<WindowMessageController>();
        }


        int l1 = textEmail.text.Length;
        string loginString = General.GF.GetEmailOrNull(textEmail.text.Trim());
        if (loginString == null)
        {
            windowMessageController.SetText("Неверный email", true);
            return;
        }


        buttonLogin.interactable = false;

        bool haveInternet = false;

        try
        {
            await Task.Run(async () =>
            {
                //проверка интернета
                haveInternet = await InternetChecker.CheckInternetConnectionAsync();

                windowMessageController.SetText("Авторизация...", false);

                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(10);

                General.Requests.Login payload = new()
                {
                    Email = loginString,
                    Password = textPassword.text.Trim()
                };

                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_login, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorDetails = await response.Content.ReadAsStringAsync();
                        string userMessage = response.StatusCode switch
                        {
                            HttpStatusCode.Unauthorized => "Неверный email или пароль",
                            HttpStatusCode.BadRequest => "Ошибка в запросе",
                            _ => $"Ошибка сервера: {(int)response.StatusCode}"
                        };

                        windowMessageController.SetText(userMessage, true);
                        Logger2.Log($"Ошибка авторизации: {response.StatusCode}\n{errorDetails}");
                        return;
                    }

                    string result = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(result);
                    GV.Jwt_token = obj["token"]?.ToString() ?? "";

                    if (GV.Jwt_token == string.Empty)
                    {
                        windowMessageController.SetText("Ошибка авторизации. Пустой токен.", true);
                        return;
                    }

                    windowMessageController.SetText("Авторизация выполнена!", false);
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    windowMessageController.SetText("Сервер не доступен", true);
                }
                catch (HttpRequestException ex) when (ex.InnerException is WebException)
                {
                    if (haveInternet)
                    {
                        windowMessageController.SetText("Нет подключения к интернету", true);
                    }
                    else {
                        windowMessageController.SetText("Сервер не доступен", true);
                    }
                }
                catch (Exception ex)
                {
                    windowMessageController.SetText($"APP_ERROR: {ex.Message}", true);
                    Logger2.Log($"GAME_UNVALIDATED_ERROR: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }
        catch (Exception ex)
        {
            windowMessageController.SetText("APP_EXCEPTION: An exception has occurred, see log file", true);
            Logger2.LogE(ex);
        }
        finally
        {
            buttonLogin.interactable = true;
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
