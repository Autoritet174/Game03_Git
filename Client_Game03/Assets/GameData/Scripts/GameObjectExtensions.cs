using System;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class GameObjectExtensions
    {
        public static GameObject SafeInstant(this GameObject original, Transform parent = null)
        {
            if (G.IsApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to'{original.name}' while application is quitting");
                return null;
            }
            return UnityEngine.Object.Instantiate(original, parent);
        }

        public static T SafeInstant<T>(this T original) where T : UnityEngine.Object
        {
            if (G.IsApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to instantiate '{original.name}' while application is quitting");
                return null;
            }
            return UnityEngine.Object.Instantiate(original);
        }
    }
}
