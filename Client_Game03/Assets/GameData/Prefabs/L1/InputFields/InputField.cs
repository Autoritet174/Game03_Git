using UnityEngine;
using TMPro;

public class InputField : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMeshPro;

    private void Awake() {
        //textMeshPro.fontSize = GetComponent<RectTransform>().rect.height / 1.8f;
    }
}