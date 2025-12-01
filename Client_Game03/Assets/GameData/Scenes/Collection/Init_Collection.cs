using Assets.GameData.Scripts;
using Game03Client.PlayerCollection;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool _ScrollbarVertical_Active = false;
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

    private RectTransform panelTop_RectTransform;
    private RectTransform buttonClose_RectTransform;
    private RectTransform buttonCloseSelectedHero_RectTransform;
    private RectTransform panelSelectedHero_RectTransform;
    private RectTransform panelSelectedHeroTop_RectTransform;
    private RectTransform panelCollection_RectTransform;
    private RectTransform panelCollectionTopButtons_RectTransform;
    private RectTransform scrollViewCollection_RectTransform;
    private RectTransform scrollbarVertical_RectTransform;
    private GameObject scrollViewCollection_GameObject;
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

    private readonly List<GroupDivider> groupDividers = new();

    private async void Start()
    {
        GameMessage.ShowLocale(L.Info.LoadingCollection, false);
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
            panelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop (id=ibal8ya0)");
            buttonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=4nretdab)");
            buttonCloseSelectedHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=0ursxw0e)");
            panelSelectedHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
            panelSelectedHeroTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroTop (id=0y6mrhc2)");
            SelectedHeroTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");

            // Панель для внутренних кнопок
            panelCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
            panelCollectionTopButtons_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionTopButtons (id=gmzb0h9f)");

            // Внутренние кнопки
            internalPanelHeroes = new("ImageButtonHeroes (id=pakco5ud)");
            internalPanelItems = new("ImageButtonItems (id=vuhjngaz)");
            internalPanelFilter = new("ImageButtonFilter (id=vjeqfzen)");
            internalPanelGroup = new("ImageButtonGroup (id=hbsaogwl)");
            internalPanelSort = new("ImageButtonSort (id=6nvcsrdm)");

            // Scroll View для коллекции героев
            scrollViewCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
            scrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
            scrollViewCollection_GameObject = scrollViewCollection_RectTransform.gameObject;

            // Коллекция контент
            transformCollectionContent = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;


            _initialized = true;
            OnResizeWindow();
            await Task.Yield();

            groupDividers.Clear();
            //List<Task> tasks = new();
            foreach (GroupHeroes item in G.Game.Collection.GetCollectionHeroesGroupByGroups().OrderByDescending(static a => a.Priority))
            {
                if (item.List.Count() > 0)
                {
                    GameObject groupDividerGameObject = (await Addressables.LoadAssetAsync<GameObject>($"GroupDividerPrefab").Task).SafeInstant();
                    groupDividerGameObject.transform.SetParent(transformCollectionContent, false);
                    GroupDivider groupDivider = new(item.Name, groupDividerGameObject, item.List);
                    groupDividers.Add(groupDivider);

                    OnResizeWindow();
                    //await Task.Yield();

                    await groupDivider.Init();
                }
                //tasks.Add(task);
            }
            //await Task.WhenAll(tasks);

            OnResizeWindow();
        }
        finally
        {
            GameMessage.Close();
        }
    }

    private void Update()
    {
        if (!_initialized)
        {
            return;
        }
        bool resize = false;
        if (_ScrollbarVertical_Active != scrollViewCollection_GameObject.activeInHierarchy)
        {
            _ScrollbarVertical_Active = scrollViewCollection_GameObject.activeInHierarchy;
            resize = false;
        }
        if (!resize && (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width)))
        {
            resize = true;
        }

        if (resize)
        {
            OnResizeWindow();
        }
    }

    private void OnClickHeroes()
    {
        OnClickTabButton(tabButtonHeroes);
        tabButtonHeroes.image.color = Color.white;

    }

    private void OnClickItems()
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

    private void OnResizeWindow()
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
        float panelTopHeight = topPanelHeightPercent * _height; // 86.4
        Vector2 vector008PercentOfHeight = new(panelTopHeight, panelTopHeight);
        panelTop_RectTransform.sizeDelta = new Vector2(_width, panelTopHeight);


        // Кнопки вкладок
        float tabButtonWidth = panelTopHeight * 3.777777f; // = (0,17 * 1920) / (0,08 * 1080)
        tabButtonItem.rectTransform.sizeDelta = tabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, panelTopHeight);
        tabButtonItem.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
        tabButtonHeroes.textMeshProUGUI.fontSize = fontSize;
        tabButtonItem.textMeshProUGUI.fontSize = fontSize;


        // Кнопки "Закрыть"
        buttonClose_RectTransform.sizeDelta = vector008PercentOfHeight;
        buttonCloseSelectedHero_RectTransform.sizeDelta = vector008PercentOfHeight;


        // Панель выбранного героя
        bool panelSelectedHeroActive = panelSelectedHero_RectTransform.gameObject.activeInHierarchy;
        float panelSelectedHeroWidth;
        if (panelSelectedHeroActive)
        {
            panelSelectedHeroWidth = 576 * coefHeight;

            // Панель выбранного героя
            panelSelectedHero_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 994 * coefHeight);

            // Панель выбранного героя. Верхняя панель где написано имя героя
            panelSelectedHeroTop_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, panelTopHeight);

            // Выбранный герой. Лабел
            SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
            //SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
            SelectedHeroTop_TextMeshProUGUI.fontSize = 40f * coefHeight;
        }
        else
        {
            panelSelectedHeroWidth = 0;
        }

        // Панель коллекции
        float panelCollection_Width = _width - panelSelectedHeroWidth;
        panelCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, _height - panelTopHeight);

        // Панель верхних кнопок
        panelCollectionTopButtons_RectTransform.sizeDelta = new Vector2(panelCollection_Width, 113f * coefHeight);

        // Внутренние кнопки
        internalPanelHeroes.Refresh(coefHeight, vector008PercentOfHeight, 10);
        internalPanelItems.Refresh(coefHeight, vector008PercentOfHeight, 10);
        internalPanelFilter.Refresh(coefHeight, vector008PercentOfHeight, 150);
        internalPanelGroup.Refresh(coefHeight, vector008PercentOfHeight, 256);
        internalPanelSort.Refresh(coefHeight, vector008PercentOfHeight, 362);

        // Scroll View для коллекции героев
        scrollViewCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, panelCollection_RectTransform.sizeDelta.y - panelCollectionTopButtons_RectTransform.sizeDelta.y);

        // ScrollbarVertical для коллекции героев
        scrollbarVertical_RectTransform.sizeDelta = new Vector2(32f * coefHeight, scrollViewCollection_RectTransform.sizeDelta.y);

        // groupDividers
        if (groupDividers.Count > 0)
        {
            groupDividers.ForEach(a => a.Resize(panelCollection_Width, coefHeight));
        }

    }
}
