using Assets.GameData.Scripts;
using UnityEngine;

public class Button_InBattle : MonoBehaviour
{
    public void OnClick()
    {
        GameSceneManager.Load(GameSceneManager.SceneName.SelectBattlefield);
    }
}
