using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClose_Click_MoveToMainMenu : MonoBehaviour
{
    public void OnClick()
    {
        if (SceneManager.GetActiveScene().name == GameSceneManager.SceneName.MainMenu.ToString())
        {
            GameExitHandler.ExitGame();
        }
        else
        {
            GameSceneManager.Load(GameSceneManager.SceneName.MainMenu);
        }
    }
}
