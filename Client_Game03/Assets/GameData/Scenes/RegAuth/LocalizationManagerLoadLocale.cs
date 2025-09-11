using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

public class LocalizationManagerLoadLocale : MonoBehaviour
{
    private void Start()
    {
        LocalizationManager.Language = LocalizationManager.Languages.Russian;

        GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Email (id=ndtil638)").text = LocalizationManager.GetValue("UI.Label_Email");
        GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Password (id=e319ahd6)").text = LocalizationManager.GetValue("UI.Label_Password");
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonLogin (id=wf6fw0y1)").text = LocalizationManager.GetValue("UI.Button_Login");
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonReg (id=tsuvx5vf)").text = LocalizationManager.GetValue("UI.Button_Reg");
        GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonClose (id=flb78tua)").text = LocalizationManager.GetValue("UI.Button_Close");
    }

}
