using UnityEngine;

public class Button_InBattle : MonoBehaviour
{
    public void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectBattlefield");
    }
}
