using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using L = General.LocalizationKeys;

public class locManagerLoadLocale : MonoBehaviour
{
    private void Start()
    {
        Game03Client.LocalizationManager.ILocalizationManagerProvider locManager = GlobalFields.ClientGame.LocalizationManagerProvider;
        GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Email (id=ndtil638)").text = locManager.GetValue(L.UI.Label.Email);
        GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Password (id=e319ahd6)").text = locManager.GetValue(L.UI.Label.Password);
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonLogin (id=wf6fw0y1)").text = locManager.GetValue(L.UI.Button.Login);
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonReg (id=tsuvx5vf)").text = locManager.GetValue(L.UI.Button.Reg);
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonClose (id=flb78tua)").text = locManager.GetValue(L.UI.Button.Close);
    }

}
