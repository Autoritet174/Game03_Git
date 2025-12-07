////using System.Collections.Generic;
////using UnityEngine;

////namespace Assets.GameData.Scripts
////{
////    public sealed class InputManager : MonoBehaviour
////    {
////        // Флаг для отслеживания состояния приложения
////        private static bool isApplicationQuitting = false;

////        // Экземпляр синглтона
////        private static InputManager instance;

////        // Список зарегистрированных обработчиков
////        private readonly List<DelegateHandler> handlers = new();

////        // Свойство для доступа к инстансу
////        private static InputManager Instance
////        {
////            get
////            {
////                if (isApplicationQuitting)
////                {
////                    Debug.LogWarning("InputManager: Application is quitting. Cannot access instance.");
////                    return null;
////                }

////                if (instance == null)
////                {
////                    instance = UnityEngine.Object.FindFirstObjectByType<InputManager>();

////                    // Если не найден, создаем новый экземпляр
////                    if (instance == null)
////                    {
////                        var singletonObj = new GameObject(nameof(InputManager));
////                        DontDestroyOnLoad(singletonObj); // Сохраняем объект при смене сцен
////                        instance = singletonObj.AddComponent<InputManager>();
////                    }
////                }

////                return instance;
////            }
////        }

////        // Делегат для обработки нажатия клавиши
////        public delegate void Handler();

////        /// <summary>
////        /// Регистрация обработчика через делегат
////        /// </summary>
////        /// <param name="key1">Основная клавиша</param>
////        /// <param name="handler">Обработчик</param>
////        /// <param name="priority">Приоритет (чем больше число, тем выше приоритет)</param>
////        /// <param name="key2">Дополнительная клавиша</param>
////        /// <param name="key3">Дополнительная клавиша</param>
////        public static void Register(KeyCode key1, Handler handler, int priority = 0, KeyCode key2 = KeyCode.None, KeyCode key3 = KeyCode.None)
////        {
////            if (isApplicationQuitting || handler == null)
////            {
////                return;
////            }

////            InputManager instance = Instance;
////            if (instance != null)
////            {
////                var wrapper = new DelegateHandler(key1, key2, key3, handler, priority);
////                instance.handlers.Add(wrapper);
////                instance.SortHandlers();
////            }
////        }

////        // Дерегистрация обработчика через делегат
////        public static void Unregister(Handler handler)
////        {
////            if (isApplicationQuitting || handler == null)
////            {
////                return;
////            }

////            InputManager instance = Instance;
////            if (instance != null)
////            {
////                _ = instance.handlers.RemoveAll(h => h.Handler == handler);
////            }
////        }

////        // Внутренний класс-обертка для делегата
////        private class DelegateHandler
////        {
////            public KeyCode Key1;
////            public KeyCode Key2;
////            public KeyCode Key3;
////            public int Priority;
////            public Handler Handler;

////            public DelegateHandler(KeyCode key1, KeyCode key2, KeyCode key3, Handler handler, int priority)
////            {
////                Key1 = key1;
////                Key2 = key2;
////                Key3 = key3;
////                Priority = priority;
////                Handler = handler;
////            }

////            public void OnKeyPressed()
////            {
////                Handler?.Invoke();
////            }
////        }

////        // Сортируем обработчики по приоритету (по возрастанию)
////        private void SortHandlers()
////        {
////            handlers.Sort((h1, h2) => h1.Priority.CompareTo(h2.Priority));
////        }

////        // Главная проверка клавиш
////        private void Update()
////        {
////            // Цикл в обратнем порядке выполняется быстрее чем в прямом порядке
////            for (int i = handlers.Count - 1; i > -1; i--)
////            {
////                DelegateHandler handler = handlers[i];
////                if (GameMessage.Exists && handler.Priority < 1000)
////                {
////                    continue;
////                }

