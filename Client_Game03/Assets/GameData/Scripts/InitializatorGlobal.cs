using Game03Client;
using System.IO;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    internal static class InitializatorGlobal
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Init()
        {
            //HttpRequester.Init();
            G.Init();
        }
    }
}
