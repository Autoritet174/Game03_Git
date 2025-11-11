using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Assets.GameData.Scripts
{

    /// <summary>
    /// Предоставляет уникальный идентификатор устройства (HWID) независимо от ОС.
    /// Поддерживает Windows, macOS, Linux, Android, iOS.
    /// </summary>
    public static class HardwareIdentifier
    {
        /// <summary>
        /// Получает хеш уникального идентификатора устройства.
        /// </summary>
        /// <returns>Строка HWID в виде хеша SHA256.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если не удалось получить HWID.</exception>
        public static string GetHWID()
        {
            string rawId = GetRawDeviceIdentifier();
            return string.IsNullOrEmpty(rawId)
                ? throw new InvalidOperationException("Не удалось получить уникальный идентификатор устройства.")
                : ComputeSha256Hash(rawId);
        }

        /// <summary>
        /// Возвращает строку, уникально идентифицирующую устройство на уровне платформы.
        /// </summary>
        /// <returns>Сырой уникальный идентификатор.</returns>
        public static string GetRawDeviceIdentifier()
        {
            //return Application.platform switch
            //{
            //    RuntimePlatform.WindowsPlayer or RuntimePlatform.WindowsEditor => SystemInfo.deviceUniqueIdentifier + Environment.MachineName,
            //    RuntimePlatform.OSXPlayer or RuntimePlatform.OSXEditor => SystemInfo.deviceUniqueIdentifier + Environment.MachineName,
            //    RuntimePlatform.LinuxPlayer or RuntimePlatform.LinuxEditor => SystemInfo.deviceUniqueIdentifier,
            //    RuntimePlatform.Android => SystemInfo.deviceUniqueIdentifier + SystemInfo.operatingSystem,
            //    RuntimePlatform.IPhonePlayer => SystemInfo.deviceUniqueIdentifier,
            //    _ => SystemInfo.deviceUniqueIdentifier,
            //};
            return SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// Вычисляет хеш SHA256 от заданной строки.
        /// </summary>
        /// <param name="input">Строка для хеширования.</param>
        /// <returns>Хеш SHA256 в шестнадцатеричном представлении.</returns>
        private static string ComputeSha256Hash(string input)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] rawBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(rawBytes);
            StringBuilder builder = new();
            foreach (byte b in hashBytes)
            {
                _ = builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
