namespace Assets.GameData.Scripts {
    public static class GlobalFunctions {

        public static void Log(object message) {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }
    }
}
