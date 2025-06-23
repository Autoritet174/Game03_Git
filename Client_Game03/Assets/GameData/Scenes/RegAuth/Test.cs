using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.RegAuth
{
    public class Test : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private TMP_InputField login;

        [SerializeField]
        private TMP_InputField password;


        private void Start()
        {
            login.text = "SuperAdmin@mail.ru";
            //login.text = "~!\"¹;%:?*()_+!@#$%^&*()_+{}[];':\"<>,./?\\/|";
            password.text = "testPassword";
        }
#endif
    }
}