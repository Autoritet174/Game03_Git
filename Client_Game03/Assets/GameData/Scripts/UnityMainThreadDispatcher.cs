using System;

public class UnityMainThreadDispatcher
{
    /// <summary>
    /// Запустить действие из основного потока.
    /// </summary>
    /// <param name="action"></param>
    public static void RunOnMainThread(Action action)
    {
        if (action == null)
        {
            return;
        }
        action.Invoke();
    }

}
