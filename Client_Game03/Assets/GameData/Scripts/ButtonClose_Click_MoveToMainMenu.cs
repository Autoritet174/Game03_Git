using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClose_Click_MoveToMainMenu : MonoBehaviour
{
    public void OnClick()
    {
        string main ="MainMenu";
        if (SceneManager.GetActiveScene().name == main)
        {
            GameExitHandler.ExitGame();
        }
        else
        {
            SceneManager.LoadScene(main);
        }
    }
}
