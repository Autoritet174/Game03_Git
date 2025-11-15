using Assets.GameData.Scripts;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectionScene_InitializatorUpdater : MonoBehaviour, IResizeWindow
{

    public bool Initialized { get; private set; }
    private const int ColorOffButtonRGBValue = 100;
    private static Color ColorOffButton = new(ColorOffButtonRGBValue / 255f, ColorOffButtonRGBValue / 255f, ColorOffButtonRGBValue / 255f);
    private bool initialized = false;
    private float _lastHeight;
    private float _lastWidth;

    private class TabButton
    {
        public readonly string name;
        public readonly RectTransform rectTransform;
        public readonly Button button;
        public readonly TextMeshProUGUI textMeshProUGUI;
        public readonly Image image;

        public TabButton(string name, string nameText, UnityAction action)
        {
            this.name = name;
            button = GameObjectFinder.FindByName<Button>(name);
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            image = GameObjectFinder.FindByName<Image>(name);
            textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(nameText);
            button.onClick.AddListener(action);
        }
    }


    private TabButton tabButtonHeroes, tabButtonItem;
    private Image imageBackground;
    private float imageBackgroundCoef = 1;

    private RectTransform rectTransformPanelTop;
    private RectTransform rectTransformButtonClose;
    private RectTransform rectTransformButtonCloseSelectedHero;
    private RectTransform rectTransformPanelSelectedHero;
    private RectTransform rectTransformPanelSelectedHeroTop;
    private TextMeshProUGUI SelectedHeroTop_TextMeshProUGUI;

    private readonly CollectionHero collectionHero = new();


    private async void Start()
    {
        tabButtonHeroes = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", OnClickHeroes);
        tabButtonItem = new("ButtonItems (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", OnClickItems);

        // Изображение заднего фона
        imageBackground = GameObjectFinder.FindByName<Image>("Image_Background (id=688x18dt)");
        if (imageBackground != null && imageBackground.sprite != null)
        {
            Texture2D texture = imageBackground.sprite.texture;
            imageBackgroundCoef = texture.width / (float)texture.height;
        }
        rectTransformPanelTop = GameObjectFinder.FindByName<RectTransform>("PanelTop (id=ibal8ya0)");
        rectTransformButtonClose = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=4nretdab)");
        rectTransformButtonCloseSelectedHero = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=0ursxw0e)");
        rectTransformPanelSelectedHero = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
        rectTransformPanelSelectedHeroTop = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroTop (id=0y6mrhc2)");
        SelectedHeroTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");



        initialized = true;
        OnResizeWindow();

        // Получить инвентарь героев
        Game03Client.HttpRequester.HttpRequesterResult httpRequesterProviderResult = await GlobalFields.ClientGame.HttpRequesterProvider.GetResponceAsync(General.Url.Inventory.Heroes);
        if (httpRequesterProviderResult == null)
        {
            Debug.Log("httpRequesterProviderResult == null");
            return;
        }
        JObject jObject = httpRequesterProviderResult.JObject;
        if (jObject == null)
        {
            Debug.Log("jObject == null");
            return;
        }
        JToken result = jObject["result"];
        foreach (JToken jToken in result)
        {
            Debug.Log($"{jToken["_id"]} | {jToken["o"]} | {jToken["h"]}");
        }
        //string s = jObject["token"]?.ToString() ?? string.Empty;
    }

    private void Update()
    {
        if (initialized && (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth)))
        {
            OnResizeWindow();
        }
    }

    public void OnClickHeroes()
    {
        OnClickTabButton(tabButtonHeroes);
        tabButtonHeroes.image.color = Color.white;

    }

    public void OnClickItems()
    {
        OnClickTabButton(tabButtonItem);


    }

    private void OnClickTabButton(TabButton tabButtonPressed)
    {
        if (tabButtonPressed.name == tabButtonHeroes.name)
        {
            tabButtonHeroes.image.color = Color.white;
            tabButtonItem.image.color = ColorOffButton;
        }
        else {
            tabButtonHeroes.image.color = ColorOffButton;
            tabButtonItem.image.color = Color.white;
        }
    }

    public void OnResizeWindow()
    {
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;
        float fontSizeCoef = _lastHeight / 1080f;
        float fontSize = 30f * fontSizeCoef;

        //buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
        //buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;

        // Изображение заднего фона
        float coefScreen = _lastWidth / _lastHeight;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
        imageBackground.rectTransform.sizeDelta = coefScreen > imageBackgroundCoef ? new Vector2(_lastWidth, _lastWidth / imageBackgroundCoef) : new Vector2(_lastHeight * imageBackgroundCoef, _lastHeight);


        // Верхняя панель
        float panelTopHeight = 0.08f * _lastHeight;


        // Кнопки вкладок
        float tabButtonWidth = panelTopHeight * 3.777777f; // = (0,17 * 1920) / (0,08 * 1080)
        tabButtonItem.rectTransform.sizeDelta = tabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, panelTopHeight);
        tabButtonItem.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
        tabButtonHeroes.textMeshProUGUI.fontSize = fontSize;
        tabButtonItem.textMeshProUGUI.fontSize = fontSize;


        // Кнопки "Закрыть"
        rectTransformButtonCloseSelectedHero.sizeDelta = rectTransformButtonClose.sizeDelta = new Vector2(panelTopHeight, panelTopHeight);


        // Панель выбранного героя. Верхняя панель
        float panelSelectedHeroWidth = rectTransformPanelSelectedHero.rect.width;
        rectTransformPanelSelectedHeroTop.sizeDelta = new Vector2(panelSelectedHeroWidth, panelTopHeight);


        // Выбранный герой. Лабел
        SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
        SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
        SelectedHeroTop_TextMeshProUGUI.fontSize = 40f * fontSizeCoef;
    }
}
