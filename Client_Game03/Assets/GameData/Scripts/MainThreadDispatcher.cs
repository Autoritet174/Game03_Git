using System;
using System.Collections.Concurrent;
using UnityEngine;

/// <summary>Передаёт действия из фоновых потоков в очередь главного потока Unity.</summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static ConcurrentQueue<Action> queue;
    private static MainThreadDispatcher instance;
    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        // Этот метод вызовется автоматически при запуске игры
        // ДО загрузки сцены и гарантированно в основном потоке
        if (instance == null && !initialized)
        {
            GameObject go = new("MainThreadDispatcher");
            instance = go.AddComponent<MainThreadDispatcher>();
            GameObject.DontDestroyOnLoad(go);
            queue = new();
            initialized = true;

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
        queue ??= new();

        queue.Enqueue(action);

        //Debug.Log($"MainThreadDispatcher: Действие добавлено в очередь. Очередь: {_queue.Count}");
    }

    private void Update()
    {
        if (queue == null || queue.IsEmpty)
        {
            return;
        }

        int executed = 0;
        while (queue.TryDequeue(out Action action) && executed < 1000)
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
        if (instance == this)
        {
            instance = null;
            initialized = false;

            if (queue != null)
            {
                while (queue.TryDequeue(out _))
                {
                }
                //Debug.Log("MainThreadDispatcher: Очищен при уничтожении");
            }
        }
    }
}
