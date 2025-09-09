using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClose_Click_MoveToMainMenu : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
