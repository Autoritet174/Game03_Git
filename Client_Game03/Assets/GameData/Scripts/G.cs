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
        private const string CONFIG_RELATIVE_PATH = @"GameData\Config\Main.ini";
        private const string CONFIG_DEV_RELATIVE_PATH = @"GameData\Config\Main.dev.ini";
        private const string SECTION_SERVER = "Server";
        private const string KEY_BASE_URL = "BaseUrl";

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
#if UNITY_EDITOR
            string configPath = Path.Combine(Application.dataPath, CONFIG_DEV_RELATIVE_PATH);
#else
            string configPath = Path.Combine(Application.dataPath, CONFIG_RELATIVE_PATH);
#endif
            General.Url.Init(ReadServerBaseUrlFromIni(configPath));

            GameLanguage lang = GameLanguage.Ru;

            string path = $"localization/{lang.NameShort}/data";
            TextAsset jsonFile = Resources.Load<TextAsset>(path);
            General.StringCapsule capsule = new()
            {
                Value = jsonFile.text,
            };

            Game03.Init(Path.Combine(Application.dataPath, CONFIG_RELATIVE_PATH), capsule, LogError, LogInfo);

            Application.targetFrameRate = 60;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize_AfterSceneLoad()
        {
            AppStateMonitor monitor = new GameObject(nameof(AppStateMonitor)).AddComponent<AppStateMonitor>();
            GameObject.DontDestroyOnLoad(monitor.gameObject);
        }

        private static string ReadServerBaseUrlFromIni(string configPath)
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Server config not found: {configPath}");
            }

            bool inServerSection = false;
            string baseUrl = null;

            foreach (string rawLine in File.ReadAllLines(configPath))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue;
                }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inServerSection = string.Equals(line, $"[{SECTION_SERVER}]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inServerSection)
                {
                    continue;
                }

                int eqIndex = line.IndexOf('=');
                if (eqIndex <= 0)
                {
                    continue;
                }

                string key = line[..eqIndex].Trim();
                if (string.Equals(key, KEY_BASE_URL, StringComparison.OrdinalIgnoreCase))
                {
                    baseUrl = line[(eqIndex + 1)..].Trim();
                    break;
                }
            }

            return string.IsNullOrWhiteSpace(baseUrl)
                ? throw new InvalidOperationException(
                    $"[{SECTION_SERVER}] {KEY_BASE_URL} is missing or empty in {configPath}")
                : baseUrl;
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
            _ = await operationHandle.ToUniTask(cancellationToken: cancellationToken);
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
            return (Screen.width > Screen.height ? Screen.height : Screen.width) / 1080f;
        }
    }
}
