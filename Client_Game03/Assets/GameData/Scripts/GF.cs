namespace Assets.GameData.Scripts {

    /// <summary>
    /// GlobalFunctions
    /// </summary>
    public static class GF {

        public static void Log(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }
    }
}
