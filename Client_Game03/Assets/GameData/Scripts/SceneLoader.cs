using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Переход по имени сцены (регистрозависимо!)
    public static void LoadSceneByName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // Переход по индексу сцены в Build Settings
    public static void LoadSceneByIndex(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }
}
