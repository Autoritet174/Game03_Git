using UnityEngine;

public class Button_Close : MonoBehaviour
{
    public void CloseGame()
    {
        GameExitHandler.ExitGame();
    }
}
