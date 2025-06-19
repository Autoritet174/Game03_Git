using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.RegAuth {
    public class Test : MonoBehaviour {
#if UNITY_EDITOR
        [SerializeField]
        TMP_InputField login;

        [SerializeField]
        TMP_InputField password;


        void Start() {
            login.text = "SuperAdmin@mail.ru";
            password.text = "testPassword";
        }
#endif
    }
}