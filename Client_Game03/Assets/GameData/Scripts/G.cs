using Game03Client;
using System;
using System.Diagnostics;
using System.IO;

namespace Assets.GameData.Scripts
{

    /// <summary>
    /// Глобальный статический класс.
    /// </summary>
    public static class G
    {
        public static Game03 Game { get; private set; }

        public static void Init()
        {
            GameLanguage lang = GameLanguage.Ru;

            string path = $"localization/{lang.NameShort}/data";
            UnityEngine.TextAsset jsonFile = UnityEngine.Resources.Load<UnityEngine.TextAsset>(path);
            General.StringCapsule capsule = new()
            {
                Value = jsonFile.text,
            };

            Game = Game03.Create(Path.Combine(UnityEngine.Application.dataPath, @"GameData\Config\Main.ini"), capsule, lang);
            Game.Logger.OnLog += Game_OnLog;
        }

        private static void Game_OnLog(object message)
        {
            UnityEngine.Debug.Log("[Library: Game03Client] " + message);
        }
    }

}
