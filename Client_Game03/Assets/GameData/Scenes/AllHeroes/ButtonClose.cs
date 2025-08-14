using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClose : MonoBehaviour
{
    public void LoadScene_MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
