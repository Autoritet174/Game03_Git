using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class WindowMessageController : MonoBehaviour
{
    private TextMeshProUGUI labelText;
    private GameObject buttonGameObject;
    private GameObject canvasGameObject;


    private void Start()
    {
        UpdateComponents();
    }

    void UpdateComponents()
    {
        if (canvasGameObject == null || labelText == null || buttonGameObject == null)
        {
            Canvas canvas = GetComponent<Canvas>();
            canvasGameObject = canvas.gameObject;
            labelText = GetComponentInChildren<TextMeshProUGUI>(true);
            Button button = GetComponentInChildren<Button>(true);
            buttonGameObject = button.gameObject;
        }
    }
    public void SetText(string text, bool buttonActive)
    {
        UpdateComponents();
        UnityMainThreadDispatcher.RunOnMainThread(() =>
        {
            canvasGameObject.SetActive(true);
            labelText.text = text;
            buttonGameObject.SetActive(buttonActive);
        });
      
    }

    public void Hide() {
        UpdateComponents();
        UnityMainThreadDispatcher.RunOnMainThread(() =>
        {
            canvasGameObject.SetActive(false);
        });
    }
}
