using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    public class Button_Auth : MonoBehaviour
    {
        private void Start()
        {
            Button button = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");
            //button.onClick.AddListener(() => ButtonLoginOnClick().Forget());
            button.gameObject.SetClickEvent(ButtonLoginOnClick, true);
        }
        public static async UniTask ButtonLoginOnClick()
        {
            GameMessage.ShowLocale(L.Info.CheckingServerAvailability, false);
            if (!await GameServerPinger.PingAsync())
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

                passwordString = Game03Client.Password.HashSha512(passwordString);

                // Блокируем кнопку и выводим сообщение непосредственно перед await
                buttonLogin.interactable = false;
                GameMessage.ShowLocale(L.Info.Authentication, false);

                await Game03Client.Auth.RefreshTokensAsync(
                    AuthManager.GetDtoRequestAuthReg(emailString, passwordString),
                    CancellationTokenManager.Create("Game03Client.Auth.RefreshTokensAsync"));

                string accessToken = Game03Client.Auth.AccessToken;

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    GameMessage.ShowLocale(L.Error.Server.InvalidResponse, true);
                    return;
                }

                GameMessage.ShowLocale(L.Info.OpeningWebSocket, false);


                // Открываем веб сокет
                await Game03Client.WebSocketClient.ConnectAsync(CancellationTokenManager.Create("Game03Client.WebSocketClient.ConnectAsync"));
                if (!Game03Client.WebSocketClient.Connected)
                {
                    GameMessage.ShowLocale(L.Error.Server.OpeningWebSocketFailed, true);
                    return;
                }

                await Game03Client.WebSocketClient.SendMessageAsync("Да это жёстко!");


                // Загрузка игровых данных не связанных с конкретным пользователем
                GameMessage.ShowLocale(L.Info.LoadingData, false);
                await Game03Client.GameData.LoadGameDataAsync(accessToken, CancellationTokenManager.Create("Game03Client.GameData.LoadGameData"));

                // Предзагрузка AdressableAssets героев и редкости
                //UniTask taskPreload = AddressableCache.PreLoadAssets();
                await AddressableCache.PreLoadAssets();

                // Загрузка коллекции пользователя
                GameMessage.ShowLocale(L.Info.LoadingCollection, false);


                bool loaded = await Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync(accessToken,
                    CancellationTokenManager.Create("Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync"));
                if (!loaded)
                {
                    GameMessage.ShowLocale(L.Error.Server.LoadingCollectionFailed, true);
                    return;
                }

                SecureStorageProvider.SetValue(SecureStorageKey.AccessToken, Game03Client.Auth.Dto?.AccessToken);
                SecureStorageProvider.SetValue(SecureStorageKey.SessionToken, Game03Client.Auth.Dto?.RefreshToken);
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
