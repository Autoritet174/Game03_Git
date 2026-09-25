#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets.GameData.Scripts
{
    /// <summary>Выбирает стартовую сцену при запуске игры из редактора.</summary>
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
                string startScenePath = $"Assets/GameData/Scenes/Auth/{GameSceneManager.ESceneName.auth}Scene.unity";

                string path = EditorSceneManager.GetActiveScene().path;
                // Проверяем, не загружена ли уже нужная сцена
                if (path != startScenePath && !path.StartsWith("Assets/GameData/Scenes/TEST_"))
                {
                    // Сохраняем текущую сцену (если нужно)
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        _ = EditorSceneManager.OpenScene(startScenePath);
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
#endif
