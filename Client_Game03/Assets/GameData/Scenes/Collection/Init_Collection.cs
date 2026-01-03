using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using Game03Client.PlayerCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

public class Init_Collection : MonoBehaviour
{
    public static readonly Vector3 Vector3Selected = new(1.15f, 1.15f, 1);

    //public bool Initialized { get; private set; }
    private const int COLOR_OFF_BUTTON_RGB_VALUE = 100;
    private static Color ColorOffButton = new(COLOR_OFF_BUTTON_RGB_VALUE / 255f, COLOR_OFF_BUTTON_RGB_VALUE / 255f, COLOR_OFF_BUTTON_RGB_VALUE / 255f);
    private bool _initialized = false;
    public bool ScrollbarVertical_Active { get; private set; } = false;
    private float _width, _height;

    private class TabButton
    {
        public readonly string name;
        public readonly RectTransform rectTransform;
        public readonly Button button;
        public readonly TextMeshProUGUI textMeshProUGUI;
        public readonly Image image;

        public TabButton(string name, string nameText, Func<UniTask> asyncAction)
        {
            this.name = name;
            button = GameObjectFinder.FindByName<Button>(name);
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            image = GameObjectFinder.FindByName<Image>(name);
            textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(nameText);
            button.onClick.AddListener(() => asyncAction().Forget());
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
    private RectTransform _ButtonCloseSelectedEquipment_RectTransform;
    private RectTransform _PanelSelectedHero_RectTransform;
    internal GameObject PanelSelectedHero_GameObject { get; private set; }
    private bool _PanelSelectedHero_isActive = false;
    private bool _PanelSelectedEquipment_isActive = false;
    private RectTransform _PanelSelectedHeroTop_RectTransform;
    private RectTransform _PanelSelectedHeroBottom_RectTransform;
    private RectTransform _PanelSelectedHeroBottomTabButton1_RectTransform;
    private RectTransform _PanelSelectedHeroBottomTabButton2_RectTransform;
    private TextMeshProUGUI _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI;
    private TextMeshProUGUI _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI;
    private RectTransform _PanelSelectedHeroBottomTab1_RectTransform;

    internal GameObject PanelSelectedEquipment_GameObject { get; private set; }
    private RectTransform _PanelSelectedEquipment_RectTransform;
    private RectTransform _PanelSelectedEquipmentTop_RectTransform;
    private RectTransform _PanelSelectedEquipmentBottom_RectTransform;
    private RectTransform _PanelSelectedEquipmentBottomTabButton1_RectTransform;
    private RectTransform _PanelSelectedEquipmentBottomTabButton2_RectTransform;
    private TextMeshProUGUI _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI;
    private TextMeshProUGUI _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI;
    private RectTransform _PanelSelectedEquipmentBottomTab1_RectTransform;

    internal RectTransform PanelCollection_RectTransform { get; private set; }
    private RectTransform _PanelCollectionTopButtons_RectTransform;
    private RectTransform _ScrollViewCollection_RectTransform;
    public RectTransform ScrollbarVertical_RectTransform { get; private set; }
    private GameObject ScrollbarVertical_GameObject;

    private GameObject _ScrollViewCollection_GameObject;
    public TextMeshProUGUI SelectedHeroTop_TextMeshProUGUI { get; private set; }
    public TextMeshProUGUI SelectedEquipmentTop_TextMeshProUGUI { get; private set; }
    private RectTransform _SelectedHeroImageContainer_RectTransform;
    private RectTransform _SelectedEquipmentImageContainer_RectTransform;
    public Image SelectedHero_Image { get; private set; }
    public Image SelectedEquipment_Image { get; private set; }
    public Image SelectedHeroRarity_Image { get; private set; }
    public Image SelectedEquipmentRarity_Image { get; private set; }

    /// <summary>
    /// Внутренняя панель, кнопки.
    /// </summary>
    private class InternalPanelButton
    {
        private readonly RectTransform rectTransform;
        private readonly RectTransform rectTransformButton;
        private readonly RectTransform rectTransformLabel;
        private readonly TextMeshProUGUI textMeshProUGUILabel;
        private readonly GameObject gameObject;
        public InternalPanelButton(string name)
        {
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            gameObject = rectTransform.gameObject;
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
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }

    private InternalPanelButton _InternalPanelHeroes;
    private InternalPanelButton _InternalPanelEquipments;
    private InternalPanelButton _InternalPanelFilter;
    private InternalPanelButton _InternalPanelGroup;
    private InternalPanelButton _InternalPanelSort;

    private Transform _CollectionContent_Transform;

    private readonly List<GroupDivider> _GroupDividers = new();

    private RectTransform _RangePanel_RectTransform;
    private GameObject _RangePanel_GameObject;
    private RectTransform _ButtonPrevPage_RectTransform;
    private RectTransform _ButtonNextPage_RectTransform;
    private RectTransform _LabelRangePage_RectTransform;
    private TextMeshProUGUI _LabelRangePage_TextMeshProUGUI;



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
            const float _TopLeftBase = 20f;
            const float widthBase = 116f;
            const float heightBase = widthBase / (1f - 0.154307f);
            _RectTransform.sizeDelta = new Vector2(widthBase * coefHeight, heightBase * coefHeight);
            float x = (((widthBase + _TopLeftBase) * (posX - 1)) + _TopLeftBase) * coefHeight;
            float y = (((heightBase + _TopLeftBase) * (posY - 1)) + _TopLeftBase) * coefHeight;
            _RectTransform.anchoredPosition = new Vector2(x, -y);

            _TextMeshProUGUI.fontSize = 15f * coefHeight;
        }
    }
    private readonly List<Slot> _Slots = new();