////                if (Input.GetKeyDown(handler.Key1)
////                || (handler.Key2 != KeyCode.None && Input.GetKeyDown(handler.Key2))
////                || (handler.Key3 != KeyCode.None && Input.GetKeyDown(handler.Key3)))
////                {
////                    handler.OnKeyPressed();
////                    break;
////                }
////            }
////        }

////        // Гарантированная инициализация при загрузке сцены
////        private void Awake()
////        {
////            if (instance == null)
////            {
////                instance = this;
////                DontDestroyOnLoad(gameObject);
////            }
////            else if (instance != this)
////            {
////                Destroy(gameObject);
////            }
////        }

////        // Очистка при уничтожении объекта
////        private void OnDestroy()
////        {
////            if (instance == this)
////            {
////                handlers.Clear();
////                instance = null;
////            }
////        }

////        // Отслеживание выхода из приложения
////        private void OnApplicationQuit()
////        {
////            isApplicationQuitting = true;
////            handlers.Clear();

////            if (instance == this)
////            {
////                instance = null;
////            }
////        }
////    }
////}

//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem; // Требуется для новой системы ввода

//namespace Assets.GameData.Scripts
//{
//    /// <summary>
//    /// Централизованный менеджер ввода, реализованный как синглтон.
//    /// Он предоставляет старый интерфейс KeyCode, но использует новую Input System
//    /// для обработки событий, обеспечивая совместимость с существующим кодом.
//    /// </summary>
//    public sealed class InputManager : MonoBehaviour
//    {
//        // Флаг для отслеживания состояния выхода из приложения
//        private static bool isApplicationQuitting = false;

//        // Экземпляр синглтона
//        private static InputManager instance;

//        // Список зарегистрированных обработчиков
//        private readonly List<InputHandler> handlers = new();

//        // НОВЫЙ СПИСОК: Очередь обработчиков для безопасной очистки в LateUpdate
//        private readonly List<InputHandler> handlersToDispose = new();

//        /// <summary>
//        /// Свойство для доступа к единственному экземпляру InputManager.
//        /// Реализует логику ленивой инициализации синглтона.
//        /// </summary>
//        private static InputManager Instance
//        {
//            get
//            {
//                if (isApplicationQuitting)
//                {
//                    Debug.LogWarning("InputManager: Application is quitting. Cannot access instance.");
//                    return null;
//                }

//                if (instance == null)
//                {
//                    instance = UnityEngine.Object.FindFirstObjectByType<InputManager>();

//                    if (instance == null)
//                    {
//                        var singletonObj = new GameObject(nameof(InputManager));
//                        DontDestroyOnLoad(singletonObj); // Сохраняем объект при смене сцены
//                        instance = singletonObj.AddComponent<InputManager>();
//                    }
//                }

//                return instance;
//            }
//        }

//        /// <summary>
//        /// Делегат для обработки нажатия клавиши.
//        /// </summary>
//        public delegate void Handler();

//        /// <summary>
//        /// Регистрирует обработчик по KeyCode, используя внутреннюю реализацию через Input System.
//        /// </summary>
//        /// <param name="key1">Основная клавиша KeyCode.</param>
//        /// <param name="handler">Метод-обработчик, который будет вызван при выполнении действия.</param>
//        /// <param name="priority">Приоритет обработки (чем больше число, тем выше приоритет).</param>
//        /// <param name="key2">Дополнительная клавиша KeyCode.</param>
//        /// <param name="key3">Дополнительная клавиша KeyCode.</param>
//        /// <exception cref="ArgumentNullException">Возникает, если обработчик равен null.</exception>
//        public static void Register(KeyCode key1, Handler handler, int priority = 0, KeyCode key2 = KeyCode.None, KeyCode key3 = KeyCode.None)
//        {
//            if (isApplicationQuitting)
//            {
//                return;
//            }
//            if (handler == null)
//            {
//                throw new ArgumentNullException(nameof(handler), "Handler cannot be null for registration.");
//            }

