using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Завершает просмотр боя и возвращает игрока в главное меню.</summary>
    public class ButtonClose_Click_EndBattle : MonoBehaviour
    {
        public void OnClick()
        {
            this.RunAsync(OnClickAsync);
        }

        private async UniTask OnClickAsync(CancellationToken cancellationToken)
        {
            bool yesNo = await GameMessage.ShowLocaleYesNoAsync(L.UI.Label.EndBattle);
            if (!yesNo)
            {
                return;
            }

            bool result = await Game03Client.Battlefield.BattlefieldProvider.CombatBreakAsync(CancellationTokenManager.Create("CombatBreakAsync"));
            if (result)
            {
                GameSceneManager.Load(GameSceneManager.ESceneName.mainMenu);
            }
            else
            {
                await GameMessage.ShowAndWaitCloseAsync(L.Error.Server.CombatBreak);
            }
        }
    }
}
