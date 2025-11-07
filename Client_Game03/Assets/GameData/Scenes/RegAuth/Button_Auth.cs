using Assets.GameData.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_Auth : MonoBehaviour
{

    public async void ButtonLogin_OnClick()
    {
        Button buttonLogin = null;
        try
        {
            TMP_InputField textEmail = GameObjectFinder.FindByName<TMP_InputField>("InputText_Email (id=96oaypns)");
            TMP_InputField textPassword = GameObjectFinder.FindByName<TMP_InputField>("InputText_Password (id=9vfnj9oh)");
            buttonLogin = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");

            string emailString = textEmail.text?.Trim() ?? string.Empty;
            string passwordString = textPassword.text?.Trim() ?? string.Empty;

            if (emailString == string.Empty || passwordString == string.Empty)
            {
                GameMessage.ShowLocale("Errors.EmailOrPassword_Empty", true);
                return;
            }

            if (!General.GF.IsEmail(emailString))
            {
                GameMessage.ShowLocale("Errors.Not_Email", true);
                return;
            }

            // Данные авторизации и характеристики аппаратного устройства
            General.ModelHttp.Authorization payload = new(
                emailString,
                passwordString,
                (TimeZoneInfo.Local.BaseUtcOffset.Hours * 60) + TimeZoneInfo.Local.BaseUtcOffset.Minutes,
                System.Environment.UserName,
                SystemInfo.deviceUniqueIdentifier,
                SystemInfo.deviceModel,
                SystemInfo.deviceType.ToString(),
                SystemInfo.operatingSystem,
                SystemInfo.processorType,
                SystemInfo.processorCount,
                SystemInfo.systemMemorySize,
                SystemInfo.graphicsDeviceName,
                SystemInfo.graphicsMemorySize,
                SystemInfo.supportsInstancing,
                SystemInfo.npotSupport.ToString()
            );

            string json = JsonConvert.SerializeObject(payload);

            // Блокируем кнопку и выводим сообщение непосредственно перед await
            buttonLogin.interactable = false;
            GameMessage.ShowLocale("Info.Authentication", false, isProcess: true);

            JObject jObject = await HttpRequester.GetResponceAsync(General.URLs.Uri_login, json);
            if (jObject == null)
            {
                GameMessage.ShowLocale("Errors.Server_InvalidResponse", true);
                return;
            }

            GlobalVariables.Jwt_token = jObject["token"]?.ToString() ?? string.Empty;

            if (GlobalVariables.Jwt_token == string.Empty)
            {
                GameMessage.ShowLocale("Errors.Empty_Token", true);
                return;
            }

            //await GameMessage.ShowAndWaitCloseAsync($"Success! Token = {GV.Jwt_token}");

            GameMessage.ShowLocale("Info.AuthenticationSuccess", false);
            await Task.Delay(700);
            GameMessage.ShowLocale("Info.OpeningWebSocket", false, isProcess: true);

            // Открываем веб сокет
            GlobalVariables.webSocketClient = new WebSocketClient();
            await GlobalVariables.webSocketClient.ConnectAsync();
            if (!GlobalVariables.webSocketClient.Connected)
            {
                GameMessage.ShowLocale("Errors.OpeningWebSocketFailed", true);
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        catch (Exception ex)
        {
            GameMessage.ShowError(ex);
        }
        finally
        {
            if (buttonLogin != null)
            {
                buttonLogin.interactable = true;
            }
        }
    }

}
