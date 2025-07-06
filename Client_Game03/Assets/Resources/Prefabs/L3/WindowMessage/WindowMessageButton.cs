using UnityEngine;

public class WindowMessageButton : MonoBehaviour
{

    [SerializeField]
    private GameObject windowMessage_Canvas;

    public void OnClick()
    {
        windowMessage_Canvas.SetActive(false);
    }
}
