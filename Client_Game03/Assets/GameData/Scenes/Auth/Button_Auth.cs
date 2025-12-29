using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using G = Assets.GameData.Scripts.G;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    public class Button_Auth : MonoBehaviour
    {
        private void Start()
        {
            Button button = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");
            button.onClick.AddListener(() => ButtonLoginOnClick().Forget());
        }
        public async UniTask ButtonLoginOnClick()
        {
            GameMessage.ShowLocale(L.Info.CheckingServerAvailability, false);
            bool serverAvailable = await GameServerPinger.Ping();
            if (!serverAvailable)
            {
                GameMessage.ShowLocale(L.Error.Server.Unavailable, true);
                return;
            }

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
                if (!emailString.IsEmail())
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
                GameMessage.ShowLocale(L.Info.Authentication, false);

                // Занулить токен
                G.Game.JwtToken.DeleteToken();

                string json = JsonConvert.SerializeObject(payload);

                string token = await G.Game.JwtToken.GetTokenAsync(json, CancelToken.Create("G.Game.JwtToken.GetTokenAsync"));
                if (token.IsEmpty())
                {
                    GameMessage.ShowLocale(L.Error.Server.InvalidResponse, true);
                    return;
                }

                GameMessage.ShowLocale(L.Info.OpeningWebSocket, false);


                // Открываем веб сокет
                await G.Game.WebSocketClient.ConnectAsync(CancelToken.Create("G.Game.WebSocketClient.ConnectAsync"));
                if (!G.Game.WebSocketClient.Connected)
                {
                    GameMessage.ShowLocale(L.Error.Server.OpeningWebSocketFailed, true);
                    return;
                }

                // Загрузка игровых данных не связанных с конкретным пользователем
                GameMessage.ShowLocale(L.Info.LoadingData, false);
                await G.Game.GameData.LoadGameData(CancelToken.Create("G.Game.GameData.LoadListAllHeroesAsync"));

                // Предзагрузка AdressableAssets героев и редкости
                //UniTask taskPreload = AddressableCache.PreLoadAssets();
                await AddressableCache.PreLoadAssets();

                // Загрузка коллекции пользователя
                GameMessage.ShowLocale(L.Info.LoadingCollection, false);


                bool loaded = await G.Game.Collection.LoadAllCollectionFromServerAsync(CancelToken.Create("G.Game.Collection.LoadAllCollectionFromServerAsync"));
                if (!loaded)
                {
                    GameMessage.ShowLocale(L.Error.Server.LoadingCollectionFailed, true);
                    return;
                }

                //await taskPreload;

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
                //GameMessage.CloseIfNotButton();
            }
        }

    }
}
