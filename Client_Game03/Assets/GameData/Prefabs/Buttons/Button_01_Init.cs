using TMPro;
using UnityEngine;

public class Button_01_Init : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro_Button;

    private void Awake()
    {
        float h2 = textMeshPro_Button.GetComponent<RectTransform>().rect.height;
        textMeshPro_Button.fontSize = h2 * 0.33f;
    }
}
