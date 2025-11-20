using System;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class GameObjectExtensions
    {
        private static bool isApplicationQuitting = false;

        static GameObjectExtensions()
        {
            // Мониторим состояние через AppDomain
            AppDomain.CurrentDomain.DomainUnload += (s, e) => isApplicationQuitting = true;
        }

        public static GameObject SafeInstant(this GameObject original, Transform parent = null)
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to'{original.name}' while application is quitting");
                return null;
            }
            return UnityEngine.Object.Instantiate(original, parent);
        }

        public static T SafeInstant<T>(this T original) where T : UnityEngine.Object
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to instantiate '{original.name}' while application is quitting");
                return null;
            }
            return UnityEngine.Object.Instantiate(original);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            AppStateMonitor monitor = new GameObject("AppStateMonitor").AddComponent<AppStateMonitor>();
            GameObject.DontDestroyOnLoad(monitor.gameObject);
        }

        private class AppStateMonitor : MonoBehaviour
        {
            private void OnApplicationQuit()
            {
                isApplicationQuitting = true;
            }
        }
    }
}
