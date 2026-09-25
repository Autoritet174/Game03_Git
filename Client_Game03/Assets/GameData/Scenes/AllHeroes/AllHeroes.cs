using Assets.GameData.Scenes.AllHeroes;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

/// <summary>Загружает каталог героев и отображает подробности выбранного героя.</summary>
public class AllHeroes : MonoBehaviour
{
    private ScrollRect scrollView;
    private RectTransform content;
    private RectTransform buttonClose;

    /// <summary>Компонент ScrollRect, к которому привязан скрипт.</summary>
    private ScrollRect scrollRect;

    /// <summary>Компонент GridLayoutGroup в Content.</summary>
    private GridLayoutGroup gridLayout;

    /// <summary>Компонент RectTransform у Scroll View.</summary>
    private RectTransform scrollRectTransform;

    /// <summary>Компонент RectTransform у вертикальной полосы прокрутки. Для изменения размера шрифта при изменении размера окна.</summary>
    private RectTransform verticalScrollbar;
    private readonly ConcurrentBag<TextMeshProUGUI> list_TextMeshProUGUI_heroNames = new();
    private readonly Dictionary<string, RectTransform> dictOnResizeButtonClose = new();
    private readonly Dictionary<string, RectTransform> dictOnResizeHeroName = new();
    private readonly Dictionary<string, TextMeshProUGUI> dictOnResizeHeroNameFont = new();

    private float lastHeight;
    private float lastWidth;

    [SerializeField]
    private GameObject prefabIconHero;

    //[SerializeField]
    //private GameObject prefabHeroViewer;
    private bool inited = false;

    private int columnCount = 8;

    //private readonly int r = 4;
    private void Start()
    {
        scrollView = GameObjectFinder.FindByName<ScrollRect>("Scroll View (id=2e9cbb1a)");
        content = GameObjectFinder.FindByName<RectTransform>("Content (id=0a40ce51)", startParent: scrollView.transform);
        buttonClose = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=5cd5cc79)");
        scrollRect = scrollView.GetComponent<ScrollRect>();
        GameObject scrollbarVertical = GameObjectFinder.FindByName("Scrollbar Vertical (id=75511cdc)");
        verticalScrollbar = scrollbarVertical.GetComponent<RectTransform>();
        scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        gridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();

        //ButtonCloseHelper.UpdateSize(_lastWidth, _lastHeight, buttonClose);
        OnResizeWindow();

