using Cysharp.Threading.Tasks;
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

        private static bool resultYesNo = false;

        //private static GameMessage _instance;

        //public static GameMessage Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            GameObject go = new("GameMessageManager");
        //            _instance = go.AddComponent<GameMessage>();
        //            DontDestroyOnLoad(go); // Чтобы не уничтожался при загрузке новой сцены
        //        }
        //        return _instance;
        //    }
        //}



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
            Show(GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(keyLocalization), true);
            await UniTask.WaitUntil(() => !_opened);
        }

        /// <summary>
        /// Выводит игровое сообщение по ключу локализации и ожидает закрытие окна.
        /// </summary>
        public static async Task<bool> ShowLocaleYesNo(string keyLocalization)
        {
            resultYesNo = false;
            Show(GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(keyLocalization), buttonActiveClose: false, yesNoDialog: true);
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
        public static void ShowLocale(string keyLocalization, bool buttonActive, bool isProcess = false)
        {
            Show(GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(keyLocalization), buttonActive, isProcess: isProcess);
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
        public static void Show(string message, bool buttonActiveClose, bool isProcess = false, bool yesNoDialog = false)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.Log("Сообщение не может быть пустым.");
                buttonActiveClose = true;
            }


            // Если окно уже существует, обновляем текст
            if (_currentInstance != null)
            {
                UpdateMessage(message, buttonActiveClose, isProcess: isProcess, yesNoDialog: yesNoDialog);
                return;
            }

            // Загружаем префаб через Addressables
            AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(PREFAB_ADDRESS);
            loadHandle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _currentInstance = UnityEngine.Object.Instantiate(handle.Result);
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

                    UpdateMessage(message, buttonActiveClose, isProcess: isProcess, yesNoDialog: yesNoDialog);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab: {PREFAB_ADDRESS}");
                }
            };
        }

        /// <summary>
        /// Обновляет текст и кнопку в уже созданном окне.
        /// </summary>
        private static void UpdateMessage(string message, bool buttonActiveClose, bool isProcess = false, bool yesNoDialog = false)
        {
            Canvas canvas = _currentInstance.GetComponent<Canvas>();
            Transform windowsImageTransform = canvas.transform.Find("Window-Image");
            GameObject mainTextLabel = windowsImageTransform.Find("MainText-Label").gameObject;

            if (!mainTextLabel.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                throw new Exception("TextMeshProUGUI component not found.");
            }


            if (isProcess)
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
                buttonOkText.text = GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(L.UI.Button.Ok);

                buttonOk.onClick.RemoveAllListeners();
                buttonOk.onClick.AddListener(() =>
                {
                    UnityEngine.Object.Destroy(_currentInstance);
                    _currentInstance = null;
                    _opened = false;
                });
            }
            else if (yesNoDialog)
            {
                gameObjectButtonOk.SetActive(false);

                gameObjectButtonYes.SetActive(true);
                UnityEngine.UI.Button buttonYes = gameObjectButtonYes.GetComponent<UnityEngine.UI.Button>();
                TextMeshProUGUI buttonYesText = GameObjectFinder.FindByName<TextMeshProUGUI>("TextButtonYes", buttonYes.transform);
                buttonYesText.text = GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(L.UI.Button.Yes);
                buttonYes.onClick.RemoveAllListeners();
                buttonYes.onClick.AddListener(() =>
                {
                    resultYesNo = true;
                    UnityEngine.Object.Destroy(_currentInstance);
                    _currentInstance = null;
                    _opened = false;
                });

                gameObjectButtonNo.SetActive(true);
                UnityEngine.UI.Button buttonNo = gameObjectButtonNo.GetComponent<UnityEngine.UI.Button>();
                TextMeshProUGUI buttonNoText = GameObjectFinder.FindByName<TextMeshProUGUI>("TextButtonNo", buttonNo.transform);
                buttonNoText.text = GlobalFields.ClientGame.LocalizationManagerProvider.GetValue(L.UI.Button.No);
                buttonNo.onClick.RemoveAllListeners();
                buttonNo.onClick.AddListener(() =>
                {
                    UnityEngine.Object.Destroy(_currentInstance);
                    _currentInstance = null;
                    _opened = false;
                });
            }
            else
            {
                gameObjectButtonOk.SetActive(false);
                gameObjectButtonYes.SetActive(false);
                gameObjectButtonNo.SetActive(false);
            }
        }
    }
}
