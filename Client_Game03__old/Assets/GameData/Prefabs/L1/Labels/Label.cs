using TMPro;
using UnityEngine;

public class Label : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI textMeshPro_Label;

    private void Awake() {

        float h2 = textMeshPro_Label.GetComponent<RectTransform>().rect.height;
        textMeshPro_Label.fontSize = h2 * 0.847457f;
    }
}
