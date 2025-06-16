using UnityEngine;

public class Button_Close : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CloseGame() {
        GameExitHandler.ExitGame();
    }
}
