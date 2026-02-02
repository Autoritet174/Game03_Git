using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General.DTO.RestRequest;
using System;
using System.Security.Cryptography;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    public class AuthHelper : MonoBehaviour
    {
        public static void LogRefreshToken(string refreshToken = null)
        {
            refreshToken ??= Game03Client.Auth.RefreshToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                using var sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(Convert.FromBase64String(refreshToken));
                UnityEngine.Debug.Log(string.Join(' ', hashBytes));
            }
        }

        private static void ClearTokenInSecureStorageProvider()
        {
            SecureStorageProvider.SetValue(SecureStorageKey.RefreshToken, string.Empty);
            SecureStorageProvider.SetValue(SecureStorageKey.RefreshTokenExpirationAt, string.Empty);
        }

        private static void SaveTokenInSecureStorageProvider()
        {
            SecureStorageProvider.SetValue(SecureStorageKey.RefreshToken, Game03Client.Auth.RefreshToken);
            SecureStorageProvider.SetValue(SecureStorageKey.RefreshTokenExpirationAt, Game03Client.Auth.RefreshTokenExpirationAt);
        }

        public static async UniTask<bool> AuthAndLoadDataAsync(string email = null, string password = null, string refreshToken = null)
        {
            try
            {
                Game03Client.Auth.AuthType type;
                if (email != null && password != null && refreshToken == null)
                {
                    type = Game03Client.Auth.AuthType.Login;
                }
                else if (email == null && password == null && refreshToken != null)
                {
                    type = Game03Client.Auth.AuthType.RefreshTokens;
                }
                else
                {
                    throw new Exception("неверно вызванная процедура");
                }


                GameMessage.ShowLocale(L.Info.CheckingServerAvailability, false);
                if (!await GameServerPinger.PingAsync())
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.Unavailable, true);
                    return false;
                }

                GameMessage.ShowLocale(L.Info.Authentication, false);
                DtoRequestAuthReg dto = AuthManager.GetDtoRequestAuthReg(email, password, refreshToken);
                bool authSuccess = await Game03Client.Auth.AuthentificationAsync(dto, type,
                    CancellationTokenManager.Create("Game03Client.Auth.RefreshTokensAsync"));

                if (!authSuccess)
                {
                    ClearTokenInSecureStorageProvider();
                    if (type == Game03Client.Auth.AuthType.Login)
                    {
                        GameMessage.ShowLocale(L.Error.Server.InvalidResponse, true);
                    }
                    else
                    {
                        GameMessage.Close();
                    }
                    return false;
                }

                // Открываем веб сокет
                GameMessage.ShowLocale(L.Info.OpeningWebSocket, false);
                await Game03Client.WebSocketClient.ConnectAsync(
                    CancellationTokenManager.Create("Game03Client.WebSocketClient.ConnectAsync", 5),
                    CancellationTokenManager.GlobalQuitToken);
                if (!Game03Client.WebSocketClient.Connected)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.OpeningWebSocketFailed, true);
                    return false;
                }

                await Game03Client.WebSocketClient.SendMessageAsync("Да это жёстко!", CancellationTokenManager.Create("test"));


                // Загрузка игровых данных не связанных с конкретным пользователем
                GameMessage.ShowLocale(L.Info.LoadingData, false);
                _ = await Game03Client.GameData.LoadGameDataAsync(CancellationTokenManager.Create("Game03Client.GameData.LoadGameData"));

                // Предзагрузка AdressableAssets героев и редкости
                await AddressableCache.PreLoadAssets();

                // Загрузка коллекции пользователя
                GameMessage.ShowLocale(L.Info.LoadingCollection, false);
                bool loaded = await Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync(CancellationTokenManager.Create("Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync"));
                if (!loaded)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.LoadingCollectionFailed, true);
                    return false;
                }

                SaveTokenInSecureStorageProvider();

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
                ClearTokenInSecureStorageProvider();
                GameMessage.ShowError(ex);
                return false;
            }
        }

    }
}
