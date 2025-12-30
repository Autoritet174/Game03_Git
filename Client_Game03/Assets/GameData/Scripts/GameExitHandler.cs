using Assets.GameData.Scripts;
using System;
using System.Collections;
using UnityEngine;
using L = General.LocalizationKeys;

public static class GameExitHandler
{
    public static async void ExitGame()
    {
        bool yesNo = await GameMessage.ShowLocaleYesNo(L.UI.Label.ExitGame);
        if (!yesNo)
        {
            return;
        }
        CancellationTokenManager.CancelAllTokens();
        await G.Game.WebSocketClient.DisconnectAsync();
        // Сохраняем данные перед выходом
        //SaveSystem.SaveGame();

        // Получаем MonoBehaviour для запуска корутины
        MonoBehaviour coroutineHost = GetCoroutineHost();
        if (coroutineHost != null)
        {
            _ = coroutineHost.StartCoroutine(ExitRoutine());
        }
        else
        {
            Debug.LogError("No MonoBehaviour found to start coroutine!");
            ForceQuit();
        }
    }

    private static MonoBehaviour GetCoroutineHost()
    {
        // Ищем любой активный MonoBehaviour в сцене
        return GameObject.FindFirstObjectByType<MonoBehaviour>();
    }

    private static IEnumerator ExitRoutine()
    {
        // Эффект плавного затемнения (опционально)
        //if (FadeManager.Instance != null) {
        //    yield return FadeManager.Instance.FadeOut(1f);
        //}
        yield return 1;

        // Выбираем метод выхода в зависимости от платформы
        if (IsInEditor())
        {
            QuitInEditor();
        }
        else if (IsWindowsMacOrLinux())
        {
            Application.Quit();
        }
        else if (IsAndroid())
        {
            QuitAndroid();
        }
        else if (IsIOS())
        {
            QuitIOS();
        }
        else if (IsWebGL())
        {
            QuitWebGL();
        }
        else
        {
            Application.Quit(); // По умолчанию
        }
    }

    private static bool IsInEditor()
    {
        return Application.isEditor;
    }

    private static bool IsWindowsMacOrLinux()
    {
        return Application.platform is RuntimePlatform.WindowsPlayer or
               RuntimePlatform.OSXPlayer or
               RuntimePlatform.LinuxPlayer;
    }

    private static bool IsAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }

    private static bool IsIOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }

    private static bool IsWebGL()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }

    private static void QuitInEditor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Editor quit requested but not in editor mode");
#endif
    }

    private static void QuitAndroid()
    {
        try
        {
            AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finishAndRemoveTask");
        }
        catch (Exception e)
        {
            Debug.LogError("Android quit failed: " + e.Message);
            Application.Quit();
        }
    }

    private static void QuitIOS()
    {
        // На iOS Application.Quit() просто сворачивает приложение
        Application.Quit();
    }

    private static void QuitWebGL()
    {
        // В WebGL нельзя закрыть вкладку, только выйти из полноэкранного режима
        Screen.fullScreen = false;
        // Можно показать сообщение для пользователя
        GameObject message = new("ExitMessage");
        TMPro.TextMeshProUGUI text = message.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "Please close the browser tab to exit";
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.fontSize = 24;
        UnityEngine.Object.DontDestroyOnLoad(message);
    }

    private static void ForceQuit()
    {
        if (IsInEditor())
        {
            QuitInEditor();
        }
        else
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
