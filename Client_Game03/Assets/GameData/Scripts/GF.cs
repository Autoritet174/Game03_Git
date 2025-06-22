using Mono.Cecil;

namespace Assets.GameData.Scripts {

    /// <summary>
    /// GlobalFunctions
    /// </summary>
    public static class GF {

        public static void Log(object message, int type = 0) {
#if UNITY_EDITOR
            string s = $"---GAME_03: {message}";

            switch (type) {
                case 1:
                    UnityEngine.Debug.LogWarning(s);
                    break;
                case 2:
                    UnityEngine.Debug.LogError(s);
                    break;
                default:
                    UnityEngine.Debug.Log(s);
                    break;
            }
#endif
        }

        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        public static void LogW(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="message"></param>
        public static void LogE(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }
    }
}
