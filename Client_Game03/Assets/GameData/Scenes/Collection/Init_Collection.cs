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
    public static readonly Vector3 Vector3Selected = new(1.15f, 1.15f, 1);

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
        public void SetText(string text)
        {
            textMeshProUGUI.text = text;
        }
    }

    private TabButton _TabButtonHeroes, _TabButtonEquipment;
    private Image _Background_Image;
    private float _ImageBackgroundCoef = 1;

    private RectTransform _PanelTop_RectTransform;
    private RectTransform _ButtonClose_RectTransform;
    private RectTransform _ButtonCloseSelectedHero_RectTransform;
    private RectTransform _PanelSelectedHero_RectTransform;
    internal GameObject PanelSelectedHero_GameObject { get; private set; }
    private bool _PanelSelectedHero_isActive = false;
    private RectTransform _PanelSelectedHeroTop_RectTransform;
    private RectTransform _PanelSelectedHeroBottom_RectTransform;
    private RectTransform _PanelSelectedHeroBottomTabButton1_RectTransform;
    private RectTransform _PanelSelectedHeroBottomTabButton2_RectTransform;
    private TextMeshProUGUI _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI;
    private TextMeshProUGUI _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI;
    private RectTransform _PanelSelectedHeroBottomTab1_RectTransform;
    internal RectTransform PanelCollection_RectTransform { get; private set; }
    private RectTransform _PanelCollectionTopButtons_RectTransform;
    private RectTransform _ScrollViewCollection_RectTransform;
    private RectTransform _ScrollbarVertical_RectTransform;
    private GameObject _ScrollViewCollection_GameObject;
    private TextMeshProUGUI _SelectedHeroTop_TextMeshProUGUI;
    private RectTransform _SelectedHeroImageContainer_RectTransform;
    private Image _SelectedHero_Image;
    private Image _SelectedHeroRarity_Image;

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

    private InternalPanelButton _InternalPanelHeroes;
    private InternalPanelButton _InternalPanelItems;
    private InternalPanelButton _InternalPanelFilter;
    private InternalPanelButton _InternalPanelGroup;
    private InternalPanelButton _InternalPanelSort;

    private Transform _CollectionContent_Transform;

    private readonly List<GroupDivider> _GroupDividers = new();



    private class Slot
    {
        private readonly string name;
        private readonly int posX;
        private readonly int posY;
        private readonly RectTransform _RectTransform;
        private readonly TextMeshProUGUI _TextMeshProUGUI;

        public Slot(string name, int posX, int posY, Transform parent, string suffix = "")
        {
            this.name = name;
            this.posX = posX;
            this.posY = posY;

            _RectTransform = GameObjectFinder.FindByName<RectTransform>($"PanelSlot{name}{suffix}", parent);
            _TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelSlot", _RectTransform);
            string lKey = L.UI.Label.Slot.GetKey(name);
            string text = G.Game.LocalizationManager.GetValue(lKey);
            if (suffix != "")
            {
                text += " " + suffix;
            }
            _TextMeshProUGUI.text = text;
        }


        public void Resize(float coefHeight)
        {
            const float _TopLeftBase = 10f;
            const float widthBase = 76.363636f;
            const float heightBase = widthBase + (_TopLeftBase * 2);
            _RectTransform.sizeDelta = new Vector2(widthBase * coefHeight, heightBase * coefHeight);
            float x = (((widthBase + _TopLeftBase) * (posX - 1)) + _TopLeftBase) * coefHeight;
            float y = (((heightBase + _TopLeftBase) * (posY - 1)) + _TopLeftBase) * coefHeight;
            _RectTransform.anchoredPosition = new Vector2(x, -y);

            _TextMeshProUGUI.fontSize = 10.5f * coefHeight;
        }
    }
    private readonly List<Slot> _Slots = new();

    /// <summary>
    /// 1 - герои, 2 - экипировка
    /// </summary>
    public int CollectionMode { get; private set; } = 1;
    public Sprite[] Rarityes = new Sprite[0];

    private async void Start()
    {

        _TabButtonHeroes = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", OnClickHeroes);
        _TabButtonEquipment = new("ButtonItems (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", OnClickEquipment);
        _TabButtonHeroes.SetText($"{G.Game.LocalizationManager.GetValue(L.UI.Button.Heroes)} ({G.Game.Collection.GetCountHeroes()})");
        _TabButtonEquipment.SetText($"{G.Game.LocalizationManager.GetValue(L.UI.Button.Equipment)} ({-99999})");

        // Изображение заднего фона
        _Background_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=688x18dt)");
        if (_Background_Image != null && _Background_Image.sprite != null)
        {
            Texture2D texture = _Background_Image.sprite.texture;
            _ImageBackgroundCoef = texture.width / (float)texture.height;
        }
        _PanelTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop (id=ibal8ya0)");
        _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=4nretdab)");

        _PanelSelectedHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHero (id=vs2gi8c6)");
        PanelSelectedHero_GameObject = _PanelSelectedHero_RectTransform.gameObject;
        _PanelSelectedHero_isActive = PanelSelectedHero_GameObject.activeInHierarchy;

        Rarityes = new Sprite[]{
            await Addressables.LoadAssetAsync<Sprite>($"raritySelected").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity1").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity2").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity3").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity4").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity5").Task,
            await Addressables.LoadAssetAsync<Sprite>($"rarity6").Task,
        };

        _SelectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull (id=m5kn2f6p)");
        _SelectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=xami3s9q)");
        _SelectedHeroImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=1l6gscif)");

        _ButtonCloseSelectedHero_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=0ursxw0e)");
        _ButtonCloseSelectedHero_RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            PanelSelectedHero_GameObject.SetActive(false);
            foreach (GroupDivider a in _GroupDividers)
            {
                bool founded = false;
                foreach (GroupDivider.DataCollectionElement b in a.ListDataCollectionElement)
                {
                    if (b.Selected)
                    {
                        b.rectTransform.localScale = Vector3.one;
                        //b.imageRarity.sprite = Rarityes[(int)b.collectionHero.HeroBase.Rarity];
                        founded = true;
                        break;
                    }
                }
                if (founded)
                {
                    break;
                }
            }

            OnResizeWindow();
        });

        _PanelSelectedHeroTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroTop (id=0y6mrhc2)");
        _PanelSelectedHeroBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottom (id=wejn6493)");

        _PanelSelectedHeroBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=uiufd2wv)");
        _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=lf8q2aas)");
        _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI.SetText(G.Game.LocalizationManager.GetValue(L.UI.Button.Equipment));

        _PanelSelectedHeroBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=kzury0kd)");
        _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=6bjw6hi4)");
        _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

        _PanelSelectedHeroBottomTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottomTab1 (id=kn3yl79k)");
        _SelectedHeroTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");

        // Панель для внутренних кнопок
        PanelCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
        _PanelCollectionTopButtons_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionTopButtons (id=gmzb0h9f)");

        // Внутренние кнопки
        _InternalPanelHeroes = new("ImageButtonHeroes (id=pakco5ud)");
        _InternalPanelItems = new("ImageButtonItems (id=vuhjngaz)");
        _InternalPanelFilter = new("ImageButtonFilter (id=vjeqfzen)");
        _InternalPanelGroup = new("ImageButtonGroup (id=hbsaogwl)");
        _InternalPanelSort = new("ImageButtonSort (id=6nvcsrdm)");

        // Scroll View для коллекции героев
        _ScrollViewCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
        _ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
        _ScrollViewCollection_GameObject = _ScrollViewCollection_RectTransform.gameObject;

        // Коллекция контент
        _CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;

        _Slots.Clear();
        _Slots.Add(new Slot("Head", 1, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Shoulders", 2, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Chest", 3, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Hands", 4, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Legs", 5, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Feet", 6, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Waist", 7, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Wrist", 8, 1, _PanelSelectedHeroBottom_RectTransform));

        _Slots.Add(new Slot("Ring", 1, 2, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Ring", 2, 2, _PanelSelectedHeroBottom_RectTransform, "2"));
        _Slots.Add(new Slot("Ring", 3, 2, _PanelSelectedHeroBottom_RectTransform, "3"));
        _Slots.Add(new Slot("Ring", 4, 2, _PanelSelectedHeroBottom_RectTransform, "4"));
        _Slots.Add(new Slot("Trinket", 5, 2, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Trinket", 6, 2, _PanelSelectedHeroBottom_RectTransform, "2"));
        _Slots.Add(new Slot("Trinket", 7, 2, _PanelSelectedHeroBottom_RectTransform, "3"));
        _Slots.Add(new Slot("Trinket", 8, 2, _PanelSelectedHeroBottom_RectTransform, "4"));

        _Slots.Add(new Slot("LeftHand", 1, 3, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("RightHand", 2, 3, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Back", 3, 3, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Neck", 4, 3, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Bracelet", 5, 3, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Bracelet", 6, 3, _PanelSelectedHeroBottom_RectTransform, "2"));
        _Slots.Add(new Slot("Earring", 7, 3, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Earring", 8, 3, _PanelSelectedHeroBottom_RectTransform, "2"));

        _initialized = true;

        GameMessage.ShowLocale(L.Info.LoadingCollection, false);

        await LoadCollectionAsync();
    }

    private void Update()
    {
        if (!_initialized)
        {
            return;
        }

        bool resize = false;
        if (_ScrollbarVertical_Active != _ScrollViewCollection_GameObject.activeInHierarchy)
        {
            _ScrollbarVertical_Active = _ScrollViewCollection_GameObject.activeInHierarchy;
            resize = true;
        }
        if (!resize && (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width)))
        {
            resize = true;
        }

        if (_PanelSelectedHero_isActive != PanelSelectedHero_GameObject.activeInHierarchy)
        {
            _PanelSelectedHero_isActive = PanelSelectedHero_GameObject.activeInHierarchy;
            resize = true;
        }


        if (resize)
        {
            OnResizeWindow();
        }
    }


    private async Task LoadCollectionAsync()
    {
        try
        {
            if (_GroupDividers.Count > 0)
            {
                foreach (GroupDivider item in _GroupDividers)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }

            OnResizeWindow();
            await Task.Yield();


            _GroupDividers.Clear();
            //List<Task> tasks = new();
            int completed = 0;
            if (CollectionMode == 1)
            {
                foreach (GroupCollectionElement item in G.Game.Collection.GetCollectionHeroesGroupedByGroupNames().OrderByDescending(static a => a.Priority))
                {
                    if (item.List.Count() > 0)
                    {
                        GameObject groupDividerPrefab = await Addressables.LoadAssetAsync<GameObject>("GroupDividerPrefab").Task;
                        GameObject obj = groupDividerPrefab.SafeInstant();
                        GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                        obj.transform.SetParent(_CollectionContent_Transform, false);
                        _GroupDividers.Add(groupDivider);
                        groupDivider.Init1(item.Name, this, obj, item.List, _SelectedHeroTop_TextMeshProUGUI, _SelectedHero_Image, _SelectedHeroRarity_Image);
                        OnResizeWindow();
                        await groupDivider.Init2();
                        completed++;
                        if (completed % 100 == 0)
                        {
                            Debug.Log(completed);
                        }
                    }
                }
            }
            else if (CollectionMode == 2)
            {

            }

            OnResizeWindow();
        }
        finally
        {
            GameMessage.Close();
        }
    }

    /// <summary>
    /// Кнопка "Герои"
    /// </summary>
    private void OnClickHeroes()
    {
        if (CollectionMode == 1)
        {
            return;
        }
        CollectionMode = 1;
        OnClickTabButton(_TabButtonHeroes);
        //_TabButtonHeroes.image.color = Color.white;
        _ = LoadCollectionAsync();
    }

    /// <summary>
    /// Кнопка "Экипировка"
    /// </summary>
    private void OnClickEquipment()
    {
        if (CollectionMode == 2)
        {
            return;
        }
        CollectionMode = 2;
        OnClickTabButton(_TabButtonEquipment);
        _ = LoadCollectionAsync();
    }

    private void OnClickTabButton(TabButton tabButtonPressed)
    {
        if (tabButtonPressed.name == _TabButtonHeroes.name)
        {
            _TabButtonHeroes.image.color = Color.white;
            _TabButtonEquipment.image.color = ColorOffButton;
        }
        else
        {
            _TabButtonHeroes.image.color = ColorOffButton;
            _TabButtonEquipment.image.color = Color.white;
        }
    }

    private void OnResizeWindow()
    {
        _height = Screen.height;
        _width = Screen.width;
        float coefHeight = _height / 1080f;
        float fontSize = 25f * coefHeight;

        //buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
        //buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;

        // Изображение заднего фона
        float coefScreen = _width / _height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
        _Background_Image.rectTransform.sizeDelta = coefScreen > _ImageBackgroundCoef ? new Vector2(_width, _width / _ImageBackgroundCoef) : new Vector2(_height * _ImageBackgroundCoef, _height);


        // Верхняя панель
        float topPanelHeightPercent = 0.08f;
        float panelTopHeight = topPanelHeightPercent * _height; // 86.4
        Vector2 vector008PercentOfHeight = new(panelTopHeight, panelTopHeight);
        _PanelTop_RectTransform.sizeDelta = new Vector2(_width, panelTopHeight);


        // Кнопки вкладок
        float tabButtonWidth = panelTopHeight * 3.777777f; // = (0,17 * 1920) / (0,08 * 1080)
        _TabButtonEquipment.rectTransform.sizeDelta = _TabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, panelTopHeight);
        _TabButtonEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
        _TabButtonHeroes.textMeshProUGUI.fontSize = fontSize;
        _TabButtonEquipment.textMeshProUGUI.fontSize = fontSize;


        // Кнопки "Закрыть"
        _ButtonClose_RectTransform.sizeDelta = vector008PercentOfHeight;
        _ButtonCloseSelectedHero_RectTransform.sizeDelta = vector008PercentOfHeight;


        // Панель выбранного героя
        bool panelSelectedHeroActive = _PanelSelectedHero_RectTransform.gameObject.activeInHierarchy;
        float panelSelectedHeroWidth;
        if (panelSelectedHeroActive)
        {
            float panelSelectedHeroWidthBase = 700.9088f; // при разрешении 1920x1080
            panelSelectedHeroWidth = panelSelectedHeroWidthBase * coefHeight;

            // Панель выбранного героя
            _PanelSelectedHero_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 994 * coefHeight);

            // Панель выбранного героя. Верхняя панель где написано имя героя
            _PanelSelectedHeroTop_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, panelTopHeight);

            // Панель выбранного героя. Нижняя панель с характеристиками героя
            _PanelSelectedHeroBottom_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 908 * coefHeight);

            // Кнопки вкладок
            _PanelSelectedHeroBottomTabButton1_RectTransform.sizeDelta = new Vector2(150 * coefHeight, 50 * coefHeight);
            _PanelSelectedHeroBottomTabButton2_RectTransform.sizeDelta = _PanelSelectedHeroBottomTabButton1_RectTransform.sizeDelta;
            _PanelSelectedHeroBottomTabButton1_RectTransform.anchoredPosition = new Vector2(5f * coefHeight, -5f * coefHeight);
            _PanelSelectedHeroBottomTabButton2_RectTransform.anchoredPosition = new Vector2(160f * coefHeight, -5f * coefHeight);
            _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI.fontSize = 15f * coefHeight;
            _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI.fontSize = 15f * coefHeight;

            // Вкладка 1. Экипировка
            _PanelSelectedHeroBottomTab1_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 848 * coefHeight); ;

            // Выбранный герой. Лабел
            _SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
            //SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
            _SelectedHeroTop_TextMeshProUGUI.fontSize = 40f * coefHeight;

            _SelectedHeroImageContainer_RectTransform.sizeDelta = new Vector2(282.8571f * coefHeight, 495 * coefHeight);
            _SelectedHeroImageContainer_RectTransform.anchoredPosition = new Vector2(-10f * coefHeight, 10f * coefHeight);

            _Slots.ForEach(a => a.Resize(coefHeight));
        }
        else
        {
            panelSelectedHeroWidth = 0;
        }

        // Панель коллекции
        float panelCollection_Width = _width - panelSelectedHeroWidth;
        PanelCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, _height - panelTopHeight);

        // Панель верхних кнопок
        _PanelCollectionTopButtons_RectTransform.sizeDelta = new Vector2(panelCollection_Width, 113f * coefHeight);

        // Внутренние кнопки
        _InternalPanelHeroes.Refresh(coefHeight, vector008PercentOfHeight, 10);
        _InternalPanelItems.Refresh(coefHeight, vector008PercentOfHeight, 10);
        _InternalPanelFilter.Refresh(coefHeight, vector008PercentOfHeight, 150);
        _InternalPanelGroup.Refresh(coefHeight, vector008PercentOfHeight, 256);
        _InternalPanelSort.Refresh(coefHeight, vector008PercentOfHeight, 362);

        // Scroll View для коллекции героев
        _ScrollViewCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, PanelCollection_RectTransform.sizeDelta.y - _PanelCollectionTopButtons_RectTransform.sizeDelta.y);

        // ScrollbarVertical для коллекции героев
        _ScrollbarVertical_RectTransform.sizeDelta = new Vector2(32f * coefHeight, _ScrollViewCollection_RectTransform.sizeDelta.y);

        _CollectionContent_Transform.GetComponent<VerticalLayoutGroup>().spacing = 5f * coefHeight;

        // groupDividers
        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.Resize());
        }

    }
}
