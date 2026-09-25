using Assets.GameData.Scripts;
using UnityEngine;

/// <summary>Открывает выбор поля боя по нажатию кнопки.</summary>
public class Button_InBattle : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.ESceneName.selectBattlefield);
    }
}
