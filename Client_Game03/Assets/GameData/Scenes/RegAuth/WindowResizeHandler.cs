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

    //[SerializeField]
    //private RectTransform _panelButton;

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
        //    textMeshPro_Email_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Email (id=96oaypns)");
        //    textMeshPro_Password_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Password (id=9vfnj9oh)");
        //    textMeshPro_Email_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Email (id=ndtil638)");
        //    textMeshPro_Password_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Password (id=e319ahd6)");

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
        //_panelButton.sizeDelta = new Vector2(width, _lastHeight);
        //_panelButton.anchoredPosition = new Vector2((Screen.width - width) / 2, 0);


        fontSize = 26 * fontSizeCoef;
        textMeshPro_ButtonLogin.fontSize = fontSize;
        textMeshPro_ButtonReg.fontSize = fontSize;
        textMeshPro_ButtonExit.fontSize = fontSize;


        // Background
        float coefScreen = _lastWidth / _lastHeight;// 10000/1000 = 10
        imageBackground.rectTransform.sizeDelta = coefScreen > imageBackgroundCoef ? new Vector2(_lastWidth, _lastWidth / imageBackgroundCoef) : new Vector2(_lastHeight * imageBackgroundCoef, _lastHeight);
    }
    void ResizeImageBackground(RectTransform rectTransform) {

    }
}
