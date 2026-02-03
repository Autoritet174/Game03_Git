using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static ConcurrentQueue<Action> _queue;
    private static MainThreadDispatcher _instance;
    private static bool _initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        // Этот метод вызовется автоматически при запуске игры
        // ДО загрузки сцены и гарантированно в основном потоке
        if (_instance == null && !_initialized)
        {
            var go = new GameObject("MainThreadDispatcher");
            _instance = go.AddComponent<MainThreadDispatcher>();
            GameObject.DontDestroyOnLoad(go);
            _queue = new ConcurrentQueue<Action>();
            _initialized = true;

            //Debug.Log("MainThreadDispatcher: Автоматически инициализирован при загрузке");
        }
    }

    public static void Run(Action action)
    {
        if (action == null)
        {
            Debug.LogWarning("MainThreadDispatcher: null action");
            return;
        }

        // Если еще не инициализирован, инициализируем сейчас
        if (_queue == null)
        {
            _queue = new ConcurrentQueue<Action>();
        }

        _queue.Enqueue(action);

        //Debug.Log($"MainThreadDispatcher: Действие добавлено в очередь. Очередь: {_queue.Count}");
    }

    private void Update()
    {
        if (_queue == null || _queue.IsEmpty) return;

        int executed = 0;
        while (_queue.TryDequeue(out var action) && executed < 1000)
        {
            try
            {
                executed++;
                action.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"MainThreadDispatcher error: {e}");
            }
        }

        //if (executed > 0)
        //{
        //    Debug.Log($"MainThreadDispatcher: Выполнено {executed} действий");
        //}
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _initialized = false;

            if (_queue != null)
            {
                while (_queue.TryDequeue(out _)) { }
                //Debug.Log("MainThreadDispatcher: Очищен при уничтожении");
            }
        }
    }
}
