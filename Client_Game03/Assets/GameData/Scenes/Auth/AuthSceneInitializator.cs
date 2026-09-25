using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Auth
{
    /// <summary>Подготавливает интерфейс авторизации и восстанавливает сохранённый сеанс.</summary>
    public class AuthSceneInitializator : MonoBehaviour
    {

        private bool initialized = false;
        private float width, height;

        private Button buttonLogin_Button;

        private RectTransform buttonLogin_RectTransform;
        private RectTransform buttonReg_RectTransform;
        private RectTransform buttonExitGame_RectTransform;
        private RectTransform inputTextWithLabelEmail_RectTransform;
        private RectTransform inputTextWithLabelPassword_RectTransform;
        private TextMeshProUGUI labelEmail_TextMeshProUGUI;
        private TextMeshProUGUI labelPassword_TextMeshProUGUI;
        private TextMeshProUGUI textEmail_TextMeshProUGUI;
        private TextMeshProUGUI textPassword_TextMeshProUGUI;
        private TextMeshProUGUI textButtonLogin_TextMeshProUGUI;
        private TextMeshProUGUI textButtonReg_TextMeshProUGUI;
        private TextMeshProUGUI textButtonExitGame_TextMeshProUGUI;

        private Image imageBackground_Image;
        private float imageBackground_CoefWH = 1f;

        #region Жизненный цикл сцены

        private void Start()
        {
            InitTextLocalization();
            InitObjects();

            initialized = true;
            OnResizeWindow();
            this.RunAsync(StartAsync);
        }

        private async UniTask StartAsync(CancellationToken cancellationToken)
        {
            bool visibleInputFields = false;
            try
            {
                string refreshToken = SecureStorageProvider.GetString(ESecureStorageKey.refreshToken);
                //AuthHelper.LogRefreshToken(refreshToken);
                DateTimeOffset? refreshTokenExpirationAt = SecureStorageProvider.GetDateTimeOffset(ESecureStorageKey.refreshTokenExpirationAt);
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
                if (visibleInputFields
                    && DevAuthConfig.TryGetPrefillCredentials(out string devEmail, out string devPassword))
                {
                    GameObjectFinder.FindByName<TMP_InputField>("InputText_Email (id=96oaypns)").text = devEmail;
                    GameObjectFinder.FindByName<TMP_InputField>("InputText_Password (id=9vfnj9oh)").text = devPassword;
                }
                SetVisibleInputFields(visibleInputFields);
                //AuthHelper.LogRefreshToken();

            }
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            if (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width))
            {
                OnResizeWindow();
            }
        }

        #endregion Жизненный цикл сцены

        #region Подготовка и раскладка интерфейса

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
            buttonLogin_Button = GameObjectFinder.FindByName<Button>("Button_Login (id=bf6euydu)");

            buttonLogin_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_Login (id=bf6euydu)");
            buttonReg_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_Reg (id=4flrrger)");
            buttonExitGame_RectTransform = GameObjectFinder.FindByName<RectTransform>("Button_ExitGame (id=qn0sq5e5)");
            inputTextWithLabelEmail_RectTransform = GameObjectFinder.FindByName<RectTransform>("InputTextWithLabel_Email (id=sejzo1c1)");
            inputTextWithLabelPassword_RectTransform = GameObjectFinder.FindByName<RectTransform>("InputTextWithLabel_Password (id=0jyjud2d)");

            labelEmail_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Email (id=ndtil638)");
            labelPassword_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_Password (id=e319ahd6)");
            textEmail_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_Email (id=n4tnenbq)");
            textPassword_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_Password (id=72r1zdv1)");
            textButtonLogin_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonLogin (id=wf6fw0y1)");
            textButtonReg_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonReg (id=tsuvx5vf)");
            textButtonExitGame_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text_ButtonExitGame (id=flb78tua)");

            imageBackground_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=i16uj497)");
            if (imageBackground_Image.sprite != null)
            {
                Texture2D texture = imageBackground_Image.sprite.texture;
                imageBackground_CoefWH = texture.width / (float)texture.height;
            }
        }

        private void OnResizeWindow()
        {
            height = Screen.height;
            width = Screen.width;

            float coefHeight = height / 1080f;
            float coefWidth = width / 1920f;

            buttonLogin_RectTransform.anchoredPosition = new(0, -268.3601f * coefHeight);
            buttonLogin_RectTransform.sizeDelta = new(292.24f * coefHeight, 79.52002f * coefHeight);

            buttonReg_RectTransform.anchoredPosition = new(-20f * coefWidth, -400f * coefHeight);
            buttonExitGame_RectTransform.anchoredPosition = new(-20f * coefWidth, -473f * coefHeight);
            buttonExitGame_RectTransform.sizeDelta = buttonReg_RectTransform.sizeDelta = new(220f * coefHeight, 65f * coefHeight);

            inputTextWithLabelEmail_RectTransform.anchoredPosition = new(0f, -48.29327f * coefHeight);
            inputTextWithLabelPassword_RectTransform.anchoredPosition = new(0f, -160f * coefHeight);
            inputTextWithLabelPassword_RectTransform.sizeDelta = inputTextWithLabelEmail_RectTransform.sizeDelta = new(768f * coefHeight, 96.58661f * coefHeight);

            float fontSize = 36f * coefHeight;
            labelEmail_TextMeshProUGUI.fontSize = fontSize;
            labelPassword_TextMeshProUGUI.fontSize = fontSize;
            textEmail_TextMeshProUGUI.fontSize = fontSize;
            textPassword_TextMeshProUGUI.fontSize = fontSize;

            fontSize = 26f * coefHeight;
            textButtonLogin_TextMeshProUGUI.fontSize = fontSize;
            textButtonReg_TextMeshProUGUI.fontSize = fontSize;
            textButtonExitGame_TextMeshProUGUI.fontSize = fontSize;

            // Background
            float coefScreen = width / height;
            imageBackground_Image.rectTransform.sizeDelta = coefScreen > imageBackground_CoefWH
                ? new Vector2(width, width / imageBackground_CoefWH)
                : new Vector2(height * imageBackground_CoefWH, height);
        }

        #endregion Подготовка и раскладка интерфейса

        #region Видимость полей ввода

        private void SetVisibleInputFields(bool visible)
        {
            buttonLogin_Button.gameObject.SetActive(visible);
            inputTextWithLabelEmail_RectTransform.gameObject.SetActive(visible);
            inputTextWithLabelPassword_RectTransform.gameObject.SetActive(visible);
        }

        private async UniTask SetVisibleInputFieldsAsync(bool visible)
        {
            buttonLogin_Button.gameObject.SetActive(visible);
            inputTextWithLabelEmail_RectTransform.gameObject.SetActive(visible);
            inputTextWithLabelPassword_RectTransform.gameObject.SetActive(visible);
            await UniTask.Yield();
        }

        #endregion Видимость полей ввода

    }
}
