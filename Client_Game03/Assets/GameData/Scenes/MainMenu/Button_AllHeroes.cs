using Assets.GameData.Scripts;
using UnityEngine;

public class Button_AllHeroes : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.SceneName.AllHeroes);
    }
}
