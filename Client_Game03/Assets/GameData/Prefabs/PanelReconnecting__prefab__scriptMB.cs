using Assets.GameData.Scripts;
using Game03Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

/// <summary>Отображает состояние восстановления соединения и кнопку отмены.</summary>
public class PanelReconnecting__prefab__scriptMB : MonoBehaviour
{
    private bool initialized = false;
    private float width = 0f, height = 0f;
    private RectTransform panel__RectTransform;

    private Button buttonCancel__Button;
    private RectTransform buttonCancel__RectTransform;
    private TextMeshProUGUI buttonCancel__TextMeshProUGUI;

    private RectTransform labelReconnecting__RectTransform;
    private TextMeshProUGUI labelReconnecting__TextMeshProUGUI;

    private GameObject canvas__GameObject;
    private bool visible = true;
    private string textConnectionLost;
    private string textReconnecting;
    private string textTry;
    private string textAfter;

    private void Start()
    {
        GameObject panelReconnecting = GameObjectFinder.FindByName("PanelReconnecting");

        Canvas canvas = GameObjectFinder.FindByName<Canvas>("Canvas", panelReconnecting.transform);
        canvas.worldCamera = Camera.main;

        canvas__GameObject = canvas.gameObject;

        panel__RectTransform = GameObjectFinder.FindByName<RectTransform>("Panel", panelReconnecting.transform);

        buttonCancel__Button = GameObjectFinder.FindByName<Button>("ButtonCancel", panelReconnecting.transform);
        buttonCancel__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonCancel", panelReconnecting.transform);
        buttonCancel__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", buttonCancel__RectTransform.transform);
        buttonCancel__TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Cancel);

        labelReconnecting__RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelReconnecting", panelReconnecting.transform);
        labelReconnecting__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelReconnecting", panelReconnecting.transform);

        textConnectionLost = Game03Client.LocalizationManager.GetValue(L.UI.Label.ConnectionLost);
        textReconnecting = Game03Client.LocalizationManager.GetValue(L.UI.Label.Reconnecting);
        textTry = Game03Client.LocalizationManager.GetValue(L.UI.Label.Try);
        textAfter = Game03Client.LocalizationManager.GetValue(L.UI.Label.After).ToLowerInvariant();

        UpdateState();

        buttonCancel__Button.onClick.RemoveAllListeners();
        buttonCancel__Button.onClick.AddListener(() =>
        {
            _ = WebSocketProvider.DisconnectAsync();
            GameSceneManager.Load(GameSceneManager.ESceneName.auth);
        });

        initialized = true;
        OnResized();

        DontDestroyOnLoad(panelReconnecting);
        Visible(false);
    }

    private void Visible(bool v)
    {
        canvas__GameObject.SetActive(v);
        visible = v;
    }

    private void Update()
    {
        if (initialized && (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width)))
        {
            OnResized();
        }
        UpdateState();
    }

    private void OnResized()
    {
        if (!initialized)
        {
            return;
        }

        height = Screen.height;
        width = Screen.width;

        float coefHeight = G.GetCoefHeight();

        panel__RectTransform.sizeDelta = new(1200 * coefHeight, 400 * coefHeight);

        buttonCancel__RectTransform.sizeDelta = new(512 * coefHeight, 128 * coefHeight);
        buttonCancel__RectTransform.anchoredPosition = new(0, -89 * coefHeight);
        buttonCancel__TextMeshProUGUI.fontSize = 40 * coefHeight;

        labelReconnecting__RectTransform.sizeDelta = new(1200 * coefHeight, 200 * coefHeight);
        labelReconnecting__RectTransform.anchoredPosition = new(0, 100 * coefHeight);
        labelReconnecting__TextMeshProUGUI.fontSize = 60 * coefHeight;
    }

    private void UpdateState()
    {
        if (Game03Client.WebSocketProvider.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Reconnecting)
        {
            if (!visible)
            {
                Visible(true);
            }

            double sec = Game03Client.WebSocketProvider.retryPolicy?.SecondsUntilNextAttempt ?? 0;
            long attempt = Game03Client.WebSocketProvider.retryPolicy?.CurrentAttemptCount ?? 1;

            labelReconnecting__TextMeshProUGUI.text = $"{textConnectionLost}\r\n{(sec <= 0 ? textReconnecting + "..." : $"{textTry} ({attempt}), {textAfter} {sec:0.0}")}";
        }
        else
        {
            if (visible)
            {
                Visible(false);
            }
        }

    }
}
