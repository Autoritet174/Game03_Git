using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Battlefield
{
    public class ButtonClose_Click_EndBattle : MonoBehaviour
    {
        public void OnClick()
        {
            this.RunAsync(OnClickAsync);
        }

        private async UniTask OnClickAsync(CancellationToken cancellationToken)
        {
            bool yesNo = await GameMessage.ShowLocaleYesNo(L.UI.Label.EndBattle);
            if (!yesNo)
            {
                return;
            }

            bool result = await Game03Client.Battlefield.BattlefieldProvider.CombatBreakAsync(CancellationTokenManager.Create("CombatBreakAsync"));
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
}
