using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    public class Button_Auth : MonoBehaviour
    {
        private void Start()
        {
            Button button = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");
            //button.onClick.AddListener(() => ButtonLoginOnClick().Forget());
            button.gameObject.SetClickEvent(ButtonLoginOnClick, true);
        }
        public static async UniTask ButtonLoginOnClick()
        {
            Button buttonLogin = null;
            try
            {
                TMP_InputField textEmail = GameObjectFinder.FindByName<TMP_InputField>("InputText_Email (id=96oaypns)");
                TMP_InputField textPassword = GameObjectFinder.FindByName<TMP_InputField>("InputText_Password (id=9vfnj9oh)");
                buttonLogin = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");


                // Проверка емаил
                string emailString = textEmail.text?.Trim() ?? string.Empty;
                if (emailString == string.Empty)
                {
                    GameMessage.ShowLocale(L.Error.User.EmailEmpty, true);
                    return;
                }
                if (!emailString.IsEmail())
                {
                    GameMessage.ShowLocale(L.Error.User.NotEmail, true);
                    return;
                }

                // Проверка пароля
                string passwordString = textPassword.text?.Trim() ?? string.Empty;
                if (passwordString == string.Empty)
                {
                    GameMessage.ShowLocale(L.Error.User.PasswordEmpty, true);
                    return;
                }

                passwordString = Game03Client.Password.HashSha512(passwordString);

                // Блокируем кнопку и выводим сообщение непосредственно перед await
                buttonLogin.interactable = false;

                bool success = await AuthHelper.AuthAndLoadData(emailString, passwordString);
            }
            catch (Exception ex)
            {
                GameMessage.ShowError(ex);
            }
            finally
            {
                if (buttonLogin != null)
                {
                    buttonLogin.interactable = true;
                }
                //GameMessage.CloseIfNotButton();
            }
        }

    }
}
