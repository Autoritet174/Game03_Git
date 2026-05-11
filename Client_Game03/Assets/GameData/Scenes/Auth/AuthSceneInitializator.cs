using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    public class AuthSceneInitializator : MonoBehaviour
    {

        private bool _initialized = false;
        private float _width, _height;

        private Button _ButtonLogin_Button;

        private RectTransform _ButtonLogin_RectTransform;
        private RectTransform _ButtonReg_RectTransform;
        private RectTransform _ButtonExitGame_RectTransform;
        private RectTransform _InputTextWithLabelEmail_RectTransform;
        private RectTransform _InputTextWithLabelPassword_RectTransform;
        private TextMeshProUGUI _LabelEmail_TextMeshProUGUI;
        private TextMeshProUGUI _LabelPassword_TextMeshProUGUI;
        private TextMeshProUGUI _TextEmail_TextMeshProUGUI;
        private TextMeshProUGUI _TextPassword_TextMeshProUGUI;
        private TextMeshProUGUI _TextButtonLogin_TextMeshProUGUI;
        private TextMeshProUGUI _TextButtonReg_TextMeshProUGUI;
        private TextMeshProUGUI _TextButtonExitGame_TextMeshProUGUI;

        private Image _ImageBackground_Image;
        private float _ImageBackground_CoefWH = 1f;

        private async void Start()
        {
            InitTextLocalization();
            InitObjects();

            _initialized = true;
            OnResizeWindow();
            bool visibleInputFields = false;
            try
            {
                string refreshToken = SecureStorageProvider.GetString(SecureStorageKey.RefreshToken);
                //AuthHelper.LogRefreshToken(refreshToken);
                DateTimeOffset? refreshTokenExpirationAt = SecureStorageProvider.GetDateTimeOffset(SecureStorageKey.RefreshTokenExpirationAt);
                if (string.IsNullOrWhiteSpace(refreshToken)
                    || refreshTokenExpirationAt == null
                    || refreshTokenExpirationAt.Value < DateTimeOffset.UtcNow)
                {
                    // нет токена обновления или он просрочен
                    visibleInputFields = true;
                    AuthHelper.ClearTokenInSecureStorageProvider();
                    return;
                }

                await SetVisibleInputFieldsAsync(false);

                bool success = await AuthHelper.AuthAndLoadDataAsync(refreshToken: refreshToken);
                if (!success)
                {
                    visibleInputFields = true;
                    return;
                }
            }
            finally
            {
                if (visibleInputFields)
                {
                    GameObjectFinder.FindByName<TMP_InputField>("InputText_Email (id=96oaypns)").text = "SUPERadmin@mail.RU";
                    GameObjectFinder.FindByName<TMP_InputField>("InputText_Password (id=9vfnj9oh)").text = "testPassword";
                }
                SetVisibleInputFields(visibleInputFields);
                //AuthHelper.LogRefreshToken();

            }

            // Пример использования
            //string payloadJson = DecodeJwtPayload(accessToken);
            //Debug.Log(payloadJson);
            //InputManager.Register(KeyCode.Escape, GameExitHandler.ExitGame);
            //InputManager.Register(KeyCode.Return, PressLogin, key2: KeyCode.KeypadEnter);

            //GameMessage.Show("", true);
        }
        //private void OnDestroy()
        //{
        //    InputManager.Unregister(GameExitHandler.ExitGame);
        //    InputManager.Unregister(PressLogin);
        //}

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            if (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width))
            {
                OnResizeWindow();
            }
        }

        private void InitTextLocalization()
        {
            GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Email (id=ndtil638)").text = Game03Client.LocalizationManager.GetValue(L.UI.Label.Email);
            GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Password (id=e319ahd6)").text = Game03Client.LocalizationManager.GetValue(L.UI.Label.Password);
            GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonLogin (id=wf6fw0y1)").text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Login);
            GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonReg (id=tsuvx5vf)").text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Reg);
            GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonExitGame (id=flb78tua)").text = Game03Client.LocalizationManager.GetValue(L.UI.Button.ExitGame);
        }

        private void InitObjects()
        {
            _ButtonLogin_Button = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");

            _ButtonLogin_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_Login (id=bf6euydu)");
            _ButtonReg_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_Reg (id=4flrrger)");
            _ButtonExitGame_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_ExitGame (id=qn0sq5e5)");
            _InputTextWithLabelEmail_RectTransform = GameObjectFinder.FindByName<RectTransform>("InputTextWithLabel_Email (id=sejzo1c1)");
            _InputTextWithLabelPassword_RectTransform = GameObjectFinder.FindByName<RectTransform>("InputTextWithLabel_Password (id=0jyjud2d)");

            _LabelEmail_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Email (id=ndtil638)");
            _LabelPassword_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Password (id=e319ahd6)");
            _TextEmail_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_Email (id=n4tnenbq)");
            _TextPassword_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_Password (id=72r1zdv1)");
            _TextButtonLogin_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonLogin (id=wf6fw0y1)");
            _TextButtonReg_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonReg (id=tsuvx5vf)");
            _TextButtonExitGame_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonExitGame (id=flb78tua)");

            _ImageBackground_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=i16uj497)");
            if (_ImageBackground_Image.sprite != null)
            {
                Texture2D texture = _ImageBackground_Image.sprite.texture;
                _ImageBackground_CoefWH = texture.width / (float)texture.height;
            }
        }

        private void OnResizeWindow()
        {
            _height = Screen.height;
            _width = Screen.width;

            float coefHeight = _height / 1080f;
            float coefWidth = _width / 1920f;

            _ButtonLogin_RectTransform.anchoredPosition = new Vector2(0, -268.3601f * coefHeight);
            _ButtonLogin_RectTransform.sizeDelta = new Vector2(292.24f * coefHeight, 79.52002f * coefHeight);

            _ButtonReg_RectTransform.anchoredPosition = new Vector2(-20f * coefWidth, -400f * coefHeight);
            _ButtonExitGame_RectTransform.anchoredPosition = new Vector2(-20f * coefWidth, -473f * coefHeight);
            _ButtonExitGame_RectTransform.sizeDelta = _ButtonReg_RectTransform.sizeDelta = new Vector2(220f * coefHeight, 65f * coefHeight);

            _InputTextWithLabelEmail_RectTransform.anchoredPosition = new Vector2(0f, -48.29327f * coefHeight);
            _InputTextWithLabelPassword_RectTransform.anchoredPosition = new Vector2(0f, -160f * coefHeight);
            _InputTextWithLabelPassword_RectTransform.sizeDelta = _InputTextWithLabelEmail_RectTransform.sizeDelta = new Vector2(768f * coefHeight, 96.58661f * coefHeight);

            float fontSize = 36f * coefHeight;
            _LabelEmail_TextMeshProUGUI.fontSize = fontSize;
            _LabelPassword_TextMeshProUGUI.fontSize = fontSize;
            _TextEmail_TextMeshProUGUI.fontSize = fontSize;
            _TextPassword_TextMeshProUGUI.fontSize = fontSize;

            fontSize = 26f * coefHeight;
            _TextButtonLogin_TextMeshProUGUI.fontSize = fontSize;
            _TextButtonReg_TextMeshProUGUI.fontSize = fontSize;
            _TextButtonExitGame_TextMeshProUGUI.fontSize = fontSize;

            // Background
            float coefScreen = _width / _height;
            _ImageBackground_Image.rectTransform.sizeDelta = coefScreen > _ImageBackground_CoefWH
                ? new Vector2(_width, _width / _ImageBackground_CoefWH)
                : new Vector2(_height * _ImageBackground_CoefWH, _height);
        }

        private void SetVisibleInputFields(bool visible)
        {
            _ButtonLogin_Button.gameObject.SetActive(visible);
            _InputTextWithLabelEmail_RectTransform.gameObject.SetActive(visible);
            _InputTextWithLabelPassword_RectTransform.gameObject.SetActive(visible);
        }
        private async UniTask SetVisibleInputFieldsAsync(bool visible)
        {
            _ButtonLogin_Button.gameObject.SetActive(visible);
            _InputTextWithLabelEmail_RectTransform.gameObject.SetActive(visible);
            _InputTextWithLabelPassword_RectTransform.gameObject.SetActive(visible);
            await UniTask.Yield();
        }





    }
}
