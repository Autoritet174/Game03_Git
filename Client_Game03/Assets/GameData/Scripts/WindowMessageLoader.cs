using TMPro;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public class WindowMessageLoader
    {  /// <summary>
       /// Загружает Canvas-префаб из Resources, настраивает его RenderMode и камеру.
       /// </summary>
       /// <param name="path">Путь к префабу в папке Resources (без расширения).</param>
       /// <returns>Созданный Canvas или null, если что-то пошло не так.</returns>
        public static void Show(string message, bool buttonActive)
        {
            string prefabTag = "WindowMessage";
            GameObject prefab = GameObject.FindWithTag(prefabTag);

            if (prefab == null)
            {
                string path = "Prefabs/L3/WindowMessage/WindowMessage-Canvas";
                prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    throw new System.Exception($"Not found prefab in Resources '{path}'");
                }

                prefab.tag = prefabTag;
                prefab.name = prefabTag;

                // Создаём экземпляр
                GameObject canvasObj = Object.Instantiate(prefab);
                if (!canvasObj.TryGetComponent(out Canvas canvas))
                {
                    Object.Destroy(canvasObj);
                    throw new System.Exception($"Not found Canvas in prefab");
                }

                // Ищем камеру с тегом "MainCamera" (на первом уровне иерархии)
                if (!GameObject.FindWithTag("MainCamera").TryGetComponent(out Camera mainCamera))
                {
                    throw new System.Exception($"Not found game object with tag 'MainCamera'");
                }

                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = mainCamera;


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



        }
    }
}
