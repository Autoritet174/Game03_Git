using Assets.GameData.Scripts;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowMessageController : MonoBehaviour
{
    private TextMeshProUGUI labelText;
    private GameObject buttonGameObject;
    private GameObject canvasGameObject;


    private void Start()
    {
        UpdateComponents();
    }

    private void UpdateComponents()
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
    public void SetTextLocale(string keyLocalization, bool buttonActive)
    {
        SetText(LocalizationManager.GetValue(keyLocalization), buttonActive);
    }
    public void SetText(string text, bool buttonActive)
    {
        UnityMainThreadDispatcher.RunOnMainThread(() =>
        {
            UpdateComponents();
            canvasGameObject.SetActive(true);
            if (!buttonActive)
            {
                text += "...";
            }
            labelText.text = text;
            buttonGameObject.SetActive(buttonActive);
        });
    }
    public void SetTextErrorAndWriteExceptionInLog(Exception ex)
    {
        SetText("APP_EXCEPTION: An exception has occurred, see log file", true);
        Logger2.LogE(ex);
    }

    public void Hide()
    {
        UpdateComponents();
        UnityMainThreadDispatcher.RunOnMainThread(() =>
        {
            canvasGameObject.SetActive(false);
        });
    }
}
