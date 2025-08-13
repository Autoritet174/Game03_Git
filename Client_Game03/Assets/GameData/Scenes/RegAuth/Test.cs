using Assets.GameData.Scripts;
using UnityEngine;

namespace Assets.GameData.Scenes.RegAuth
{
    public class Test : MonoBehaviour
    {
#if UNITY_EDITOR
        //[SerializeField]
        //private TMP_InputField login;

        //[SerializeField]
        //private TMP_InputField password;


        private void Start()
        {
            GameObjectFinder.FindTMPInputFieldByName("InputText_Email (uuid=9b99b098-1949-4b68-bba9-df3660bc95d4)").text = "SUPERADMIN@MAIL.RU";
            GameObjectFinder.FindTMPInputFieldByName("InputText_Password (uuid=8003daed-ae09-43b9-b033-ae5bb5f5eb38)").text = "testPassword";

            //login.text = "SUPERADMIN@MAIL.RU";
            //login.text = "~!\"№;%:?*()_+!@#$%^&*()_+{}[];':\"<>,./?\\/|";
            //password.text = "testPassword";

            //GameObject prefab = Resources.Load<GameObject>("Prefabs/L3/WindowMessage/WindowMessage");
            //Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

            //// Получаем RectTransform
            //RectTransform rectTransform = prefab.GetComponent<RectTransform>();

            //// Устанавливаем растягивание по всему родителю (если родитель - Canvas)
            //rectTransform.anchorMin = Vector2.zero; // (0, 0)
            //rectTransform.anchorMax = Vector2.one;   // (1, 1)
            //rectTransform.offsetMin = Vector2.zero; // Left, Bottom = 0
            //rectTransform.offsetMax = Vector2.zero; // Right, Top = 0

            //// Сбрасываем позицию и масштаб
            //rectTransform.localPosition = Vector3.zero;
            //rectTransform.localScale = Vector3.one;
            //Scripts.WindowMessageLoader.Show("blablabla", true);
        }
#endif
    }
}