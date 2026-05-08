using Assets.GameData.Scripts;
using UnityEngine;
using L = General.LocalizationKeys;

public class ButtonClose_Click_EndBattle : MonoBehaviour
{
    public async void OnClick()
    {
        bool yesNo = await GameMessage.ShowLocaleYesNo(L.UI.Label.EndBattle);
        if (!yesNo)
        {
            return;
        }

        GameSceneManager.Load(GameSceneManager.SceneName.MainMenu);
    }
}
