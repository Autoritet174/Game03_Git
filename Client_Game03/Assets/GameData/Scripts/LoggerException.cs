using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Assets.GameData.Scripts
{

    public static class LoggerException
    {
        //        public enum Error { Info, Warning, ErrorUser };

        //        public static void Log(object message, int type = 0)
        //        {
        //#if UNITY_EDITOR
        //            string s = $"---GAME_03: {message}";

        //            switch (type)
        //            {
        //                case 1:
        //                    UnityEngine.Debug.LogWarning(s);
        //                    break;
        //                case 2:
        //                    UnityEngine.Debug.LogError(s);
        //                    break;
        //                default:
        //                    UnityEngine.Debug.Log(s);
        //                    break;
        //            }
        //#endif
        //        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        //        public static void LogW(object message)
        //        {
        //#if UNITY_EDITOR
        //            UnityEngine.Debug.Log(message);
        //#endif
        //        }

        /// <summary>
        /// Объект блокировки для синхронизации записи логов.
        /// </summary>
        private static readonly object _logFileLock = new();

        /// <summary>
        /// Записать исключение в лог файл.
        /// </summary>
        /// <param name="message"></param>
        public static void LogException(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            try
            {
                DateTime nowUtc = DateTime.UtcNow;

                // Подготовка данных в основном потоке
                string logText = PrepareLogText(ex, nowUtc);

                // Асинхронная запись в файл без вызова Unity API
                _ = Task.Run(() =>
                {
                    string dir = Path.Combine(Application.dataPath, @"Logs");
                    string fileName = $"{Path.Combine(dir, $"ApplicationException_[{nowUtc:yyyy-MM-dd}].log")}";

                    lock (_logFileLock)
                    {
                        _ = Directory.CreateDirectory(dir);
                        File.AppendAllText(fileName, logText, Encoding.UTF8);
                    }
                });
            }
            catch (Exception logEx)
            {
                // Логировать ошибку внутри логирования некуда, можно вывести в консоль
                Debug.LogError($"Exception when logging an exception: {logEx}");
            }
        }
        private static string PrepareLogText(Exception ex, DateTime nowUtc)
        {
            StringBuilder sb = new();
            _ = sb.AppendLine($"[{nowUtc:yyyy-MM-dd HH:mm:ss.fff} (UTC)] Exception logged:");
            _ = sb.AppendLine($"SystemInfo.deviceModel={SystemInfo.deviceModel}");
            _ = sb.AppendLine($"SystemInfo.deviceType={SystemInfo.deviceType}");
            _ = sb.AppendLine($"SystemInfo.operatingSystem={SystemInfo.operatingSystem}");
            _ = sb.AppendLine($"SystemInfo.processorType={SystemInfo.processorType}");
            _ = sb.AppendLine($"SystemInfo.processorCount={SystemInfo.processorCount}");
            _ = sb.AppendLine($"SystemInfo.systemMemorySize={SystemInfo.systemMemorySize}");
            _ = sb.AppendLine($"SystemInfo.graphicsDeviceName={SystemInfo.graphicsDeviceName}");
            _ = sb.AppendLine($"SystemInfo.graphicsMemorySize={SystemInfo.graphicsMemorySize}");
            _ = sb.AppendLine($"SystemInfo.deviceUniqueIdentifier={SystemInfo.deviceUniqueIdentifier}");
            _ = sb.AppendLine($"Screen.currentResolution={Screen.currentResolution.width}x{Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio.value}Hz");
            _ = sb.AppendLine($"Screen.windowSize={Screen.width}x{Screen.height}");
            _ = sb.AppendLine($"Application.version={Application.version}");
            _ = sb.AppendLine($"Application.platform={Application.platform}");
            _ = sb.AppendLine($"Application.unityVersion={Application.unityVersion}");
            _ = sb.AppendLine($"ActiveScene={SceneManager.GetActiveScene().name}");
            _ = sb.AppendLine($"Profiler.TotalAllocatedMemory={Profiler.GetTotalAllocatedMemoryLong()}");
            _ = sb.AppendLine($"Profiler.MonoUsedSize={Profiler.GetMonoUsedSizeLong()}");

            _ = sb.AppendLine();
            _ = sb.AppendLine(ex.ToString());
            _ = sb.AppendLine(new string('*', 100));
            _ = sb.AppendLine();

            return sb.ToString();
        }

    }
}
