using Assets.GameData.Scripts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using L = General.LocalizationKeys;

public class Init_Collection : MonoBehaviour
{

    //public bool Initialized { get; private set; }
    private const int COLOR_OFF_BUTTON_RGB_VALUE = 100;
    private static Color ColorOffButton = new(COLOR_OFF_BUTTON_RGB_VALUE / 255f, COLOR_OFF_BUTTON_RGB_VALUE / 255f, COLOR_OFF_BUTTON_RGB_VALUE / 255f);
    private bool _initialized = false;
    private float _width, _height;

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

    /// <summary>
    /// Внутренняя панель, кнопки.
    /// </summary>
    private class InternalPanelButton
    {
        private readonly RectTransform rectTransform;
        private readonly RectTransform rectTransformButton;
        private readonly RectTransform rectTransformLabel;
        private readonly TextMeshProUGUI textMeshProUGUILabel;
        public InternalPanelButton(string name)
        {
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            rectTransformButton = GameObjectFinder.FindByName<RectTransform>("Button", rectTransform.transform);
            rectTransformLabel = GameObjectFinder.FindByName<RectTransform>("Label", rectTransform.transform);
            textMeshProUGUILabel = GameObjectFinder.FindByName<TextMeshProUGUI>("Label", rectTransform.transform);
        }
        public void Refresh(float coefHeight, Vector2 vector008PercentOfHeight, float anchoredPositionX)
        {
            rectTransform.sizeDelta = vector008PercentOfHeight;
            rectTransform.anchoredPosition = new(anchoredPositionX * coefHeight, -5 * coefHeight);
            rectTransformButton.sizeDelta = new(77 * coefHeight, 77 * coefHeight);
            rectTransformLabel.sizeDelta = new(86 * coefHeight, 13 * coefHeight);
            rectTransformLabel.anchoredPosition = new(0, -86 * coefHeight);
            textMeshProUGUILabel.fontSize = 18 * coefHeight;
        }
    }

    private InternalPanelButton internalPanelHeroes;
    private InternalPanelButton internalPanelItems;
    private InternalPanelButton internalPanelFilter;
    private InternalPanelButton internalPanelGroup;
    private InternalPanelButton internalPanelSort;

    private Transform transformCollectionContent;


    private async void Start()
    {
        GameMessage.Show("Loading collection", false);
        try
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


            // Внутренние кнопки
            internalPanelHeroes = new("ImageButtonHeroes (id=pakco5ud)");
            internalPanelItems = new("ImageButtonItems (id=vuhjngaz)");
            internalPanelFilter = new("ImageButtonFilter (id=vjeqfzen)");
            internalPanelGroup = new("ImageButtonGroup (id=hbsaogwl)");
            internalPanelSort = new("ImageButtonSort (id=6nvcsrdm)");


            // Коллекция контент
            transformCollectionContent = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;


            _initialized = true;
            OnResizeWindow();

            CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(30));
            // Получить коллекцию героев игрока
            JObject jObject = await G.Game.HttpRequester.GetJObjectAsync(General.Url.Collection.Heroes, cancellationTokenSource.Token);
            if (jObject == null)
            {
                GameMessage.Show(L.Error.Server.InvalidResponse, true);
                return;
            }

            JToken heroes = jObject["heroes"];
            if (heroes == null)
            {
                Debug.Log("heroes == null");
                GameMessage.Show(L.Error.Server.InvalidResponse, true);
                return;
            }

            // Получить уникальные имена групп
            List<string> group_name_List = new();
            foreach (JToken hero in heroes)
            {
                //Debug.Log($"_id={j["_id"]}; owner_id={j["owner_id"]}; hero_id={j["hero_id"]}; health={j["health"]}; attack={j["attack"]}; speed={j["speed"]}; strength={j["strength"]}; agility={j["agility"]}; intelligence={j["intelligence"]}");
                string group_name = hero["group_name"]?.ToString()?.Trim() ?? string.Empty;
                if (group_name != string.Empty && !group_name_List.Contains(group_name))
                {
                    group_name_List.Add(group_name);
                }
            }

            string guidNoGroupStr = Guid.NewGuid().ToString();
            group_name_List.Add(guidNoGroupStr);