        this.RunAsync(StartAsync);
    }

    private async UniTask StartAsync(CancellationToken cancellationToken)
    {
        await AddAllImageOnContent();
        inited = true;
    }

    private void Update()
    {
        if (inited && (!Mathf.Approximately(Screen.height, lastHeight) || !Mathf.Approximately(Screen.width, lastWidth)))
        {
            OnResizeWindow();
        }
    }

    private async UniTask AddAllImageOnContent()
    {
        List<UniTask> list = new();
        foreach (BaseHero heroStats in Game03Client.GameData.Container.baseHeroes.OrderByDescending(static a => a.rarity))
        {
            list.Add(LoadHeroByName(heroStats));
        }
        OnResizeWindow();
        await UniTask.WhenAll(list);
    }

    private async UniTask LoadHeroByName(BaseHero hero)
    {
        GameObject prefabIconHeroValue = prefabIconHero.SafeInstant();
        prefabIconHeroValue.name = hero.name;

        Transform transform = prefabIconHeroValue.transform;
        transform.SetParent(content.transform, false);

        // Текст (может быть установлен сразу)
        Transform childText = prefabIconHeroValue.transform.Find("TextCollectionElement");
        if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
        {
            textMeshPro.text = hero.name.ToUpper1Char();
            list_TextMeshProUGUI_heroNames.Add(textMeshPro);
        }
        else
        {
            throw new Exception("TextCollectionElement not found");
        }

        // Изображение (загружаем через Addressable)
        Transform childImageMaskHero = prefabIconHeroValue.transform.Find("ImageMaskCollectionElement");
        Transform childImageMaskRarity = prefabIconHeroValue.transform.Find("ImageMaskRarity");
        Transform childImageHero = childImageMaskHero.Find("ImageCollectionElement");
        Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");

        UnityEngine.UI.Image imageHero = childImageHero.GetComponent<Image>();
        UnityEngine.UI.Image imageRarity = childImageRarity.GetComponent<Image>();

        //string addressableKey = $"hero-image-{heroName.ToLower()}_face";

        //var heroSprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).ToUniTask();
        //var raritySprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{hero.Rarity}").ToUniTask();
        //var selectedSprite = await Addressables.LoadAssetAsync<Sprite>($"raritySelected").ToUniTask();

        imageHero.sprite = AddressablePrefabProvider.heroes[hero.name + "_face"];
        imageHero.preserveAspect = true; // Сохраняет пропорции изображения
        imageHero.type = Image.Type.Simple; // Режим без растягивания;

        imageRarity.sprite = AddressablePrefabProvider.GetRarity(hero.rarity);
        imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
        imageRarity.type = Image.Type.Simple; // Режим без растягивания;

        async UniTask OnClick()
        {
            await HeroView(hero);
        }
        prefabIconHeroValue.SetClickOnGameObject(OnClick);

        async UniTask OnPoinerEnter()
        {
            imageRarity.sprite = AddressablePrefabProvider.raritySelected;
            await UniTask.Yield();
        }
        async UniTask OnPoinerExit()
        {
            imageRarity.sprite = AddressablePrefabProvider.GetRarity(hero.rarity);
            await UniTask.Yield();
        }
        prefabIconHeroValue.SetHoverEvents(OnPoinerEnter, OnPoinerExit);

        // Добавляем компонент для обработки кликов
        //ImageHeroHandler clickHandler = _prefabIconHero.AddComponent<ImageHeroHandler>();
        //clickHandler.Initialize(hero, raritySprite.Result, selectedSprite.Result, HeroView, imageRarity);

        //Addressables.Release(handle
    }

    private void OnResizeWindow()
    {
        lastHeight = Screen.height;
        lastWidth = Screen.width;

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
        gridLayout.cellSize = new(cellWidth, cellWidth);

        // Отступ между элементами в пикселях.
        float spacing = totalAvailableWidth / (columnCount - 1) * (1f - percentWidthForImage);
        gridLayout.spacing = new(spacing, spacing);

        //scrollRect = GetComponentInParent<ScrollRect>();

        //настройки ScrollSensitivity так, чтобы при единичном повороте колеса мыши прокручивалась одна ячейка.
        scrollRect.scrollSensitivity = cellWidth + spacing;// / 6f / 2f;

        foreach (TextMeshProUGUI textMeshProUGUI in list_TextMeshProUGUI_heroNames)
        {
            textMeshProUGUI.fontSize = cellWidth * 0.16f;
        }

        ButtonCloseHelper.UpdateSize(buttonClose);

        OnResizeAllDictotaries();
    }

    private void OnResizeAllDictotaries()
    {

        foreach (KeyValuePair<string, RectTransform> item in dictOnResizeButtonClose)
        {
            ButtonCloseHelper.UpdateSize(item.Value);
        }

        foreach (KeyValuePair<string, RectTransform> item in dictOnResizeHeroName)
        {
            item.Value.offsetMin = new(0, 993 * lastHeight / 1080);
        }

        foreach (KeyValuePair<string, TextMeshProUGUI> item in dictOnResizeHeroNameFont)
        {
            item.Value.fontSize = 66.66666f * lastHeight / 1080;
        }

    }

    public async UniTask HeroView(BaseHero hero)
    {

        GameObject prefabHeroViewer;

        string addressableKey = $"HeroViewer";
        AsyncOperationHandle<GameObject> prefabHeroViewer_handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        _ = await prefabHeroViewer_handle.ToUniTask();
        prefabHeroViewer = prefabHeroViewer_handle.Status == AsyncOperationStatus.Succeeded
            ? prefabHeroViewer_handle.Result.SafeInstant()
            : throw new Exception($"{nameof(prefabHeroViewer)} не загружен");
        prefabHeroViewer.name = $"IconHero_{hero.name}";

        Canvas canvas = GameObjectFinder.FindByName<Canvas>($"Canvas_HeroViewer (id=6fpbu4db)", prefabHeroViewer.transform);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //Кнопка "Закрыть"
        const string buttonClose__Name = "ButtonClose (id=1berxtk2)";
        Button buttonCloseValue = GameObjectFinder.FindByName<Button>(buttonClose__Name, prefabHeroViewer.transform);
        RectTransform buttonClose__RT = GameObjectFinder.FindByName<RectTransform>(buttonClose__Name);
        _ = dictOnResizeButtonClose.TryAdd($"{buttonClose__Name}{buttonClose__RT.GetHashCode()}", buttonClose__RT);

        OnResizeAllDictotaries();

        //Имя героя
        const string text_HeroName__Name = "Text_HeroName (id=rw8uftqp)";
        TextMeshProUGUI text_HeroName = GameObjectFinder.FindByName<TextMeshProUGUI>(text_HeroName__Name, prefabHeroViewer.transform);
        text_HeroName.text = hero.name.ToUpper1Char();
        RectTransform text_HeroName__RT = GameObjectFinder.FindByName<RectTransform>(text_HeroName__Name);
        _ = dictOnResizeHeroName.TryAdd($"{text_HeroName__Name}{text_HeroName__RT.GetHashCode()}", text_HeroName__RT);
        TextMeshProUGUI textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(text_HeroName__Name);
        _ = dictOnResizeHeroNameFont.TryAdd($"{text_HeroName__Name}{textMeshProUGUI.GetHashCode()}", textMeshProUGUI);

        //Изображение героя
        const string imageHeroFull__Name = "Image_HeroFull (id=6z1ddxml)";
        Image imageHero = GameObjectFinder.FindByName<Image>(imageHeroFull__Name);

        imageHero.sprite = AddressablePrefabProvider.heroes[hero.name];
        imageHero.preserveAspect = true; // Сохраняет пропорции изображения
        imageHero.type = Image.Type.Simple; // Режим без растягивания;

        //Привязать метод
        buttonCloseValue.onClick.AddListener(() =>
        {
            string key = $"{buttonClose__Name}{buttonClose__RT.GetHashCode()}";
            if (dictOnResizeButtonClose.TryGetValue(key, out _))
            {
                _ = dictOnResizeButtonClose.Remove(key);
            }

            key = $"{text_HeroName__Name}{text_HeroName__RT.GetHashCode()}";
            if (dictOnResizeHeroName.TryGetValue(key, out _))
            {
                _ = dictOnResizeHeroName.Remove(key);
            }

            key = $"{text_HeroName__Name}{textMeshProUGUI.GetHashCode()}";
            if (dictOnResizeHeroNameFont.TryGetValue(key, out _))
            {
                _ = dictOnResizeHeroNameFont.Remove(key);
            }

            Destroy(prefabHeroViewer);
        });

        //Анимация
        await AllHeroesConsts.RunAnimationImageAsync(imageHero, 500);
    }

}
