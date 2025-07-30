using Assets.GameData.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
            TMP_InputField textEmail = GameObjectFinder.FindTMPInputFieldByName("InputText_Email (uuid=9b99b098-1949-4b68-bba9-df3660bc95d4)");

            TMP_InputField textPassword = GameObjectFinder.FindTMPInputFieldByName("InputText_Password (uuid=8003daed-ae09-43b9-b033-ae5bb5f5eb38)");

            buttonLogin = GameObjectFinder.FindButtonByName("Button_Login (uuid=0043f96f-ff37-40c4-9a7f-4b302be4eff7)");


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
            General.ModelHttp.Authorization payload = new(emailString, passwordString, SystemInfo.deviceModel, SystemInfo.deviceType.ToString(), SystemInfo.operatingSystem, SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.systemMemorySize, SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.deviceUniqueIdentifier);

            string json = JsonConvert.SerializeObject(payload);

            // Блокируем кнопку и выводим сообщение непосредственно перед await
            buttonLogin.interactable = false;
            GameMessage.ShowLocale("Info.Authorization", false);

            JObject jObject = await HttpRequester.GetResponceAsync(General.URLs.Uri_login, json);
            if (jObject == null)
            {
                GameMessage.ShowLocale("Errors.Server_InvalidResponse", true);
                return;
            }

            GV.Jwt_token = jObject["token"]?.ToString() ?? string.Empty;

            if (GV.Jwt_token == string.Empty)
            {
                GameMessage.ShowLocale("Errors.Empty_Token", true);
                return;
            }

            //await GameMessage.ShowAndWaitCloseAsync($"Success! Token = {GV.Jwt_token}");

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