    /// <summary>
    /// 1 - герои, 2 - экипировка
    /// </summary>
    public int CollectionMode { get; private set; } = 1;

    private async void Start()
    {
        _TabButtonHeroes = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", OnClickHeroes);
        _TabButtonEquipment = new("ButtonItems (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", OnClickEquipment);
        _TabButtonHeroes.SetText($"{G.Game.LocalizationManager.GetValue(L.UI.Button.Heroes)} ({G.Game.Collection.GetCountHeroes()})");
        _TabButtonEquipment.SetText($"{G.Game.LocalizationManager.GetValue(L.UI.Button.Equipment)} ({G.Game.Collection.GetCountEquipments()})");

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
        PanelSelectedHero_GameObject.SetActive(false);
        _PanelSelectedHero_RectTransform.anchoredPosition = Vector2.zero;
        _PanelSelectedHero_isActive = PanelSelectedHero_GameObject.activeInHierarchy;

        _PanelSelectedEquipment_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipment (id=ta39338e)");
        PanelSelectedEquipment_GameObject = _PanelSelectedEquipment_RectTransform.gameObject;
        PanelSelectedEquipment_GameObject.SetActive(false);
        _PanelSelectedEquipment_RectTransform.anchoredPosition = Vector2.zero;
        _PanelSelectedEquipment_isActive = PanelSelectedEquipment_GameObject.activeInHierarchy;

        SelectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull (id=m5kn2f6p)");
        SelectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=xami3s9q)");

        SelectedEquipment_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull (id=gu7wtz83)");
        SelectedEquipmentRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=qje8dq78)");

