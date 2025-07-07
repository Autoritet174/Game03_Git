using TMPro;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class WindowMessageLoader
    {
        private static readonly string prefabTag = "WindowMessage";

        /// <summary>
        /// Загружает Canvas-префаб из Resources, настраивает его RenderMode и камеру.
        /// </summary>
        /// <param name="path">Путь к префабу в папке Resources (без расширения).</param>
        /// <returns>Созданный Canvas или null, если что-то пошло не так.</returns>
        public static void Show(string message, bool buttonActive)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new System.Exception("message is empty");
            }

            GameObject prefab_Object = GameObject.FindWithTag(prefabTag);
            Canvas canvas = null;
            if (prefab_Object == null)
            {
                string path = "Prefabs/L3/WindowMessage/WindowMessage-Canvas";
                prefab_Object = Resources.Load<GameObject>(path);
                if (prefab_Object == null)
                {
                    throw new System.Exception($"Not found prefab in Resources '{path}'");
                }

                prefab_Object.tag = prefabTag;
                prefab_Object.name = prefabTag;

                prefab_Object = Object.Instantiate(prefab_Object);
                if (!prefab_Object.TryGetComponent(out canvas))
                {
                    Object.Destroy(prefab_Object);
                    throw new System.Exception($"Not found Canvas in prefab");
                }

                // Ищем камеру с тегом "MainCamera" (на первом уровне иерархии)
                if (!GameObject.FindWithTag("MainCamera").TryGetComponent(out Camera mainCamera))
                {
                    throw new System.Exception($"Not found game object with tag 'MainCamera'");
                }
                canvas.worldCamera = mainCamera;
            }

            if (canvas == null && !prefab_Object.TryGetComponent(out canvas))
            {
                Object.Destroy(prefab_Object);
                throw new System.Exception($"Not found Canvas in prefab");
            }

            Transform windosImageTransform = canvas.transform.Find("Window-Image");
            GameObject go_label = windosImageTransform.Find("MainText-Label").gameObject;

            if (!go_label.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                throw new System.Exception($"Not found 'TextMeshProUGUI'");
            }

            tmpText.text = message;

            GameObject go_button = windosImageTransform.Find("Ok-Button").gameObject;
            go_button.SetActive(buttonActive);
        }

        public static void Hide()
        {
            GameObject prefab = GameObject.FindWithTag(prefabTag);
            if (prefab != null)
            {
                Object.Destroy(prefab);
            }
        }

    }
}
