using Game03Client;
using System.IO;

namespace Assets.GameData.Scripts
{

    /// <summary>
    /// Глобальные поля на всю игру.
    /// </summary>
    public static class GlobalFields
    {
        //public static string Jwt_token;
        public static WebSocketClient webSocketClient = null;
        public static Game03 ClientGame { get; private set; }
        public static void Init()
        {
            GameLanguage lang = GameLanguage.Ru;

            string path = $"localization/{lang.NameShort}/data";
            UnityEngine.TextAsset jsonFile = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path);
            General.StringCapsule capsule = new()
            {
                Value = jsonFile.text,
            };

            ClientGame = Game03.Create(Path.Combine(UnityEngine.Application.dataPath, @"GameData\Config\Main.ini"), capsule, lang);
        }
    }

}
