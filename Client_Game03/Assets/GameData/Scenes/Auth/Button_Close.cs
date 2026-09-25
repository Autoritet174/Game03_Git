using UnityEngine;

namespace Assets.GameData.Scenes.Auth
{
    /// <summary>Обрабатывает закрытие игры со сцены авторизации.</summary>
    public class Button_Close : MonoBehaviour
    {
        public void CloseGame()
        {
            GameExitHandler.ExitGame();
        }
    }
}
