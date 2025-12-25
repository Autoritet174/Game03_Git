using Assets.GameData.Scenes.AllHeroes;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using G = Assets.GameData.Scripts.G;

public class AllHeroes : MonoBehaviour
{
    private ScrollRect scrollView;
    private RectTransform content;
    private RectTransform buttonClose;

    /// <summary>
    /// Компонент ScrollRect, к которому привязан скрипт.
    /// </summary>
    private ScrollRect scrollRect;

    /// <summary>
    /// Компонент GridLayoutGroup в Content.
    /// </summary>
    private GridLayoutGroup gridLayout;

    /// <summary>
    /// Компонент RectTransform у Scroll View.
    /// </summary>
    private RectTransform scrollRectTransform;

    /// <summary>
    /// Компонент RectTransform у вертикальной полосы прокрутки. Для изменения размера шрифта при изменении размера окна.
    /// </summary>
    private RectTransform verticalScrollbar;
    private readonly ConcurrentBag<TextMeshProUGUI> list_TextMeshProUGUI_heroNames = new();
    private readonly Dictionary<string, RectTransform> dictOnResizeButtonClose = new();
    private readonly Dictionary<string, RectTransform> dictOnResizeHeroName = new();
    private readonly Dictionary<string, TextMeshProUGUI> dictOnResizeHeroNameFont = new();

    private float _lastHeight;
    private float _lastWidth;

    [SerializeField]
    private GameObject prefabIconHero;

    //[SerializeField]
    //private GameObject prefabHeroViewer;
    private bool inited = false;

    private int columnCount = 8;


    //private readonly int r = 4;
    private async void Start()
    {
        scrollView = GameObjectFinder.FindByName<ScrollRect>("Scroll View (id=2e9cbb1a)");
        content = GameObjectFinder.FindByName<RectTransform>("Content (id=0a40ce51)", startParent: scrollView.transform);
        buttonClose = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=5cd5cc79)");
        scrollRect = scrollView.GetComponent<ScrollRect>();
        GameObject ScrollbarVertical = GameObjectFinder.FindByName("Scrollbar Vertical (id=75511cdc)");
        verticalScrollbar = ScrollbarVertical.GetComponent<RectTransform>();
        scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        gridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();

        //ButtonCloseHelper.UpdateSize(_lastWidth, _lastHeight, buttonClose);
        OnResizeWindow();

        await AddAllImageOnContent();



