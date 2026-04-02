using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        SceneManager.LoadScene("MainMenu");
    }
}
