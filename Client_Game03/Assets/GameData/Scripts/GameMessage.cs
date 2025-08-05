using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.GameData.Scripts
{
    public class GameMessage : MonoBehaviour
    {
        private const string ObjectName = "GameMessage (uuid=6822cd84-695a-4840-9eb7-1cae1b16a9a6)";
        private const string PrefabAddress = "GameMessage-Canvas"; // Адрес префаба в Addressables
        private static bool _opened = false;
        private static GameObject _currentInstance;

        private static GameMessage _instance;

        public static GameMessage Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new("GameMessageManager");
                    _instance = go.AddComponent<GameMessage>();
                    DontDestroyOnLoad(go); // Чтобы не уничтожался при загрузке новой сцены
                }
                return _instance;
            }
        }



        /// <summary>
        /// Выводит игровое сообщение и ожидает закрытие окна.
        /// </summary>
        public static async Task ShowAndWaitCloseAsync(string message)
        {
            Show(message, true);
            //while (_opened)
            //{
            //    await Task.Delay(10);
            //}

            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        public static async Task ShowLocaleAndWaitCloseAsync(string keyLocalization)
        {
            Show(LocalizationManager.GetValue(keyLocalization), true);
            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        public static async Task ShowErrorAndWaitCloseAsync(Exception ex)
        {
            Show("APP_EXCEPTION: An exception has occurred, see log file.", true);
            LoggerException.LogException(ex);
            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации.
        /// </summary>
        public static void ShowLocale(string keyLocalization, bool buttonActive)
        {
            Show(LocalizationManager.GetValue(keyLocalization), buttonActive);
        }

        /// <summary>
        /// Выводит сообщение об ошибке, детали исключения записываются в лог.
        /// </summary>
        public static void ShowError(Exception ex)
        {
            Show("APP_EXCEPTION: An exception has occurred, see log file.", true);
            LoggerException.LogException(ex);
        }

        /// <summary>
        /// Основной метод отображения сообщения.
        /// </summary>
        public static void Show(string message, bool buttonActive)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Message cannot be empty.");
            }

            // Если окно уже существует, обновляем текст
            if (_currentInstance != null)
            {
                UpdateMessage(_currentInstance, message, buttonActive);
                return;
            }

            // Загружаем префаб через Addressables
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(PrefabAddress);
            loadHandle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _currentInstance = UnityEngine.Object.Instantiate(handle.Result);
                    _currentInstance.name = ObjectName;
                    _opened = true;

                    if (!_currentInstance.TryGetComponent(out Canvas canvas))
                    {
                        UnityEngine.Object.Destroy(_currentInstance);
                        throw new Exception("Canvas component not found in the prefab.");
                    }

                    // Находим камеру и настраиваем Canvas
                    if (!GameObject.FindWithTag("MainCamera").TryGetComponent(out Camera mainCamera))
                    {
                        throw new Exception("MainCamera not found in the scene.");
                    }
                    canvas.worldCamera = mainCamera;

                    UpdateMessage(_currentInstance, message, buttonActive);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab: {PrefabAddress}");
                }
            };
        }

        /// <summary>
        /// Обновляет текст и кнопку в уже созданном окне.
        /// </summary>
        private static void UpdateMessage(GameObject messageInstance, string message, bool buttonActive)
        {
            Canvas canvas = messageInstance.GetComponent<Canvas>();
            Transform windowsImageTransform = canvas.transform.Find("Window-Image");
            GameObject mainTextLabel = windowsImageTransform.Find("MainText-Label").gameObject;

            if (!mainTextLabel.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                throw new Exception("TextMeshProUGUI component not found.");
            }

            tmpText.text = message;

            GameObject okButton = windowsImageTransform.Find("Ok-Button").gameObject;
            okButton.SetActive(buttonActive);

            // Подписываемся на кнопку, если она активна
            if (buttonActive)
            {
                UnityEngine.UI.Button button = okButton.GetComponent<UnityEngine.UI.Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    UnityEngine.Object.Destroy(messageInstance);
                    _currentInstance = null;
                    _opened = false;
                });
            }
        }
    }
}