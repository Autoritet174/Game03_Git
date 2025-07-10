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


        string emailString = textEmail.text?.Trim() ?? string.Empty;
        string passwordString = textPassword.text?.Trim() ?? string.Empty;

        if (emailString == string.Empty || passwordString == string.Empty)
        {
            windowMessageController.SetTextLocale("Errors.EmailOrPassword_Empty", true);
            return;
        }

        if (!General.GF.IsEmail(emailString))
        {
            windowMessageController.SetTextLocale("Errors.Not_Email", true);
            return;
        }


        buttonLogin.interactable = false;

        // Данные авторизации и характеристики аппаратного устройства
        General.ModelHttp.Authorization payload = new(emailString, passwordString, SystemInfo.deviceModel, SystemInfo.deviceType.ToString(), SystemInfo.operatingSystem, SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.systemMemorySize, SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize);



        bool haveInternet = false;

        try
        {
            await Task.Run(async () =>
            {
                //проверка интернета
                haveInternet = await InternetChecker.CheckInternetConnectionAsync();

                windowMessageController.SetTextLocale("Info.Authorization", false);

                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(15);

                UnityMainThreadDispatcher.RunOnMainThread(() =>
                {
                    Debug.Log("Device Model: " + SystemInfo.deviceModel);
                    Debug.Log("Device Type: " + SystemInfo.deviceType);
                    Debug.Log("Operating System: " + SystemInfo.operatingSystem);
                    Debug.Log("Processor Type: " + SystemInfo.processorType);
                    Debug.Log("Processor Count: " + SystemInfo.processorCount);
                    Debug.Log("System Memory Size (MB): " + SystemInfo.systemMemorySize);
                    Debug.Log("Graphics Device Name: " + SystemInfo.graphicsDeviceName);
                    Debug.Log("Graphics Memory Size (MB): " + SystemInfo.graphicsMemorySize);
                });



                string json = JsonConvert.SerializeObject(payload);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_login, content);

                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(responseContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        windowMessageController.SetTextLocale(LocalizationManager.GetKeyError(jObject), true);
                        return;
                    }

                    GV.Jwt_token = jObject["token"]?.ToString() ?? "";

                    if (GV.Jwt_token == string.Empty)
                    {
                        windowMessageController.SetTextLocale("Errors.Empty_Token", true);
                        return;
                    }

                    windowMessageController.SetTextLocale("Info.AuthorizationSuccess", true);
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    windowMessageController.SetTextLocale("Errors.Server_Unavailable", true);
                }
                catch (HttpRequestException ex) when (ex.InnerException is WebException)
                {
                    if (haveInternet)
                    {
                        windowMessageController.SetTextLocale("Errors.No_internet_connection", true);
                    }
                    else
                    {
                        windowMessageController.SetTextLocale("Errors.Server_Unavailable", true);
                    }
                }
                catch (Exception ex)
                {
                    windowMessageController.SetTextErrorAndWriteExceptionInLog(ex);
                }
            });
        }
        catch (Exception ex)
        {
            windowMessageController.SetTextErrorAndWriteExceptionInLog(ex);
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
