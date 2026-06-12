using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class UniTaskRunner
    {
        public static void RunAsync(this MonoBehaviour host, Func<CancellationToken, UniTask> action)
        {
            if (host == null)
            {
                Debug.LogError($"{nameof(UniTaskRunner)}.{nameof(RunAsync)}: host is null.");
                return;
            }

            if (action == null)
            {
                Debug.LogError($"{nameof(UniTaskRunner)}.{nameof(RunAsync)}: action is null.");
                return;
            }

            action(host.GetCancellationTokenOnDestroy()).Forget(LogException);
        }

        public static void Run(Func<UniTask> action)
        {
            if (action == null)
            {
                Debug.LogError($"{nameof(UniTaskRunner)}.{nameof(Run)}: action is null.");
                return;
            }

            action().Forget(LogException);
        }

        private static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
