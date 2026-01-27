using General.DTO.RestRequest;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.GameData.Scenes.Auth
{
    public static class AuthManager
    {
        public static DtoRequestAuthReg GetDtoRequestAuthReg(string email, string password)
        {
            return new DtoRequestAuthReg(
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
                    ""
                );
        }
    }
}
