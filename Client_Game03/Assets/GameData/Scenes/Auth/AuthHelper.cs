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
    /// <summary>Управляет сохранением токенов, авторизацией и загрузкой данных игрока.</summary>
    public class AuthHelper : MonoBehaviour
    {
        public static void LogRefreshToken(string refreshToken = null)
        {
            refreshToken ??= Game03Client.Auth.refreshToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                using var sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(Convert.FromBase64String(refreshToken));
                Debug.Log(string.Join(' ', hashBytes));
            }
        }

        public static void ClearTokenInSecureStorageProvider()
        {
            SecureStorageProvider.SetValue(ESecureStorageKey.refreshToken, string.Empty);
            SecureStorageProvider.SetValue(ESecureStorageKey.refreshTokenExpirationAt, string.Empty);
        }

        private static void SaveTokenInSecureStorageProvider()
        {
            SecureStorageProvider.SetValue(ESecureStorageKey.refreshToken, Game03Client.Auth.refreshToken);
            SecureStorageProvider.SetValue(ESecureStorageKey.refreshTokenExpirationAt, Game03Client.Auth.refreshTokenExpirationAt);
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

                GameMessage.ShowLocale(L.Info.checkingServerAvailability, false);
                success = await GameServerPinger.PingAsync();
                if (!success)
                {
                    // ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.unavailable, true);
                    return false;
                }

                GameMessage.ShowLocale(L.Info.authentication, false);
                DtoRequestAuthReg dto = AuthManager.GetDtoRequestAuthReg(email, password, refreshToken);
                success = await Game03Client.Auth.AuthentificationAsync(dto, type,
                    CancellationTokenManager.Create("Game03Client.Auth.AuthentificationAsync"));
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.invalidResponse, true);
                    return false;
                }

                // Открываем веб сокет
                GameMessage.ShowLocale(L.Info.openingWebSocket, false);
                success = await Game03Client.WebSocketProvider.ConnectAsync(
                    CancellationTokenManager.Create("Game03Client.WebSocketClient.ConnectAsync", 5),
                    CancellationTokenManager.globalQuitToken);
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.openingWebSocketFailed, true);
                    return false;
                }

                // Загрузка игровых данных не связанных с конкретным пользователем
                GameMessage.ShowLocale(L.Info.loadingData, false);
                success = await Game03Client.GameData.LoadGameDataAsync(CancellationTokenManager.Create("Game03Client.GameData.LoadGameData"));
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    await Game03Client.WebSocketProvider.DisconnectAsync();
                    GameMessage.ShowLocale(L.Error.Server.loadingCollectionFailed, true);
                    Debug.Log("Loading game data failed");
                    return false;
                }

                // Предзагрузка AdressableAssets героев и редкости
                await AddressablePrefabProvider.PreLoadAssetsAsync();

                // Загрузка коллекции пользователя
                GameMessage.ShowLocale(L.Info.loadingCollection, false);

                CancellationToken ct = CancellationTokenManager.Create("Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync");
                success = await Game03Client.Collection.CollectionProvider.LoadAllCollectionFromServerAsync(ct);
                if (!success)
                {
                    ClearTokenInSecureStorageProvider();
                    GameMessage.ShowLocale(L.Error.Server.loadingCollectionFailed, true);
                    Debug.LogError("Loading collection failed");
                    return false;
                }

                SaveTokenInSecureStorageProvider();

                GameSceneManager.Load(GameSceneManager.ESceneName.mainMenu);
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