//            InputManager instance = Instance;
//            if (instance != null)
//            {
//                var wrapper = new InputHandler(key1, key2, key3, handler, priority);
//                instance.handlers.Add(wrapper);
//                instance.SortHandlers();
//            }
//        }

//        /// <summary>
//        /// Дерегистрирует обработчик по ссылке на делегат.
//        /// Если вызов происходит во время колбэка ввода, очистка будет отложена до LateUpdate.
//        /// </summary>
//        /// <param name="handler">Обработчик, который необходимо удалить.</param>
//        public static void Unregister(Handler handler)
//        {
//            if (isApplicationQuitting || handler == null)
//            {
//                return;
//            }

//            InputManager instance = Instance;
//            if (instance != null)
//            {
//                // Ищем и удаляем из активного списка
//                for (int i = instance.handlers.Count - 1; i >= 0; i--)
//                {
//                    if (instance.handlers[i].Handler == handler)
//                    {
//                        // ВАЖНО: Вместо немедленного вызова Dispose() добавляем в очередь.
//                        instance.handlersToDispose.Add(instance.handlers[i]);
//                        instance.handlers.RemoveAt(i);
//                        // Продолжаем цикл, так как один делегат может быть зарегистрирован несколько раз
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Метод, вызываемый после всех Update().
//        /// Используется для безопасного уничтожения InputAction, 
//        /// которые были отменены в ходе обработки ввода.
//        /// </summary>
//        private void LateUpdate()
//        {
//            if (handlersToDispose.Count > 0)
//            {
//                // Safely dispose of actions that were queued for removal
//                foreach (InputHandler handler in handlersToDispose)
//                {
//                    handler.Dispose();
//                }
//                handlersToDispose.Clear();
//            }
//        }

//        /// <summary>
//        /// Внутренний класс-обертка, использующий InputAction для обработки событий ввода.
//        /// Реализует IDisposable для корректной отписки и очистки ресурсов.
//        /// </summary>
//        private class InputHandler : IDisposable
//        {
//            public KeyCode Key1 { get; }
//            public KeyCode Key2 { get; }
//            public KeyCode Key3 { get; }
//            public int Priority { get; }
//            public Handler Handler { get; }

//            // Динамически создаваемое действие Input System
//            private readonly InputAction action;

//            /// <summary>
//            /// Конструктор обработчика, который создает и привязывает InputAction к заданным KeyCode.
//            /// </summary>
//            public InputHandler(KeyCode key1, KeyCode key2, KeyCode key3, Handler handler, int priority)
//            {
//                Key1 = key1;
//                Key2 = key2;
//                Key3 = key3;
//                Priority = priority;
//                Handler = handler;

//                // 1. Создаем новое действие (тип Button подходит для нажатий клавиш)
//                action = new InputAction(type: InputActionType.Button);

//                // 2. Формируем список строк привязки (Binding String)
//                List<string> bindings = new();

//                if (key1 != KeyCode.None)
//                {
//                    // ИСПОЛЬЗУЕМ НОВЫЙ МЕТОД:
//                    bindings.Add($"<Keyboard>/{GetInputSystemBindingName(key1)}");
//                }
//                if (key2 != KeyCode.None)
//                {
//                    // ИСПОЛЬЗУЕМ НОВЫЙ МЕТОД:
//                    bindings.Add($"<Keyboard>/{GetInputSystemBindingName(key2)}");
//                }
//                if (key3 != KeyCode.None)
//                {
//                    // ИСПОЛЬЗУЕМ НОВЫЙ МЕТОД:
//                    bindings.Add($"<Keyboard>/{GetInputSystemBindingName(key3)}");
//                }

//                // 3. Добавляем привязки к действию
//                foreach (var binding in bindings)
//                {
//                    _ = action.AddBinding(binding);
//                }

//                // 4. Подписываемся на событие выполнения действия
//                action.performed += OnActionPerformed;

