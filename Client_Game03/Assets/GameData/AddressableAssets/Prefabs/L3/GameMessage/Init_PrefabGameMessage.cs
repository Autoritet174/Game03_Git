using TMPro;
using UnityEngine;

public class Init_PrefabGameMessage : MonoBehaviour
{
    private bool _initialized = false;
    private float _width, _height;
    [SerializeField]
    private RectTransform frame_RectTransform;

    [SerializeField]
    private RectTransform buttonOk_RectTransform;
    [SerializeField]
    private TextMeshProUGUI buttonOk_TextMeshProUGUI;

    [SerializeField]
    private RectTransform buttonYes_RectTransform;
    [SerializeField]
    private TextMeshProUGUI buttonYes_TextMeshProUGUI;

    [SerializeField]
    private RectTransform buttonNo_RectTransform;
    [SerializeField]
    private TextMeshProUGUI buttonNo_TextMeshProUGUI;

    [SerializeField]
    private RectTransform mainTextLabel_RectTransform;
    [SerializeField]
    private TextMeshProUGUI mainTextLabel_TextMeshProUGUI;
    private float _mainTextLabel_height;
    private string mainTextLabel_LastText = string.Empty;

    private void Start()
    {
        _initialized = true;
        OnResizeWindow();
    }

    private void Update()
    {
        if (!_initialized) {
            return;
        }

        if (_mainTextLabel_height != mainTextLabel_RectTransform.rect.height)
        {
            _mainTextLabel_height = mainTextLabel_RectTransform.rect.height;
        }

        bool onResizeWindow = false;
        if (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width))
        {
            onResizeWindow = true;
        }
        if (!onResizeWindow && mainTextLabel_LastText !=mainTextLabel_TextMeshProUGUI.text)
        {
            mainTextLabel_LastText = mainTextLabel_TextMeshProUGUI.text;
            onResizeWindow = true;
        }

        if (onResizeWindow)
        {
            OnResizeWindow();
        }
    }

    private void OnResizeWindow()
    {
        _height = Screen.height;
        _width = Screen.width;
        float coefHeight = _height / 1080f;

        // 1. Установка ширины фрейма
        float frameWidth = 1240 * coefHeight;
        if (frameWidth > _width)
        {
            frameWidth = _width;
        }

        // 2. Установка ширины и top лабела
        mainTextLabel_RectTransform.sizeDelta = new Vector2(1192f/ 1240f * frameWidth, 0);//высота на авто
        float labelTop = (7f * coefHeight) + 13f;
        mainTextLabel_RectTransform.anchoredPosition = new Vector2(0, -labelTop);

        // 3. Размеры и позиция всех кнопок
        float buttonHeight = 74f * coefHeight;
        buttonNo_RectTransform.sizeDelta
            = buttonYes_RectTransform.sizeDelta
            = buttonOk_RectTransform.sizeDelta
            = new Vector2(248f * coefHeight, buttonHeight);

        float buttonBottom = (7f * coefHeight) + 13f;//13 количество неизменяемых пикселей в рамке
        buttonOk_RectTransform.anchoredPosition = new Vector2(0, buttonBottom);
        buttonYes_RectTransform.anchoredPosition = new Vector2(-183f * coefHeight, buttonBottom);
        buttonNo_RectTransform.anchoredPosition = new Vector2(183f * coefHeight, buttonBottom);

        // 4. Размеры шрифтов
        float buttonFontSize = 24.75f * coefHeight;
        buttonOk_TextMeshProUGUI.fontSize = buttonFontSize;
        buttonYes_TextMeshProUGUI.fontSize = buttonFontSize;
        buttonNo_TextMeshProUGUI.fontSize = buttonFontSize;
        mainTextLabel_TextMeshProUGUI.fontSize = 36f * coefHeight;


        float emptyRowHeight = (220 - 20f - 40.22f- 74f - 20f) * coefHeight;

        float frameHeight = emptyRowHeight + labelTop + _mainTextLabel_height + buttonHeight + buttonBottom;

        Debug.Log(_mainTextLabel_height);
        frame_RectTransform.sizeDelta = new Vector2(frameWidth, frameHeight);
    }
}
