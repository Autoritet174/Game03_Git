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

        bool result = await Game03Client.BattleField.BattleFieldProvider.CombatBreakAsync(CancellationTokenManager.Create("CombatBreakAsync"));
        if (result)
        {
            GameSceneManager.Load(GameSceneManager.SceneName.MainMenu);
        }
        else
        {
            await GameMessage.ShowAndWaitCloseAsync(L.Error.Server.CombatBreak);
        }

       
    }
}