            // Добавить всех героев в список, для легкого удаления потом.
            List<JToken> allHeroesJToken = new();
            foreach (JToken j in heroes)
            {
                allHeroesJToken.Add(j);
            }

            Dictionary<string, List<JToken>> heroesByGroups = new();
            foreach (string group_name_L in group_name_List)
            {
                List<JToken> list = new();
                heroesByGroups.Add(group_name_L, list);

                if (group_name_L == guidNoGroupStr)
                {
                    list.AddRange(allHeroesJToken);
                    break;
                }

                for (int i = allHeroesJToken.Count - 1; i >= 0; i--)
                {
                    JToken j = allHeroesJToken[i];
                    string group_name = j["group_name"]?.ToString()?.Trim() ?? string.Empty;
                    if (group_name == group_name_L)
                    {
                        list.Add(j);
                        _ = allHeroesJToken.Remove(j);
                    }
                }
            }

            List<Task> tasks = new();
            foreach (KeyValuePair<string, List<JToken>> keyValue in heroesByGroups)
            {
                GameObject groupDividerGameObject = (await Addressables.LoadAssetAsync<GameObject>($"GroupDividerPrefab").Task).SafeInstant();
                groupDividerGameObject.transform.SetParent(transformCollectionContent, false);
                string group_name = keyValue.Key == guidNoGroupStr ? null : keyValue.Key;
                GroupDivider groupDivider_NoGroup = new(group_name);
                Task task = groupDivider_NoGroup.Init(groupDividerGameObject, keyValue.Value);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
        finally
        {
            GameMessage.Close();
        }

        //string s = jObject["token"]?.ToString() ?? string.Empty;
    }

    private void Update()
    {
        if (_initialized && (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width)))
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
        else
        {
            tabButtonHeroes.image.color = ColorOffButton;
            tabButtonItem.image.color = Color.white;
        }
    }

    public void OnResizeWindow()
    {
        _height = Screen.height;
        _width = Screen.width;
        float coefHeight = _height / 1080f;
        float fontSize = 30f * coefHeight;

        //buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
        //buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;

        // Изображение заднего фона
        float coefScreen = _width / _height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
        imageBackground.rectTransform.sizeDelta = coefScreen > imageBackgroundCoef ? new Vector2(_width, _width / imageBackgroundCoef) : new Vector2(_height * imageBackgroundCoef, _height);


        // Верхняя панель
        float topPanelHeightPercent = 0.08f;
        float panelTopHeight = topPanelHeightPercent * _height;
        Vector2 vector008PercentOfHeight = new(panelTopHeight, panelTopHeight);


        // Кнопки вкладок
        float tabButtonWidth = panelTopHeight * 3.777777f; // = (0,17 * 1920) / (0,08 * 1080)
        tabButtonItem.rectTransform.sizeDelta = tabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, panelTopHeight);
        tabButtonItem.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
        tabButtonHeroes.textMeshProUGUI.fontSize = fontSize;
        tabButtonItem.textMeshProUGUI.fontSize = fontSize;


        // Кнопки "Закрыть"
        rectTransformButtonClose.sizeDelta = vector008PercentOfHeight;
        rectTransformButtonCloseSelectedHero.sizeDelta = vector008PercentOfHeight;


        // Панель выбранного героя. Верхняя панель
        float panelSelectedHeroWidth = rectTransformPanelSelectedHero.rect.width;
        rectTransformPanelSelectedHeroTop.sizeDelta = new Vector2(panelSelectedHeroWidth, panelTopHeight);


        // Выбранный герой. Лабел
        SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
        SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
        SelectedHeroTop_TextMeshProUGUI.fontSize = 40f * coefHeight;


        // Внутренние кнопки
        internalPanelHeroes.Refresh(coefHeight, vector008PercentOfHeight, 10);
        internalPanelItems.Refresh(coefHeight, vector008PercentOfHeight, 10);
        internalPanelFilter.Refresh(coefHeight, vector008PercentOfHeight, 150);
        internalPanelGroup.Refresh(coefHeight, vector008PercentOfHeight, 256);
        internalPanelSort.Refresh(coefHeight, vector008PercentOfHeight, 362);

    }
}
