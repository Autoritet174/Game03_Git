using System;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Security.Cryptography;
#endif

namespace Assets.GameData.Scripts
{

    public enum SecureStorageKey {AccessToken =1 , RefreshToken=2 , RefreshTokenExpirationAt=3}

    /// <summary>
    /// Обеспечивает защищенное хранение данных на Windows (DPAPI), Android (Keystore) и iOS (Keychain).
    /// </summary>
    public static class SecureStorageProvider
    {
        public static void SetValue(SecureStorageKey key, string value)
        {
            SetValue(key.ToString(), value);
        }
        public static void SetValue(SecureStorageKey key, DateTimeOffset? value)
        {
            SetValue(key.ToString(), value?.ToString("yyyy.MM.dd.HH.mm.ss") ?? string.Empty);
        }

        public static string GetString(SecureStorageKey key)
        {
            return GetValue(key.ToString());
        }
        public static DateTimeOffset? GetDateTimeOffset(SecureStorageKey key)
        {
            string storedValue = GetValue(key.ToString());
            return !string.IsNullOrEmpty(storedValue) && DateTimeOffset.TryParseExact(storedValue, "yyyy.MM.dd.HH.mm.ss", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset result)
                ? result
                : null;
        }

        #region Private Methods
        /// <summary>
        /// Сохраняет значение в защищенное хранилище.
        /// </summary>
        /// <param name="key">Ключ доступа к данным.</param>
        /// <param name="value">Строковое значение для сохранения.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если key или value равны null.</exception>
        private static void SetValue(string key, string value)
        {
            if (key == null || value == null)
            {
                throw new ArgumentNullException();
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            SaveWindows(key, value);
#elif UNITY_IOS || UNITY_ANDROID
        // Вызов внешней библиотеки Unity-Simple-Keychain
        //Keychain.SetValue(key, value);
        throw new NotSupportedException("Платформа не поддерживается.");
#else
        throw new NotSupportedException("Платформа не поддерживается.");
#endif
        }


        /// <summary>
        /// Извлекает значение из защищенного хранилища.
        /// </summary>
        /// <param name="key">Ключ доступа.</param>
        /// <returns>Строковое значение или null, если ключ не найден.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если key равен null.</exception>
        private static string GetValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return LoadWindows(key);
#elif UNITY_IOS || UNITY_ANDROID
        //return Keychain.GetValue(key);
        throw new NotSupportedException("Платформа не поддерживается.");
#else
        throw new NotSupportedException("Платформа не поддерживается.");
        //return null;
#endif
        }


#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        /// <summary>
        /// Сохраняет данные на Windows, используя DPAPI (Data Protection API).
        /// </summary>
        private static void SaveWindows(string key, string value)
        {
            // 1. Переводим строку в байты UTF-8
            byte[] data = Encoding.UTF8.GetBytes(value);

            // 2. Шифруем байты через DPAPI
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

            // 3. Конвертируем зашифрованные байты в Base64-строку для хранения в PlayerPrefs
            string base64 = Convert.ToBase64String(encrypted);

            PlayerPrefs.SetString(GetHashedKey(key), base64);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Загружает и расшифровывает данные на Windows.
        /// </summary>
        private static string LoadWindows(string key)
        {
            string storedBase64 = PlayerPrefs.GetString(GetHashedKey(key), null);
            if (string.IsNullOrEmpty(storedBase64))
            {
                return null;
            }

            try
            {
                // 1. Декодируем из Base64 обратно в зашифрованный массив байтов
                byte[] encrypted = Convert.FromBase64String(storedBase64);

                // 2. Расшифровываем через DPAPI
                byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);

                // 3. Переводим байты обратно в строку UTF-8
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SecureStorage] Ошибка расшифровки: {ex.Message}");
                return null;
            }
        }

        private static string GetHashedKey(string key)
        {
            return $"win_sec_{key}";
        }
#endif
    }
    #endregion Private Methods
}
