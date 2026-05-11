using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General.DTO.RestRequest;
using System;
using System.Security.Cryptography;
using System.Threading;
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
                Debug.Log(string.Join(' ', hashBytes));
            }
        }

        public static void ClearTokenInSecureStorageProvider()
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
                if (email != null && password != null)
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

                bool success;

                GameMessage.ShowLocale(L.Info.CheckingServerAvailability, false);
                success = await GameServerPinger.PingAsync();
                if (!success)
                {
                    // ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.Unavailable, true);
                    return false;
                }

                GameMessage.ShowLocale(L.Info.Authentication, false);
                DtoRequestAuthReg dto = AuthManager.GetDtoRequestAuthReg(email, password, refreshToken);
                success = await Game03Client.Auth.AuthentificationAsync(dto, type,
                    CancellationTokenManager.Create("Game03Client.Auth.RefreshTokensAsync"));
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    if (type == Game03Client.Auth.AuthType.Login)
                    {
                        GameMessage.ShowLocale(L.Error.Server.InvalidResponse, true);
                    }
                    return false;
                }

                // Открываем веб сокет
                GameMessage.ShowLocale(L.Info.OpeningWebSocket, false);
                success = await Game03Client.WebSocketProvider.ConnectAsync(
                    CancellationTokenManager.Create("Game03Client.WebSocketClient.ConnectAsync", 5),
                    CancellationTokenManager.GlobalQuitToken);
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.OpeningWebSocketFailed, true);
                    return false;
                }


                // Загрузка игровых данных не связанных с конкретным пользователем
                GameMessage.ShowLocale(L.Info.LoadingData, false);
                success = await Game03Client.GameData.LoadGameDataAsync(CancellationTokenManager.Create("Game03Client.GameData.LoadGameData"));
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    await Game03Client.WebSocketProvider.DisconnectAsync();
                    GameMessage.ShowLocale(L.Error.Server.LoadingCollectionFailed, true);
                    Debug.Log("Loading game data failed");
                    return false;
                }

                // Предзагрузка AdressableAssets героев и редкости
                await AddressableCache.PreLoadAssets();


                // Загрузка коллекции пользователя
                GameMessage.ShowLocale(L.Info.LoadingCollection, false);

                CancellationToken ct = CancellationTokenManager.Create("Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync");
                success = await Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync(ct);
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.LoadingCollectionFailed, true);
                    Debug.LogError("Loading collection failed");
                    return false;
                }

                SaveTokenInSecureStorageProvider();

                GameSceneManager.Load(GameSceneManager.SceneName.MainMenu);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                ClearTokenInSecureStorageProvider();
                GameMessage.ShowError(ex);
                return false;
            }
        }

    }
}
