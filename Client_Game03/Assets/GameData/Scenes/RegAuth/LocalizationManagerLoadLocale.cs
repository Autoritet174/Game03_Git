using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

public class LocalizationManagerLoadLocale : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI labelEmail;

    [SerializeField]
    TextMeshProUGUI labelPassword;

    [SerializeField]
    TextMeshProUGUI buttonLogin;

    [SerializeField]
    TextMeshProUGUI buttonReg;

    [SerializeField]
    TextMeshProUGUI buttonClose;

    private void Start()
    {
        LocalizationManager.Init("ru");
        labelEmail.text = LocalizationManager.GetValue("UI.Label_Email");
        labelPassword.text = LocalizationManager.GetValue("UI.Label_Password");
        buttonLogin.text = LocalizationManager.GetValue("UI.Button_Login");
        buttonReg.text = LocalizationManager.GetValue("UI.Button_Reg");
        buttonClose.text = LocalizationManager.GetValue("UI.Button_Close");
    }

}
