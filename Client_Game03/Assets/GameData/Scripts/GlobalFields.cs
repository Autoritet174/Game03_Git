using System.IO;
using Game03Client;

namespace Assets.GameData.Scripts
{

    /// <summary>
    /// Глобальные поля на всю игру.
    /// </summary>
    public static class GlobalFields
    {
        //public static string Jwt_token;
        public static WebSocketClient webSocketClient = null;
        public static Game03 ClientGame { get; } = Game03.Create(Path.Combine(UnityEngine.Application.dataPath, @"GameData\Config\Main.ini"));
    }
}
