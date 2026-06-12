using Cysharp.Threading.Tasks;
using Game03Client;
using System;
using System.IO;
using System.Threading;
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
        public const float PANELTOP_HEIGHT = 60f;

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

        private const string CURSOR_TEXTURE_ADDRESS = "UI-cursors-cursor_var2_green_64x64";

        private class AppStateMonitor : MonoBehaviour
        {
            private void Awake()
            {
                this.RunAsync(StartupAsync);
            }

            private async UniTask StartupAsync(CancellationToken cancellationToken)
            {
                await UniTask.WhenAll(
                    LoadCursorTextureAsync(cancellationToken),
                    GameMessage.PreloadAsync(cancellationToken));
            }

            private void OnApplicationQuit()
            {
                IsApplicationQuitting = true;
            }
        }

        //public static Game03 Game { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize_BeforeSceneLoad()
        {
            General.Url.Init("https://localhost:7227");
            GameLanguage lang = GameLanguage.Ru;

            string path = $"localization/{lang.NameShort}/data";
            TextAsset jsonFile = Resources.Load<TextAsset>(path);
            General.StringCapsule capsule = new()
            {
                Value = jsonFile.text,
            };

            Game03.Init(Path.Combine(Application.dataPath, @"GameData\Config\Main.ini"), capsule, LogError, LogInfo);

            Application.targetFrameRate = 60;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize_AfterSceneLoad()
        {
            AppStateMonitor monitor = new GameObject(nameof(AppStateMonitor)).AddComponent<AppStateMonitor>();
            GameObject.DontDestroyOnLoad(monitor.gameObject);
        }

        private static void LogError(object message)
        {
            string m = message.ToString();
            Debug.LogError($"[Library: {nameof(Game03Client)}] {m}");
            LogGameMessage(m);
        }
        private static void LogInfo(object message)
        {
            string m = message.ToString();
            Debug.Log($"[Library: {nameof(Game03Client)}] {m}");
            LogGameMessage(m);
        }
        private static void LogGameMessage(string m)
        {
            int index = m.IndexOf(General.LocalizationKeys.KEY_LOCALIZATION);
            if (index > 0)
            {
                int index1 = m.IndexOf('<', index) + 1;
                int index2 = m.IndexOf('>', index);
                if (index1 > index && index2 > index1)
                {
                    string keyLocale = m[index1..index2];
                    MainThreadDispatcher.Run(() =>
                    {
                        //GameMessage.ShowLocale(keyLocale, true);
                    });
                }
            }
        }


        private static async UniTask LoadCursorTextureAsync(CancellationToken cancellationToken)
        {
            AsyncOperationHandle<Texture2D> operationHandle = Addressables.LoadAssetAsync<Texture2D>(CURSOR_TEXTURE_ADDRESS);
            await operationHandle.ToUniTask(cancellationToken: cancellationToken);
            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                UnityEngine.Debug.LogError($"Ошибка загрузки текста '{CURSOR_TEXTURE_ADDRESS}'");
                return;
            }

            Cursor.SetCursor(operationHandle.Result, Vector2.zero, CursorMode.Auto);
        }

        /// <summary>
        /// Возвращает True если запущена в десктопной операционной системе (Windows, Mac, Linux).
        /// </summary>
        /// <returns></returns>
        public static bool WorkingOnDesktop()
        {
            //return Application.platform switch
            //{
            //    RuntimePlatform.WindowsPlayer or RuntimePlatform.OSXPlayer or RuntimePlatform.LinuxPlayer => true,
            //    RuntimePlatform.Android or RuntimePlatform.IPhonePlayer => false,
            //    _ => false,
            //};
            return Application.platform is RuntimePlatform.WindowsPlayer or RuntimePlatform.OSXPlayer or RuntimePlatform.LinuxPlayer or RuntimePlatform.WindowsEditor;
        }

        /// <summary>
        /// Коэфициент высоты относительно высоты FullHD монитора = "Screen.height / 1080"
        /// </summary>
        public static float GetCoefHeight()
        {
            return Screen.height / 1080f;
        }
    }
}
