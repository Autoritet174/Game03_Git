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
    private Button buttonLogin;
    private TMP_InputField textEmail;
    private TMP_InputField textPassword;
    private WindowMessageController windowMessageController;


    public async void Login()
    {
        if (windowMessageController == null)
        {
            GameObject windowMessage = GameObjectFinder.FindByTag(WindowMessageInitializator.PrefabTag);
            windowMessageController = windowMessage.GetComponent<WindowMessageController>();
        }
        if (textEmail == null)
        {
            textEmail = GameObjectFinder.FindTMPInputFieldByName("InputText_Email (uuid=9b99b098-1949-4b68-bba9-df3660bc95d4)");
        }
        if (textPassword == null) {
            textPassword = GameObjectFinder.FindTMPInputFieldByName("InputText_Password (uuid=8003daed-ae09-43b9-b033-ae5bb5f5eb38)");
        }
        if (buttonLogin == null)
        {
            buttonLogin = GameObjectFinder.FindButtonByName("Button_Login (uuid=0043f96f-ff37-40c4-9a7f-4b302be4eff7)");
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
        General.ModelHttp.Authorization payload = new(emailString, passwordString, SystemInfo.deviceModel, SystemInfo.deviceType.ToString(), SystemInfo.operatingSystem, SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.systemMemorySize, SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.deviceUniqueIdentifier);

        try
        {
            bool success = await LoginAsync(payload);
            if (success)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
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


    private async Task<bool> LoginAsync(General.ModelHttp.Authorization payload)
    {
        //проверка интернета
        bool haveInternet = await InternetChecker.CheckInternetConnectionAsync();

        windowMessageController.SetTextLocale("Info.Authorization", false);

        using HttpClient client = new();
        client.Timeout = TimeSpan.FromSeconds(15);

        //UnityMainThreadDispatcher.RunOnMainThread(() =>
        //{
        //    Debug.Log("Device Model: " + SystemInfo.deviceModel);
        //    Debug.Log("Device Type: " + SystemInfo.deviceType);
        //    Debug.Log("Operating System: " + SystemInfo.operatingSystem);
        //    Debug.Log("Processor Type: " + SystemInfo.processorType);
        //    Debug.Log("Processor Count: " + SystemInfo.processorCount);
        //    Debug.Log("System Memory Size (MB): " + SystemInfo.systemMemorySize);
        //    Debug.Log("Graphics Device Name: " + SystemInfo.graphicsDeviceName);
        //    Debug.Log("Graphics Memory Size (MB): " + SystemInfo.graphicsMemorySize);
        //    Debug.Log("Device Unique Identifier: " + SystemInfo.deviceUniqueIdentifier);
        //});



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
                return false;
            }

            GV.Jwt_token = jObject["token"]?.ToString() ?? "";

            if (GV.Jwt_token == string.Empty)
            {
                windowMessageController.SetTextLocale("Errors.Empty_Token", true);
                return false;
            }

            //windowMessageController.SetTextLocale("Info.AuthorizationSuccess", true);

            return true;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            windowMessageController.SetTextLocale("Errors.Server_Unavailable", true);
            return false;
        }
        catch (HttpRequestException ex) when (ex.InnerException is WebException)
        {
            windowMessageController.SetTextLocale(haveInternet ? "Errors.Server_Unavailable" : "Errors.No_internet_connection", true);
            return false;
        }
        catch (Exception ex)
        {
            windowMessageController.SetTextErrorAndWriteExceptionInLog(ex);
            return false;
        }
    }

}