        inited = true;
    }

    private void Update()
    {
        if (inited && (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth)))
        {
            OnResizeWindow();
        }
    }

    private async UniTask AddAllImageOnContent()
    {
        List<UniTask> list = new();
        foreach (DtoBaseHero heroStats in G.Game.GameData.GetDtoContainer().DtoBaseHeroes.OrderByDescending(a => a.Rarity))
        {
            list.Add(LoadHeroByName(heroStats));
        }
        OnResizeWindow();
        await UniTask.WhenAll(list);
    }

    private async UniTask LoadHeroByName(DtoBaseHero hero)
    {
        GameObject _prefabIconHero = prefabIconHero.SafeInstant();
        string heroName = hero.Name;
        _prefabIconHero.name = heroName;

        Transform transform = _prefabIconHero.transform;
        transform.SetParent(content.transform, false);

        // Текст (может быть установлен сразу)
        Transform childText = _prefabIconHero.transform.Find("TextHero");
        if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
        {
            textMeshPro.text = heroName.ToUpper1Char();
            list_TextMeshProUGUI_heroNames.Add(textMeshPro);
        }

        // Изображение (загружаем через Addressable)
        Transform childImageMaskHero = _prefabIconHero.transform.Find("ImageMaskCollectionElement");
        Transform childImageMaskRarity = _prefabIconHero.transform.Find("ImageMaskRarity");
        Transform childImageHero = childImageMaskHero.Find("ImageCollectionElement");
        Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");

        UnityEngine.UI.Image imageHero = childImageHero.GetComponent<Image>();
        UnityEngine.UI.Image imageRarity = childImageRarity.GetComponent<Image>();

        string addressableKey = $"hero-image-{heroName.ToLower()}_face";

        var heroSprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).ToUniTask();
        var raritySprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{hero.Rarity}").ToUniTask();
        var selectedSprite = await Addressables.LoadAssetAsync<Sprite>($"raritySelected").ToUniTask();

        imageHero.sprite = heroSprite;
        imageHero.preserveAspect = true; // Сохраняет пропорции изображения
        imageHero.type = Image.Type.Simple; // Режим без растягивания;

        imageRarity.sprite = raritySprite;
        imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
        imageRarity.type = Image.Type.Simple; // Режим без растягивания;

        async UniTask OnClick()
        {
            await HeroView(hero);
        }
        _prefabIconHero.AddClickEvent(OnClick);

        async UniTask OnPoinerEnter()
        {
            imageRarity.sprite = selectedSprite;
            await UniTask.Yield();
        }
        async UniTask OnPoinerExit()
        {
            imageRarity.sprite = raritySprite;
            await UniTask.Yield();
        }
        _prefabIconHero.AddHoverEvents(OnPoinerEnter, OnPoinerExit);

        // Добавляем компонент для обработки кликов
        //ImageHeroHandler clickHandler = _prefabIconHero.AddComponent<ImageHeroHandler>();
        //clickHandler.Initialize(hero, raritySprite.Result, selectedSprite.Result, HeroView, imageRarity);



        //Addressables.Release(handle
    }


    private void OnResizeWindow()
    {
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        // Устанавливаем Constraint как FixedcolumnCount
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        float scrollView_Width = scrollRectTransform.rect.width * 0.95f;
        float scrollBar_Width = verticalScrollbar.rect.width;

        float percentWidthForImage = 0.9f;

        //float totalAvailableWidth = scrollViewWidth - scrollbarWidth - (horizontalPadding * 2) - (spacing * (columnCount - 1));
        float totalAvailableWidth = scrollView_Width - scrollBar_Width;
        //Debug.Log("totalAvailableWidth=" + totalAvailableWidth);
        if (totalAvailableWidth < 200f)
        {
            totalAvailableWidth = 200f;
        }

        columnCount = totalAvailableWidth switch
        {
            <= 300f => 3,
            <= 800f => Mathf.RoundToInt(totalAvailableWidth / 100f),
            _ => 8,
        };

        // Устанавливаем количество колонок
        gridLayout.constraintCount = columnCount;
        float cellWidth = totalAvailableWidth / columnCount * percentWidthForImage;
        gridLayout.cellSize = new Vector2(cellWidth, cellWidth);

        // Отступ между элементами в пикселях.
        float spacing = totalAvailableWidth / (columnCount - 1) * (1f - percentWidthForImage);
        gridLayout.spacing = new Vector2(spacing, spacing);

        //scrollRect = GetComponentInParent<ScrollRect>();


        //настройки ScrollSensitivity так, чтобы при единичном повороте колеса мыши прокручивалась одна ячейка.
        scrollRect.scrollSensitivity = cellWidth + spacing;// / 6f / 2f;

        foreach (TextMeshProUGUI textMeshProUGUI in list_TextMeshProUGUI_heroNames)
        {
            textMeshProUGUI.fontSize = cellWidth * 0.16f;
        }

        ButtonCloseHelper.UpdateSize(_lastWidth, _lastHeight, buttonClose);

        OnResizeAllDictotaries();
    }

    private void OnResizeAllDictotaries()
    {

        foreach (KeyValuePair<string, RectTransform> item in dictOnResizeButtonClose)
        {
            ButtonCloseHelper.UpdateSize(_lastWidth, _lastHeight, item.Value);
        }

        foreach (KeyValuePair<string, RectTransform> item in dictOnResizeHeroName)
        {
            item.Value.offsetMin = new Vector2(0, 993 * _lastHeight / 1080);
        }

        foreach (KeyValuePair<string, TextMeshProUGUI> item in dictOnResizeHeroNameFont)
        {
            item.Value.fontSize = 66.66666f * _lastHeight / 1080;
        }

    }

    public async UniTask HeroView(DtoBaseHero hero)
    {

        GameObject prefabHeroViewer;

        string addressableKey = $"HeroViewer";
        AsyncOperationHandle<GameObject> prefabHeroViewer_handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        _ = await prefabHeroViewer_handle.ToUniTask();
        prefabHeroViewer = prefabHeroViewer_handle.Status == AsyncOperationStatus.Succeeded
            ? prefabHeroViewer_handle.Result.SafeInstant()
            : throw new Exception($"{nameof(prefabHeroViewer)} не загружен");
        prefabHeroViewer.name = $"IconHero_{hero.Name}";


        Canvas canvas = GameObjectFinder.FindByName<Canvas>($"Canvas_HeroViewer (id=6fpbu4db)", prefabHeroViewer.transform);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        //Кнопка "Закрыть"
        const string _buttonClose__Name = "ButtonClose (id=1berxtk2)";
        Button _buttonClose = GameObjectFinder.FindByName<Button>(_buttonClose__Name, prefabHeroViewer.transform);
        RectTransform _buttonClose__RT = GameObjectFinder.FindByName<RectTransform>(_buttonClose__Name);
        _ = dictOnResizeButtonClose.TryAdd($"{_buttonClose__Name}{_buttonClose__RT.GetHashCode()}", _buttonClose__RT);


        OnResizeAllDictotaries();


        //Имя героя
        const string _Text_HeroName__Name = "Text_HeroName (id=rw8uftqp)";
        TextMeshProUGUI _Text_HeroName = GameObjectFinder.FindByName<TextMeshProUGUI>(_Text_HeroName__Name, prefabHeroViewer.transform);
        _Text_HeroName.text = hero.Name.ToUpper1Char();
        RectTransform _Text_HeroName__RT = GameObjectFinder.FindByName<RectTransform>(_Text_HeroName__Name);
        _ = dictOnResizeHeroName.TryAdd($"{_Text_HeroName__Name}{_Text_HeroName__RT.GetHashCode()}", _Text_HeroName__RT);
        TextMeshProUGUI textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(_Text_HeroName__Name);
        _ = dictOnResizeHeroNameFont.TryAdd($"{_Text_HeroName__Name}{textMeshProUGUI.GetHashCode()}", textMeshProUGUI);


        //Изображение героя
        const string imageHeroFull__Name = "Image_HeroFull (id=6z1ddxml)";
        Image imageHero = GameObjectFinder.FindByName<Image>(imageHeroFull__Name);
        string imageHeroFull__Image = $"hero-image-{hero.Name.ToLower()}";
        var heroImage = await Addressables.LoadAssetAsync<Sprite>(imageHeroFull__Image).ToUniTask();

        imageHero.sprite = heroImage;
        imageHero.preserveAspect = true; // Сохраняет пропорции изображения
        imageHero.type = Image.Type.Simple; // Режим без растягивания;

        //Привязать метод
        _buttonClose.onClick.AddListener(() =>
        {
            string key = $"{_buttonClose__Name}{_buttonClose__RT.GetHashCode()}";
            if (dictOnResizeButtonClose.TryGetValue(key, out _))
            {
                _ = dictOnResizeButtonClose.Remove(key);
            }

            key = $"{_Text_HeroName__Name}{_Text_HeroName__RT.GetHashCode()}";
            if (dictOnResizeHeroName.TryGetValue(key, out _))
            {
                _ = dictOnResizeHeroName.Remove(key);
            }

            key = $"{_Text_HeroName__Name}{textMeshProUGUI.GetHashCode()}";
            if (dictOnResizeHeroNameFont.TryGetValue(key, out _))
            {
                _ = dictOnResizeHeroNameFont.Remove(key);
            }

            Destroy(prefabHeroViewer);
        });


        //Анимация
        await AllHeroesConsts.RunAnimationImage(imageHero, 500);
    }

}
