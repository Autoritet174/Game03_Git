using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public class GameMessage : MonoBehaviour
    {
        private static readonly string objectName = "GameMessage (uuid=6822cd84-695a-4840-9eb7-1cae1b16a9a6)";
        private static bool opened = false;


        /// <summary>
        /// Выводит игровое сообщение и ожидает закрытие окна.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttonActive"></param>
        /// <returns></returns>
        public static async Task ShowAndWaitCloseAsync(string message)
        {
            Show(message, true);
            while (opened)
            {
                await Task.Delay(10);
            }

        }


        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttonActive"></param>
        /// <returns></returns>
        public static async Task ShowLocaleAndWaitCloseAsync(string keyLocalization)
        {
            Show(LocalizationManager.GetValue(keyLocalization), true);
            while (opened)
            {
                await Task.Delay(10);
            }
        }


        /// <summary>
        /// Выводит игровое сообщение по ключу локализации.
        /// </summary>
        public static void ShowLocale(string keyLocalization, bool buttonActive)
        {
            Show(LocalizationManager.GetValue(keyLocalization), buttonActive);
        }


        /// <summary>
        /// Выводит сообщение о том что возникло исключение, детали исключения записываются в лог файл.
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowError(Exception ex)
        {
            Show("APP_EXCEPTION: An exception has occurred, see log file.", true);
            LoggerException.LogException(ex);
        }


        /// <summary>
        /// Выводит игровое сообщение.
        /// </summary>
        public static void Show(string message, bool buttonActive)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new System.Exception("message is empty");
            }

            GameObject prefab_Object = GameObjectFinder.FindByName(objectName);
            Canvas canvas = null;
            if (prefab_Object == null)
            {
                string path = "Prefabs/L3/GameMessage/GameMessage-Canvas";
                prefab_Object = Resources.Load<GameObject>(path);
                if (prefab_Object == null)
                {
                    throw new System.Exception($"Not found prefab in Resources '{path}'");
                }

                prefab_Object = Instantiate(prefab_Object);
                opened = true;

                if (!prefab_Object.TryGetComponent(out canvas))
                {
                    Destroy(prefab_Object);
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
                Destroy(prefab_Object);
                throw new System.Exception($"Not found Canvas in prefab");
            }

            Transform windowsImageTransform = canvas.transform.Find("Window-Image");
            GameObject go_label = windowsImageTransform.Find("MainText-Label").gameObject;

            if (!go_label.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                throw new System.Exception($"Not found 'TextMeshProUGUI'");
            }

            tmpText.text = message;
            prefab_Object.name = objectName;

            GameObject go_button = windowsImageTransform.Find("Ok-Button").gameObject;
            go_button.SetActive(buttonActive);
        }


        /// <summary>
        /// Закрывает окно сообщения.
        /// </summary>
        public void Destroy_OnClick()
        {
            GameObject prefab_Object = GameObjectFinder.FindByName(objectName);
            if (prefab_Object != null)
            {
                Destroy(prefab_Object);
                opened = false;
            }
        }

    }
}