        _SelectedHeroImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=1l6gscif)");
        _SelectedEquipmentImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=bqxjhczr)");


        // Панель для внутренних кнопок
        PanelCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollection (id=jcxwa01g)");
        _PanelCollectionTopButtons_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionTopButtons (id=gmzb0h9f)");

        // Внутренние кнопки
        _InternalPanelHeroes = new("ImageButtonHeroes (id=pakco5ud)");
        _InternalPanelEquipments = new("ImageButtonEquipments (id=vuhjngaz)");
        _InternalPanelFilter = new("ImageButtonFilter (id=vjeqfzen)");
        _InternalPanelGroup = new("ImageButtonGroup (id=hbsaogwl)");
        _InternalPanelSort = new("ImageButtonSort (id=6nvcsrdm)");


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


        _ButtonCloseSelectedEquipment_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=va8d3lsz)");
        _ButtonCloseSelectedEquipment_RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            PanelSelectedEquipment_GameObject.SetActive(false);
            foreach (GroupDivider a in _GroupDividers)
            {
                bool founded = false;
                foreach (GroupDivider.DataCollectionElement b in a.ListDataCollectionElement)
                {
                    if (b.Selected)
                    {
                        b.rectTransform.localScale = Vector3.one;
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

        _PanelSelectedEquipmentTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentTop (id=dp54agcp)");
        _PanelSelectedEquipmentBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottom (id=bj3zvapm)");

        _PanelSelectedHeroBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=uiufd2wv)");
        _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=lf8q2aas)");
        _PanelSelectedHeroBottomTabButton1_TextMeshProUGUI.SetText(G.Game.LocalizationManager.GetValue(L.UI.Button.Equipment));

        _PanelSelectedEquipmentBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=n94o21t8)");
        _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=yjb1gqbc)");
        _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI.SetText(G.Game.LocalizationManager.GetValue(L.UI.Button.Item));

        _PanelSelectedHeroBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=kzury0kd)");
        _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=6bjw6hi4)");
        _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

        _PanelSelectedEquipmentBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=c1xjs5dr)");
        _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=pn28dhfr)");
        _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

        _PanelSelectedHeroBottomTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedHeroBottomTab1 (id=kn3yl79k)");
        SelectedHeroTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedHero (id=ahrtgg43)");

        _PanelSelectedEquipmentBottomTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottomTab1 (id=9nwzj7p8)");
        SelectedEquipmentTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedEquipment (id=004gk90y)");

        // Scroll View для коллекции героев
        _ScrollViewCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
        ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
        ScrollbarVertical_GameObject = ScrollbarVertical_RectTransform.gameObject;

        _ScrollViewCollection_GameObject = _ScrollViewCollection_RectTransform.gameObject;

        // Коллекция контент
        _CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;

        _Slots.Clear();
        _Slots.Add(new Slot("Head", 1, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Armor", 2, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Hands", 3, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Feet", 4, 1, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Waist", 5, 1, _PanelSelectedHeroBottom_RectTransform));

        _Slots.Add(new Slot("Ring", 1, 2, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Ring", 2, 2, _PanelSelectedHeroBottom_RectTransform, "2"));
        _Slots.Add(new Slot("Neck", 3, 2, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("Trinket", 4, 2, _PanelSelectedHeroBottom_RectTransform, "1"));
        _Slots.Add(new Slot("Trinket", 5, 2, _PanelSelectedHeroBottom_RectTransform, "2"));

        _Slots.Add(new Slot("Weapon", 1, 3, _PanelSelectedHeroBottom_RectTransform));
        _Slots.Add(new Slot("WeaponShield", 2, 3, _PanelSelectedHeroBottom_RectTransform));


        // Панель навигации по страницам
        _RangePanel_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelRange (id=66z5bnzi)");
        _RangePanel_GameObject = _RangePanel_RectTransform.gameObject;
        _ButtonPrevPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonPrevPage (id=25alql62)");
        _ButtonNextPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonNextPage (id=k5moi57b)");
        _LabelRangePage_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelRangePage (id=6jgz12bu)");
        _LabelRangePage_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelRangePage (id=6jgz12bu)");
        _ButtonPrevPage_RectTransform.gameObject.AddClickEvent(PagePrev, true);
        _ButtonNextPage_RectTransform.gameObject.AddClickEvent(PageNext, true);
        UpdatePageMax();


        _initialized = true;

        GameMessage.ShowLocale(L.Info.LoadingCollection, false);

        await InstantiateCollectionAsync();
    }

    private void Update()
    {
        if (!_initialized)
        {
            return;
        }

        bool resize = false;
        if (ScrollbarVertical_Active != ScrollbarVertical_GameObject.activeInHierarchy)
        {
            ScrollbarVertical_Active = ScrollbarVertical_GameObject.activeInHierarchy;

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

        if (_PanelSelectedEquipment_isActive != PanelSelectedEquipment_GameObject.activeInHierarchy)
        {
            _PanelSelectedEquipment_isActive = PanelSelectedEquipment_GameObject.activeInHierarchy;
            resize = true;
        }


        if (resize)
        {
            OnResizeWindow();
        }
    }


    private int pageCurrent = 1;
    private int pageMax = 1;

    private void UpdatePageMax()
    {
        int c;
        if (CollectionMode == 1)
        {
            c = G.Game.Collection.GetCountHeroes();
        }
        else if (CollectionMode == 2)
        {
            c = G.Game.Collection.GetCountEquipments();
        }
        else
        {
            throw new Exception();
        }
        pageMax = (c / PlayerCollectionProvider.PAGE_SIZE) + (c % PlayerCollectionProvider.PAGE_SIZE > 0 ? 1 : 0);
        if (pageMax < 1)
        {
            pageMax = 1;
        }
        if (pageCurrent > pageMax)
        {
            pageCurrent = pageMax;
        }
        _RangePanel_GameObject.SetActive(pageMax > 1);
    }

    private async UniTask PagePrev()
    {
        if (pageCurrent > 1)
        {
            pageCurrent--;
            await InstantiateCollectionAsync();
        }
    }
    private async UniTask PageNext()
    {
        if (pageCurrent < pageMax)
        {
            pageCurrent++;
            await InstantiateCollectionAsync();
        }
    }

    private async UniTask InstantiateCollectionAsync()
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
            await UniTask.Yield();


            int max = PlayerCollectionProvider.PAGE_SIZE * pageCurrent;

            _GroupDividers.Clear();
            if (CollectionMode == 1)
            {
                if (pageCurrent == pageMax)
                {
                    max = G.Game.Collection.GetCountHeroes();
                }
                PanelSelectedEquipment_GameObject.SetActive(false);

                IEnumerable<GroupCollectionElement> grouped = G.Game.Collection.GetCollectionHeroesGroupedByGroupNames(pageCurrent);
                IOrderedEnumerable<GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
                foreach (GroupCollectionElement item in sorted)
                {
                    if (item.List.Count() > 0)
                    {
                        GameObject obj = AddressableCache.GroupDividerPrefabAddressableGameObject.SafeInstant();
                        GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                        obj.transform.SetParent(_CollectionContent_Transform, false);
                        _GroupDividers.Add(groupDivider);
                        await groupDivider.Init(item.Name, this, obj, item.List);
                    }
                }
            }
            else if (CollectionMode == 2)
            {
                if (pageCurrent == pageMax)
                {
                    max = G.Game.Collection.GetCountEquipments();
                }
                PanelSelectedHero_GameObject.SetActive(false);

                IEnumerable<GroupCollectionElement> grouped = G.Game.Collection.GetCollectionEquipmentesGroupByGroups(pageCurrent);
                IOrderedEnumerable<GroupCollectionElement> sorted = grouped.OrderByDescending(static a => a.Priority);
                foreach (GroupCollectionElement item in sorted)
                {
                    if (item.List.Count() > 0)
                    {
                        GameObject groupDividerPrefab = AddressableCache.GroupDividerPrefabAddressableGameObject;
                        GameObject obj = groupDividerPrefab.SafeInstant();
                        GroupDivider groupDivider = obj.AddComponent<GroupDivider>();
                        obj.transform.SetParent(_CollectionContent_Transform, false);
                        _GroupDividers.Add(groupDivider);
                        await groupDivider.Init(item.Name, this, obj, item.List);
                    }
                }
            }
            else
            {
                throw new Exception();
            }


            _LabelRangePage_TextMeshProUGUI.text = $"{((pageCurrent - 1) * PlayerCollectionProvider.PAGE_SIZE) + 1} - {max}";

            OnResizeWindow();
        }
        finally
        {
            GameMessage.Close();
        }
    }

    public void UnselectAll()
    {
        for (int i = 0; i < _GroupDividers.Count; i++)
        {
            GroupDivider g = _GroupDividers[i];
            for (int j = 0; j < g.ListDataCollectionElement.Count; j++)
            {
                GroupDivider.DataCollectionElement el = g.ListDataCollectionElement[j];
                el.Selected = false;
                el.rectTransform.localScale = Vector3.one;
            }

        }
    }

    /// <summary> Кнопка "Герои". </summary>
    private async UniTask OnClickHeroes()
    {
        if (CollectionMode == 1)
        {
            return;
        }
        CollectionMode = 1;
        _InternalPanelHeroes.SetActive(true);
        _InternalPanelEquipments.SetActive(false);
        OnClickTabButton(_TabButtonHeroes);
        UpdatePageMax();
        await InstantiateCollectionAsync();
    }

    /// <summary> Кнопка "Экипировка". </summary>
    private async UniTask OnClickEquipment()
    {
        if (CollectionMode == 2)
        {
            return;
        }
        CollectionMode = 2;
        _InternalPanelHeroes.SetActive(false);
        _InternalPanelEquipments.SetActive(true);
        OnClickTabButton(_TabButtonEquipment);
        UpdatePageMax();
        await InstantiateCollectionAsync();
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

    public void OnResizeWindow()
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
        float tabButtonWidth = 230.4f * coefHeight;
        _TabButtonEquipment.rectTransform.sizeDelta = _TabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, panelTopHeight);
        _TabButtonEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
        _TabButtonHeroes.textMeshProUGUI.fontSize = 22f * coefHeight;
        _TabButtonEquipment.textMeshProUGUI.fontSize = 22f * coefHeight;


        // Кнопки "Закрыть"
        _ButtonClose_RectTransform.sizeDelta = vector008PercentOfHeight;


        // Панель выбранного героя
        bool panelSelectedHeroActive = _PanelSelectedHero_RectTransform.gameObject.activeInHierarchy;
        float panelSelectedHeroWidth = 0;
        if (panelSelectedHeroActive)
        {
            _ButtonCloseSelectedHero_RectTransform.sizeDelta = vector008PercentOfHeight;

            float panelSelectedHeroWidthBase = 700f; // при разрешении 1920x1080
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
            _PanelSelectedHeroBottomTab1_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 848 * coefHeight);

            // Выбранный герой. Лабел
            SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
            //SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
            SelectedHeroTop_TextMeshProUGUI.fontSize = 40f * coefHeight;

            _SelectedHeroImageContainer_RectTransform.sizeDelta = new Vector2(282.8571f * coefHeight, 495 * coefHeight);
            _SelectedHeroImageContainer_RectTransform.anchoredPosition = new Vector2(-20f * coefHeight, 20f * coefHeight);

            _Slots.ForEach(a => a.Resize(coefHeight));
        }

        bool panelSelectedEquipmentActive = _PanelSelectedEquipment_RectTransform.gameObject.activeInHierarchy;
        float panelSelectedEquipmentWidth = 0;
        if (panelSelectedEquipmentActive)
        {
            _ButtonCloseSelectedEquipment_RectTransform.sizeDelta = vector008PercentOfHeight;

            float panelSelectedEquipmentWidthBase = 700f; // при разрешении 1920x1080
            panelSelectedEquipmentWidth = panelSelectedEquipmentWidthBase * coefHeight;
            // Панель выбранной экипировки
            _PanelSelectedEquipment_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 994 * coefHeight);

            // Панель выбранного героя. Верхняя панель где написано название экипировки
            _PanelSelectedEquipmentTop_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, panelTopHeight);

            // Панель выбранного героя. Нижняя панель с характеристиками героя
            _PanelSelectedEquipmentBottom_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 908 * coefHeight);

            // Кнопки вкладок
            _PanelSelectedEquipmentBottomTabButton1_RectTransform.sizeDelta = new Vector2(150 * coefHeight, 50 * coefHeight);
            _PanelSelectedEquipmentBottomTabButton2_RectTransform.sizeDelta = _PanelSelectedEquipmentBottomTabButton1_RectTransform.sizeDelta;
            _PanelSelectedEquipmentBottomTabButton1_RectTransform.anchoredPosition = new Vector2(5f * coefHeight, -5f * coefHeight);
            _PanelSelectedEquipmentBottomTabButton2_RectTransform.anchoredPosition = new Vector2(160f * coefHeight, -5f * coefHeight);
            _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI.fontSize = 15f * coefHeight;
            _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI.fontSize = 15f * coefHeight;

            // Вкладка 1. Экипировка
            _PanelSelectedEquipmentBottomTab1_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 848 * coefHeight);

            // Выбранный герой. Лабел
            SelectedEquipmentTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth - panelTopHeight, panelTopHeight);
            //SelectedEquipmentTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
            SelectedEquipmentTop_TextMeshProUGUI.fontSize = 40f * coefHeight;

            _SelectedEquipmentImageContainer_RectTransform.sizeDelta = new Vector2(345f * coefHeight, 345 * coefHeight);
            _SelectedEquipmentImageContainer_RectTransform.anchoredPosition = new Vector2(-10f * coefHeight, 10f * coefHeight);
        }


        // Панель коллекции
        float panelCollection_Width = _width - Math.Max(panelSelectedHeroWidth, panelSelectedEquipmentWidth);
        PanelCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, _height - panelTopHeight);

        // Панель верхних кнопок
        _PanelCollectionTopButtons_RectTransform.sizeDelta = new Vector2(panelCollection_Width, 113f * coefHeight);

        // Внутренние кнопки
        _InternalPanelHeroes.Refresh(coefHeight, vector008PercentOfHeight, 10);
        _InternalPanelEquipments.Refresh(coefHeight, vector008PercentOfHeight, 10);
        _InternalPanelFilter.Refresh(coefHeight, vector008PercentOfHeight, 150);
        _InternalPanelGroup.Refresh(coefHeight, vector008PercentOfHeight, 256);
        _InternalPanelSort.Refresh(coefHeight, vector008PercentOfHeight, 362);

        // Scroll View для коллекции героев
        _ScrollViewCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, PanelCollection_RectTransform.sizeDelta.y - _PanelCollectionTopButtons_RectTransform.sizeDelta.y);

        // ScrollbarVertical для коллекции героев
        ScrollbarVertical_RectTransform.sizeDelta = new Vector2(32f * coefHeight, _ScrollViewCollection_RectTransform.sizeDelta.y);

        _CollectionContent_Transform.GetComponent<VerticalLayoutGroup>().spacing = 5f * coefHeight;


        // groupDividers
        if (_GroupDividers.Count > 0)
        {
            _GroupDividers.ForEach(a => a.Resize());
        }

        // Панель навигации по страницам
        _RangePanel_RectTransform.sizeDelta = new Vector2(230f * coefHeight, 90f * coefHeight);
        _RangePanel_RectTransform.anchoredPosition = new Vector2(468f * coefHeight, -10f * coefHeight);
        _ButtonNextPage_RectTransform.sizeDelta = _ButtonPrevPage_RectTransform.sizeDelta = new Vector2(100f * coefHeight, 60f * coefHeight);
        _LabelRangePage_RectTransform.sizeDelta = new Vector2(230f * coefHeight, 30f * coefHeight);
        _LabelRangePage_TextMeshProUGUI.fontSize = 18f * coefHeight;
    }
}
