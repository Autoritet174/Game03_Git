using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_01 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    Image image;

    [SerializeField]
    TextMeshProUGUI textMeshPro;

    static Color colorHighlighted = new(0, 255, 255);
    static Color colorNormal = new(255, 255, 255);

    void Start() {
        image.enabled = false;
        textMeshPro.color = colorNormal;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //image.enabled = true;
        textMeshPro.color = colorHighlighted;
    }

    public void OnPointerExit(PointerEventData eventData) {
        //image.enabled = false;
        textMeshPro.color = colorNormal;
    }
}
