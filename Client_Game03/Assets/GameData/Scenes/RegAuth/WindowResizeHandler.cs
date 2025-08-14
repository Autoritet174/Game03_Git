using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class WindowResizeHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform _panelLogin;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_Email_input;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_Email_label;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_Password_input;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_Password_label;

    [SerializeField]
    private RectTransform _panelButton;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_ButtonLogin;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_ButtonReg;

    [SerializeField]
    private TextMeshProUGUI textMeshPro_ButtonExit;

    [SerializeField]
    private Image imageBackground;
    private float imageBackgroundCoef = 1;

    private float _lastHeight;
    private float _lastWidth;
    private void Start()
    {
        //    textMeshPro_Email_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Email (uuid=9b99b098-1949-4b68-bba9-df3660bc95d4)");
        //    textMeshPro_Password_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Password (uuid=8003daed-ae09-43b9-b033-ae5bb5f5eb38)");
        //    textMeshPro_Email_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Email (uuid=d7e89f75-89bf-4fc0-b243-349e38906945)");
        //    textMeshPro_Password_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Password (uuid=e7f6163f-b580-4491-b5e0-8aa1c004c951)");

        if (imageBackground != null && imageBackground.sprite != null)
        {
            Texture2D texture = imageBackground.sprite.texture;
            imageBackgroundCoef = texture.width / (float)texture.height;
        }


        UpdatePanelAfterResize();
    }

    private void Update()
    {
        // Проверяем изменение высоты Canvas
        if (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth))
        {
            UpdatePanelAfterResize();
        }
    }

    private void UpdatePanelAfterResize()
    {
        // Фиксируем новую высоту
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        // _panelLogin
        float newHeight = _lastHeight / 3.375f;// = _lastHeight * 320f / 1080f;
        _panelLogin.sizeDelta = new Vector2(newHeight * 2.4f, newHeight);
        _panelLogin.anchoredPosition = new Vector2(0, newHeight / -2);

        float fontSizeCoef = _lastHeight / 1080f;

        float fontSize = 36 * fontSizeCoef;
        textMeshPro_Email_input.fontSize = fontSize;
        textMeshPro_Password_input.fontSize = fontSize;
        textMeshPro_Email_label.fontSize = fontSize;
        textMeshPro_Password_label.fontSize = fontSize;


        // _panelButton
        float width = _lastHeight / 1.125f; // = _lastHeight * 960 / 1080;
        _panelButton.sizeDelta = new Vector2(width, _lastHeight);
        _panelButton.anchoredPosition = new Vector2((Screen.width - width) / 2, 0);


        fontSize = 26 * fontSizeCoef;
        textMeshPro_ButtonLogin.fontSize = fontSize;
        textMeshPro_ButtonReg.fontSize = fontSize;
        textMeshPro_ButtonExit.fontSize = fontSize;


        // Background
        float coefScreen = _lastWidth / _lastHeight;// 10000/1000 = 10
        imageBackground.rectTransform.sizeDelta = coefScreen > imageBackgroundCoef ? new Vector2(_lastWidth, _lastWidth / imageBackgroundCoef) : new Vector2(_lastHeight * imageBackgroundCoef, _lastHeight);

    }
}
