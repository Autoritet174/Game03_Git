using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new();

    private static UnityMainThreadDispatcher _instance;
    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("MainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out Action action))
        {
            action?.Invoke();
        }
    }

    public static void RunOnMainThread(Action action)
    {
        if (action == null)
        {
            return;
        }

        _executionQueue.Enqueue(action);
    }
}