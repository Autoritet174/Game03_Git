using Game03Client;
using System;
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
        /// <summary>
        /// Флаг, указывающий на то, что приложение находится в процессе завершения работы.
        /// Должен быть установлен извне.
        /// </summary>
        public static bool IsApplicationQuitting { get; private set; } = false;
        static G()
        {
            // Мониторим состояние через AppDomain
            AppDomain.CurrentDomain.DomainUnload += (s, e) => IsApplicationQuitting = true;
        }
        private class AppStateMonitor : MonoBehaviour
        {
            private void OnApplicationQuit()
            {
                IsApplicationQuitting = true;
            }
        }

        public static Game03 Game { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize_BeforeSceneLoad()
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

            Application.targetFrameRate = 60;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize_AfterSceneLoad()
        {
            AppStateMonitor monitor = new GameObject(nameof(AppStateMonitor)).AddComponent<AppStateMonitor>();
            GameObject.DontDestroyOnLoad(monitor.gameObject);
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
