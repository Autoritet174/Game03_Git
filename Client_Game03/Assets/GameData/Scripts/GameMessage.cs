using Cysharp.Threading.Tasks;
using General;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scripts
{
    public class GameMessage : MonoBehaviour
    {
        private const string OBJECT_NAME = "GameMessage (id=p25hg2gr)";
        private const string PREFAB_ADDRESS = "GameMessage-Canvas"; // Адрес префаба в Addressables
        private static bool _opened = false;
        private static GameObject _currentInstance;

        public static bool Exists => _currentInstance != null;

        private static bool resultYesNo = false;

        /// <summary>
        /// Выводит игровое сообщение и ожидает закрытие окна.
        /// </summary>
        public static async Task ShowAndWaitCloseAsync(string message)
        {
            Show(message, true);
            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        public static async Task ShowLocaleAndWaitCloseAsync(string keyLocalization)
        {
            Show(G.Game.LocalizationManager.GetValue(keyLocalization), true);
            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        public static async Task<bool> ShowLocaleYesNo(string keyLocalization)
        {
            resultYesNo = false;
            Show(G.Game.LocalizationManager.GetValue(keyLocalization), buttonActiveClose: false, yesNoDialog: true);
            await UniTask.WaitUntil(() => !_opened);
            return resultYesNo;
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
            Show(G.Game.LocalizationManager.GetValue(keyLocalization), buttonActive);
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
        public static void Show(string message, bool buttonActiveClose, bool yesNoDialog = false)
        {
            if (message.IsEmpty())
            {
                Debug.Log("Сообщение не может быть пустым.");
                message = string.Empty;
                buttonActiveClose = true;
            }


            // Если окно уже существует, обновляем текст
            if (_currentInstance != null)
            {
                UpdateMessage(message, buttonActiveClose, yesNoDialog: yesNoDialog);
                return;
            }

            //if (!Application.isPlaying)
            //{
            //    return;
            //}
            // Загружаем префаб через Addressables
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(PREFAB_ADDRESS);
            GameObject go = loadHandle.WaitForCompletion();
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                //if (!Application.isPlaying)
                //{
                //    return;
                //}
                _currentInstance = go.SafeInstant();
                if (_currentInstance == null)
                {
                    return;
                }
                _currentInstance.name = OBJECT_NAME;
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

                UpdateMessage(message, buttonActiveClose, yesNoDialog: yesNoDialog);
            }
            else
            {
                Debug.LogError($"Failed to load prefab: {PREFAB_ADDRESS}");
            }
        }

        /// <summary>
        /// Обновляет текст и кнопку в уже созданном окне.
        /// </summary>
        private static void UpdateMessage(string message, bool buttonActiveClose, bool yesNoDialog = false)
        {
            Canvas canvas = _currentInstance.GetComponent<Canvas>();
            Transform windowsImageTransform = canvas.transform.Find("Window-Image");
            GameObject mainTextLabel = windowsImageTransform.Find("MainText-Label").gameObject;

            if (!mainTextLabel.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                throw new Exception("TextMeshProUGUI component not found.");
            }

            if (!buttonActiveClose && !yesNoDialog)
            {
                message += "...";
            }

            tmpText.text = message;

            GameObject gameObjectButtonOk = windowsImageTransform.Find("ButtonOk").gameObject;
            GameObject gameObjectButtonNo = windowsImageTransform.Find("ButtonNo").gameObject;
            GameObject gameObjectButtonYes = windowsImageTransform.Find("ButtonYes").gameObject;

            if (buttonActiveClose)
            {
                gameObjectButtonOk.SetActive(true);
                gameObjectButtonYes.SetActive(false);
                gameObjectButtonNo.SetActive(false);


                UnityEngine.UI.Button buttonOk = gameObjectButtonOk.GetComponent<UnityEngine.UI.Button>();
                TextMeshProUGUI buttonOkText = GameObjectFinder.FindByName<TextMeshProUGUI>("TextButtonOk", buttonOk.transform);
                buttonOkText.text = G.Game.LocalizationManager.GetValue(L.UI.Button.Ok);

                buttonOk.onClick.RemoveAllListeners();
                buttonOk.onClick.AddListener(PressOk);

                InputManager.Register(KeyCode.Escape, PressOk, 1000, KeyCode.Return, KeyCode.KeypadEnter);
            }
            else if (yesNoDialog)
            {
                gameObjectButtonOk.SetActive(false);

                gameObjectButtonYes.SetActive(true);
                UnityEngine.UI.Button buttonYes = gameObjectButtonYes.GetComponent<UnityEngine.UI.Button>();
                TextMeshProUGUI buttonYesText = GameObjectFinder.FindByName<TextMeshProUGUI>("TextButtonYes", buttonYes.transform);
                buttonYesText.text = G.Game.LocalizationManager.GetValue(L.UI.Button.Yes);
                buttonYes.onClick.RemoveAllListeners();
                buttonYes.onClick.AddListener(PressYes);

                gameObjectButtonNo.SetActive(true);
                UnityEngine.UI.Button buttonNo = gameObjectButtonNo.GetComponent<UnityEngine.UI.Button>();
                TextMeshProUGUI buttonNoText = GameObjectFinder.FindByName<TextMeshProUGUI>("TextButtonNo", buttonNo.transform);
                buttonNoText.text = G.Game.LocalizationManager.GetValue(L.UI.Button.No);
                buttonNo.onClick.RemoveAllListeners();
                buttonNo.onClick.AddListener(PressNo);

                InputManager.Register(KeyCode.Escape, PressNo, 1000);
                InputManager.Register(KeyCode.Return, PressYes, 1000, KeyCode.KeypadEnter);
            }
            else
            {
                gameObjectButtonOk.SetActive(false);
                gameObjectButtonYes.SetActive(false);
                gameObjectButtonNo.SetActive(false);
            }
        }

        private static void PressYes()
        {
            InputManager.Unregister(PressYes);
            InputManager.Unregister(PressNo);
            resultYesNo = true;
            Close();
        }
        private static void PressNo()
        {
            InputManager.Unregister(PressYes);
            InputManager.Unregister(PressNo);
            resultYesNo = false;
            Close();
        }
        private static void PressOk()
        {
            InputManager.Unregister(PressOk);
            Close();
        }

        public static void Close()
        {
            try
            {
                if (_currentInstance != null)
                {
                    UnityEngine.Object.Destroy(_currentInstance);
                    _currentInstance = null;
                }
            }
            catch (Exception ex){
                Debug.LogError(ex);
            }
            _opened = false;
        }

        public static void CloseIfNotButton()
        {
            if (!_currentInstance)
            {
                UnityEngine.Object.Destroy(_currentInstance);
                _currentInstance = null;
                _opened = false;
            }
        }
    }
}
