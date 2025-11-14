using Assets.GameData.Scripts;
using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

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

            // Проверка емаил
            string emailString = textEmail.text?.Trim() ?? string.Empty;
            if (emailString == string.Empty)
            {
                GameMessage.ShowLocale(L.Error.User.EmailEmpty, true);
                return;
            }
            if (!General.GF.IsEmail(emailString))
            {
                GameMessage.ShowLocale(L.Error.User.NotEmail, true);
                return;
            }

            // Проверка пароля
            string passwordString = textPassword.text?.Trim() ?? string.Empty;
            if (passwordString == string.Empty)
            {
                GameMessage.ShowLocale(L.Error.User.PasswordEmpty, true);
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

            // Блокируем кнопку и выводим сообщение непосредственно перед await
            buttonLogin.interactable = false;
            GameMessage.ShowLocale(L.Info.Authentication, false, isProcess: true);

            string json = JsonConvert.SerializeObject(payload);

            Game03Client.JwtToken.JwtTokenResult jwtTokenResult = await GlobalFields.ClientGame.JwtTokenProvider.GetTokenAsync(json);
            if (jwtTokenResult == null)
            {
                GameMessage.ShowLocale(L.Error.Server.InvalidResponse, true);
                return;
            }
            if (jwtTokenResult.Result == null)
            {
                GameMessage.ShowLocale(GlobalFields.ClientGame.LocalizationManagerProvider.GetTextByJObject(jwtTokenResult.JObject), true);
                return;
            }

            //await GameMessage.ShowAndWaitCloseAsync($"Success! Token = {GV.Jwt_token}");

            //GameMessage.ShowLocale(L.Info.AuthenticationSuccess, false);
            //await Task.Delay(100);
            GameMessage.ShowLocale(L.Info.OpeningWebSocket, false, isProcess: true);

            // Открываем веб сокет
             var webSocketClient = GlobalFields.ClientGame.WebSocketClientProvider;
            await webSocketClient.ConnectAsync();
            if (!webSocketClient.Connected)
            {
                GameMessage.ShowLocale(L.Error.Server.OpeningWebSocketFailed, true);
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
