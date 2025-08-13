using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class WindowResizeHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform _panelRect;

    [SerializeField]
    TextMeshProUGUI textMeshPro_Email_input;

    [SerializeField]
    TextMeshProUGUI textMeshPro_Email_label;

    [SerializeField]
    TextMeshProUGUI textMeshPro_Password_input;

    [SerializeField]
    TextMeshProUGUI textMeshPro_Password_label;


    private float _lastHeight;
    void Start()
    {
    //    textMeshPro_Email_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Email (uuid=9b99b098-1949-4b68-bba9-df3660bc95d4)");
    //    textMeshPro_Password_input = GameObjectFinder.FindTMPInputFieldByName("InputText_Password (uuid=8003daed-ae09-43b9-b033-ae5bb5f5eb38)");
    //    textMeshPro_Email_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Email (uuid=d7e89f75-89bf-4fc0-b243-349e38906945)");
    //    textMeshPro_Password_label = GameObjectFinder.FindTextMeshProUGUIByName("Label_Password (uuid=e7f6163f-b580-4491-b5e0-8aa1c004c951)");

        UpdatePanelAfterResize();
    }

    void Update()
    {
        // Проверяем изменение высоты Canvas
        if (!Mathf.Approximately(Screen.height, _lastHeight))
        {
            UpdatePanelAfterResize();
        }
    }

    private void UpdatePanelAfterResize()
    {
        // Фиксируем новую высоту
        _lastHeight = Screen.height;

        float newHeight = _lastHeight * 320f / 1080f;

        // Рассчитываем новые параметры
        float newWidth = newHeight * 2.4f;
        float newY = newHeight / (-2);

        // Применяем изменения
        _panelRect.sizeDelta = new Vector2(newWidth, newHeight);
        _panelRect.anchoredPosition = new Vector2(0, newY);

        float fontSize = 36 * _lastHeight / 1080f;

        textMeshPro_Email_input.fontSize = fontSize;
        textMeshPro_Password_input.fontSize = fontSize;
        textMeshPro_Email_label.fontSize = fontSize;
        textMeshPro_Password_label.fontSize = fontSize;

        //Debug.Log($"Обновлено: H={newHeight}, W={newWidth}, Y={newY}");
    }
}