//                // 5. Включаем действие, чтобы оно начало работать
//                action.Enable();
//            }

//            /// <summary>
//            /// Преобразует KeyCode в корректную строку привязки Input System.
//            /// Обрабатывает исключения, такие как Return и KeypadEnter.
//            /// </summary>
//            /// <param name="keyCode">Исходный KeyCode.</param>
//            /// <returns>Строка привязки Input System (например, "escape", "enter", "space").</returns>
//            private static string GetInputSystemBindingName(KeyCode keyCode)
//            {
//                // Обработка специальных случаев
//                return keyCode switch
//                {
//                    KeyCode.Return => "enter",// KeyCode.Return должен быть "enter", а не "return"
//                    KeyCode.KeypadEnter => "numpadEnter",// KeyCode.KeypadEnter должен быть "numpadEnter", а не "keypadenter"
//                    KeyCode.Escape => "escape",// Для единообразия, хотя ToString() тоже работает
//                                               // ... (Можно добавить другие проблемные клавиши, например, Shift, Control)
//                    _ => keyCode.ToString().ToLower(),// Для большинства клавиш достаточно преобразования в нижний регистр
//                };
//            }


//            /// <summary>
//            /// Обработчик события выполнения действия Input System.
//            /// </summary>
//            /// <param name="context">Контекст колбэка действия.</param>
//            private void OnActionPerformed(InputAction.CallbackContext context)
//            {
//                // Вызов пользовательского делегата
//                Handler?.Invoke();

//                // ВАЖНО: В этом месте пользовательский Handler может вызвать Unregister,
//                // что приведет к добавлению текущего InputHandler в handlersToDispose.
//            }

//            /// <summary>
//            /// Метод для отписки от события и очистки ресурсов Input Action.
//            /// </summary>
//            public void Dispose()
//            {
//                action.performed -= OnActionPerformed;
//                action.Disable();
//                action.Dispose(); // Удаление динамически созданного InputAction
//            }
//        }

//        /// <summary>
//        /// Сортирует обработчики по приоритету.
//        /// </summary>
//        private void SortHandlers()
//        {
//            // Сортируем по убыванию (чем больше число, тем выше приоритет)
//            handlers.Sort((h1, h2) => h2.Priority.CompareTo(h1.Priority));
//        }

//        // Метод Update() более не требуется для обработки ввода.
//        // private void Update() { } 

//        /// <summary>
//        /// Гарантированная инициализация синглтона при загрузке сцены.
//        /// </summary>
//        private void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//                DontDestroyOnLoad(gameObject);
//            }
//            else if (instance != this)
//            {
//                Destroy(gameObject);
//            }
//        }

//        /// <summary>
//        /// Очистка ресурсов при уничтожении объекта.
//        /// </summary>
//        private void OnDestroy()
//        {
//            if (instance == this)
//            {
//                // Очищаем активный список
//                foreach (InputHandler handler in handlers)
//                {
//                    handler.Dispose();
//                }
//                handlers.Clear();

//                // Очищаем список отложенной утилизации
//                foreach (InputHandler handler in handlersToDispose)
//                {
//                    handler.Dispose();
//                }
//                handlersToDispose.Clear();

//                instance = null;
//            }
//        }

//        /// <summary>
//        /// Отслеживание выхода из приложения и окончательная очистка.
//        /// </summary>
//        private void OnApplicationQuit()
//        {
//            isApplicationQuitting = true;

//            // Очистка активного списка
//            foreach (InputHandler handler in handlers)
//            {
//                handler.Dispose();
//            }
//            handlers.Clear();

//            // Очистка списка отложенной утилизации
//            foreach (InputHandler handler in handlersToDispose)
//            {
//                handler.Dispose();
//            }
//            handlersToDispose.Clear();

//            if (instance == this)
//            {
//                instance = null;
//            }
//        }
//    }
//}
