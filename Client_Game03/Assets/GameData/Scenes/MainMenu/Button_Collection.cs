using Assets.GameData.Scripts;
using UnityEngine;

public class Button_Collection : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.SceneName.Collection);
    }
}
