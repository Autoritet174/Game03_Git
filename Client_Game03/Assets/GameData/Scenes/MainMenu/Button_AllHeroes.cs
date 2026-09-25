using Assets.GameData.Scripts;
using UnityEngine;

/// <summary>Открывает каталог всех героев.</summary>
public class Button_AllHeroes : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.ESceneName.allHeroes);
    }
}
