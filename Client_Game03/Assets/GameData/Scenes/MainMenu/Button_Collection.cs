using Assets.GameData.Scripts;
using UnityEngine;

/// <summary>Открывает коллекцию игрока.</summary>
public class Button_Collection : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.ESceneName.collection);
    }
}
