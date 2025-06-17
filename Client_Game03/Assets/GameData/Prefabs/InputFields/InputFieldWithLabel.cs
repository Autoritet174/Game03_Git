using TMPro;
using UnityEngine;

public class InputFieldWithLabel : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI textMeshPro_InputField;

    [SerializeField]
    TextMeshProUGUI textMeshPro_Label;

    private void Awake() {
        float h1 = textMeshPro_InputField.GetComponent<RectTransform>().rect.height;
        textMeshPro_InputField.fontSize = h1 * 0.895087f;

        float h2 = textMeshPro_Label.GetComponent<RectTransform>().rect.height;
        textMeshPro_Label.fontSize = h2 * 0.847457f;
    }
}
