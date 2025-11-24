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

    string mainTextLabel_LastText = string.Empty;

    private void Start()
    {
    }

    private void Update()
    {
        if (!_initialized) {
            return;
        }

        bool onResizeWindow = false;
        if (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width))
        {
            onResizeWindow = true;
        }
        if (mainTextLabel_LastText!=mainTextLabel_TextMeshProUGUI.text)
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
        float frame_width = 1240 * coefHeight;
        if (frame_width > _width)
        {
            frame_width = _width;
        }

        // 2. Установка ширины лабела
        mainTextLabel_RectTransform.sizeDelta = new Vector2(1192f * coefHeight, 0);//высота на авто

        // 3. Размеры и позиция всех кнопок
        buttonNo_RectTransform.sizeDelta
            = buttonYes_RectTransform.sizeDelta
            = buttonOk_RectTransform.sizeDelta
            = new Vector2(248f * coefHeight, 74f * coefHeight);

        float buttonHeight = 10f * coefHeight;
        buttonOk_RectTransform.anchoredPosition = new Vector2(0, buttonHeight);
        buttonYes_RectTransform.anchoredPosition = new Vector2(-183f * coefHeight, buttonHeight);
        buttonNo_RectTransform.anchoredPosition = new Vector2(183f * coefHeight, buttonHeight);

        // 4. Размеры шрифтов
        float buttonFontSize = 24.75f * coefHeight;
        buttonOk_TextMeshProUGUI.fontSize = buttonFontSize;
        buttonYes_TextMeshProUGUI.fontSize = buttonFontSize;
        buttonNo_TextMeshProUGUI.fontSize = buttonFontSize;
        mainTextLabel_TextMeshProUGUI.fontSize = 36f * coefHeight;


        float emptyRowHeight = (220-20-40.22f-74-20) * coefHeight;


        frame_RectTransform.sizeDelta = new Vector2(frame_width, 0);
    }
}
