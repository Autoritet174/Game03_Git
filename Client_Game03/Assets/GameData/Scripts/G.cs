using Game03Client;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.GameData.Scripts
{

    /// <summary>
    /// Глобальный статический класс.
    /// </summary>
    public static class G
    {
        public static Game03 Game { get; private set; }

        /// <summary>
        /// Процедура выполняется сразу после компиляции.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            LoadCursorTexture();

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
            UnityEngine.Debug.Log($"[Library: {nameof(Game03Client)}] {message}");
        }

        private static void LoadCursorTexture(string address = "cursor_var2_green_64x64")
        {
            AsyncOperationHandle<Texture2D> operationHandle = Addressables.LoadAssetAsync<Texture2D>(address); // Начинаем операцию загрузки
            _ = operationHandle.WaitForCompletion();
            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                UnityEngine.Debug.LogError($"Ошибка загрузки текста '{address}'");
                return;
            }

            Cursor.SetCursor(operationHandle.Result, Vector2.zero, CursorMode.Auto);
        }
    }

}
