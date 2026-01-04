using TMPro;
using UnityEngine;

public class InputField : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        //textMeshPro.fontSize = GetComponent<RectTransform>().rect.height / 1.8f;
    }
}
