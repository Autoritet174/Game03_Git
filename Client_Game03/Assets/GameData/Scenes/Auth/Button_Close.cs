using UnityEngine;

namespace Assets.GameData.Scenes.Auth
{
    public class Button_Close : MonoBehaviour
    {
        public void CloseGame()
        {
            GameExitHandler.ExitGame();
        }
    }
}
