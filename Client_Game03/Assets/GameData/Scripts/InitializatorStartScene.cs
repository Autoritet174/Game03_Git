using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets.GameData.Scripts
{
    [InitializeOnLoad]
    internal static class InitializatorStartScene
    {
        static InitializatorStartScene()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            // Если игра запускается (перед входом в Play Mode)
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                const string StartScenePath = @"Assets\GameData\Scenes\RegAuth\RegAuth.unity";
                // Проверяем, не загружена ли уже нужная сцена
                if (EditorSceneManager.GetActiveScene().path != StartScenePath)
                {
                    // Сохраняем текущую сцену (если нужно)
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        _ = EditorSceneManager.OpenScene(StartScenePath);
                    }
                    else
                    {
                        // Если пользователь отменил сохранение, отменяем вход в Play Mode
                        EditorApplication.isPlaying = false;
                    }
                }
            }
        }

    }
}
