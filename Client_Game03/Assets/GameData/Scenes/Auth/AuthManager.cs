using General.DTO.RestRequest;
using System;
using UnityEngine;

namespace Assets.GameData.Scenes.Auth
{
    /// <summary>Формирует запрос авторизации с данными устройства и приложения.</summary>
    public static class AuthManager
    {
        public static DtoRequestAuthReg GetDtoRequestAuthReg(string email, string password, string refreshToken)
        {
            return new(
                    email,
                    password,
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
                    SystemInfo.npotSupport.ToString(),
                    refreshToken
                );
        }
    }
}
