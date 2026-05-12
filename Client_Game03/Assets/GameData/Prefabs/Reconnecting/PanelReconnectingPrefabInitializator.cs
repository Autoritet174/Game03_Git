using Assets.GameData.Scripts;
using Game03Client;
using System.Net.WebSockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

public class PanelReconnectingPrefabInitializator : MonoBehaviour
{
    private bool _Initialized = false;
    private float Width = 0f, Height = 0f;
    private RectTransform Panel__RectTransform;

    private Button ButtonCancel__Button;
    private RectTransform ButtonCancel__RectTransform;
    private TextMeshProUGUI ButtonCancel__TextMeshProUGUI;

    private RectTransform LabelReconnecting__RectTransform;
    private TextMeshProUGUI LabelReconnecting__TextMeshProUGUI;

    private GameObject canvas__GameObject;
    private bool visible = true;
    private string textConnectionLost;
    private string textReconnecting;
    private string textTry;
    private string textAfter;

    private void Start()
    {
        GameObject PanelReconnecting = GameObjectFinder.FindByName("PanelReconnecting");

        Canvas canvas = GameObjectFinder.FindByName<Canvas>("Canvas", PanelReconnecting.transform);
        canvas.worldCamera = Camera.main;

        canvas__GameObject = canvas.gameObject;

        Panel__RectTransform = GameObjectFinder.FindByName<RectTransform>("Panel", PanelReconnecting.transform);

        ButtonCancel__Button = GameObjectFinder.FindByName<Button>("ButtonCancel", PanelReconnecting.transform);
        ButtonCancel__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonCancel", PanelReconnecting.transform);
        ButtonCancel__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", ButtonCancel__RectTransform.transform);
        ButtonCancel__TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Cancel);

        LabelReconnecting__RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelReconnecting", PanelReconnecting.transform);
        LabelReconnecting__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelReconnecting", PanelReconnecting.transform);

        textConnectionLost = Game03Client.LocalizationManager.GetValue(L.UI.Label.ConnectionLost);
        textReconnecting = Game03Client.LocalizationManager.GetValue(L.UI.Label.Reconnecting);
        textTry = Game03Client.LocalizationManager.GetValue(L.UI.Label.Try);
        textAfter = Game03Client.LocalizationManager.GetValue(L.UI.Label.After).ToLowerInvariant();

        UpdateState();

        ButtonCancel__Button.onClick.RemoveAllListeners();
        ButtonCancel__Button.onClick.AddListener(()=> {
            WebSocketProvider.DisconnectAsync();
            GameSceneManager.Load(GameSceneManager.SceneName.Auth);
        });

        _Initialized = true;
        OnResized();

        DontDestroyOnLoad(PanelReconnecting);
        Visible(false);
    }

    private void Visible(bool v)
    {
        canvas__GameObject.SetActive(v);
        visible = v;
    }

    private void Update()
    {
        if (_Initialized && (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width)))
        {
            OnResized();
        }
        UpdateState();
    }

    private void OnResized()
    {
        if (!_Initialized)
        {
            return;
        }

        Height = Screen.height;
        Width = Screen.width;

        float coefHeight = G.GetCoefHeight();

        Panel__RectTransform.sizeDelta = new Vector2(1200 * coefHeight, 400 * coefHeight);

        ButtonCancel__RectTransform.sizeDelta = new Vector2(512 * coefHeight, 128 * coefHeight);
        ButtonCancel__RectTransform.anchoredPosition = new Vector2(0, -89 * coefHeight);
        ButtonCancel__TextMeshProUGUI.fontSize = 40 * coefHeight;

        LabelReconnecting__RectTransform.sizeDelta = new Vector2(1200 * coefHeight, 200 * coefHeight);
        LabelReconnecting__RectTransform.anchoredPosition = new Vector2(0, 100 * coefHeight);
        LabelReconnecting__TextMeshProUGUI.fontSize = 60 * coefHeight;
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

            LabelReconnecting__TextMeshProUGUI.text = $"{textConnectionLost}\r\n{(sec <= 0 ? textReconnecting + "..." : $"{textTry} ({attempt}), {textAfter} {sec:0.0}")}";
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
