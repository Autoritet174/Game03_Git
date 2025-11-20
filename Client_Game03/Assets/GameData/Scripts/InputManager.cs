using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public sealed class InputManager : MonoBehaviour
    {
        // Флаг для отслеживания состояния приложения
        private static bool isApplicationQuitting = false;

        // Экземпляр синглтона
        private static InputManager instance;

        // Список зарегистрированных обработчиков
        private readonly List<DelegateHandler> handlers = new();

        // Свойство для доступа к инстансу
        private static InputManager Instance
        {
            get
            {
                if (isApplicationQuitting)
                {
                    Debug.LogWarning("InputManager: Application is quitting. Cannot access instance.");
                    return null;
                }

                if (instance == null)
                {
                    instance = UnityEngine.Object.FindFirstObjectByType<InputManager>();

                    // Если не найден, создаем новый экземпляр
                    if (instance == null)
                    {
                        var singletonObj = new GameObject(nameof(InputManager));
                        DontDestroyOnLoad(singletonObj); // Сохраняем объект при смене сцен
                        instance = singletonObj.AddComponent<InputManager>();
                    }
                }

                return instance;
            }
        }

        // Делегат для обработки нажатия клавиши
        public delegate void Handler();

        /// <summary>
        /// Регистрация обработчика через делегат
        /// </summary>
        /// <param name="key1">Основная клавиша</param>
        /// <param name="handler">Обработчик</param>
        /// <param name="priority">Приоритет (чем больше число, тем выше приоритет)</param>
        /// <param name="key2">Дополнительная клавиша</param>
        /// <param name="key3">Дополнительная клавиша</param>
        public static void Register(KeyCode key1, Handler handler, int priority = 0, KeyCode key2 = KeyCode.None, KeyCode key3 = KeyCode.None)
        {
            if (isApplicationQuitting || handler == null)
            {
                return;
            }

            InputManager instance = Instance;
            if (instance != null)
            {
                var wrapper = new DelegateHandler(key1, key2, key3, handler, priority);
                instance.handlers.Add(wrapper);
                instance.SortHandlers();
            }
        }

        // Дерегистрация обработчика через делегат
        public static void Unregister(Handler handler)
        {
            if (isApplicationQuitting || handler == null)
            {
                return;
            }

            InputManager instance = Instance;
            if (instance != null)
            {
                _ = instance.handlers.RemoveAll(h => h.Handler == handler);
            }
        }

        // Внутренний класс-обертка для делегата
        private class DelegateHandler
        {
            public KeyCode Key1;
            public KeyCode Key2;
            public KeyCode Key3;
            public int Priority;
            public Handler Handler;

            public DelegateHandler(KeyCode key1, KeyCode key2, KeyCode key3, Handler handler, int priority)
            {
                Key1 = key1;
                Key2 = key2;
                Key3 = key3;
                Priority = priority;
                Handler = handler;
            }

            public void OnKeyPressed()
            {
                Handler?.Invoke();
            }
        }

        // Сортируем обработчики по приоритету (по возрастанию)
        private void SortHandlers()
        {
            handlers.Sort((h1, h2) => h1.Priority.CompareTo(h2.Priority));
        }

        // Главная проверка клавиш
        private void Update()
        {
            // Цикл в обратнем порядке выполняется быстрее чем в прямом порядке
            for (int i = handlers.Count - 1; i > -1; i--)
            {
                DelegateHandler handler = handlers[i];
                if (GameMessage.Exists && handler.Priority < 1000)
                {
                    continue;
                }

                if (Input.GetKeyDown(handler.Key1)
                || (handler.Key2 != KeyCode.None && Input.GetKeyDown(handler.Key2))
                || (handler.Key3 != KeyCode.None && Input.GetKeyDown(handler.Key3)))
                {
                    handler.OnKeyPressed();
                    break;
                }
            }
        }

        // Гарантированная инициализация при загрузке сцены
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Очистка при уничтожении объекта
        private void OnDestroy()
        {
            if (instance == this)
            {
                handlers.Clear();
                instance = null;
            }
        }

        // Отслеживание выхода из приложения
        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
            handlers.Clear();

            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
