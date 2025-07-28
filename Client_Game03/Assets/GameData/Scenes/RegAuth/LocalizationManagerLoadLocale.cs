using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

public class LocalizationManagerLoadLocale : MonoBehaviour
{
    private void Start()
    {
        LocalizationManager.Language = LocalizationManager.Languages.Russian;

        GameObjectFinder.FindTextMeshProUGUIByName("Label_Email (uuid=d7e89f75-89bf-4fc0-b243-349e38906945)").text = LocalizationManager.GetValue("UI.Label_Email");
        GameObjectFinder.FindTextMeshProUGUIByName("Label_Password (uuid=e7f6163f-b580-4491-b5e0-8aa1c004c951)").text = LocalizationManager.GetValue("UI.Label_Password");
        GameObjectFinder.FindTextMeshProUGUIByName("Text_ButtonLogin (uuid=e7b048da-baf4-424c-99ad-023d13161d43)").text = LocalizationManager.GetValue("UI.Button_Login");
        GameObjectFinder.FindTextMeshProUGUIByName("Text_ButtonReg (uuid=a57ac64e-4067-48f2-a49e-3a7a82eac12a)").text = LocalizationManager.GetValue("UI.Button_Reg");
        GameObjectFinder.FindTextMeshProUGUIByName("Text_ButtonClose (uuid=33cf102b-5708-4091-884c-032b4188d79b)").text = LocalizationManager.GetValue("UI.Button_Close");
    }

}
