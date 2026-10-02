using Assets.GameData.Scenes.AllHeroes;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using L = General.LocalizationKeys;

/// <summary>Загружает каталог героев и отображает подробности выбранного героя.</summary>
public class AllHeroes : MonoBehaviour
{
    private const string HERO_VIEWER_ADDRESS = "HeroViewer";
    private const string HERO_VIEWER_CANVAS_NAME = "Canvas_HeroViewer (id=6fpbu4db)";
    private const string HERO_VIEWER_CLOSE_BUTTON_NAME = "ButtonClose (id=1berxtk2)";
    private const string HERO_VIEWER_NAME_TEXT_NAME = "Text_HeroName (id=rw8uftqp)";
    private const string HERO_VIEWER_IMAGE_NAME = "Image_HeroFull (id=6z1ddxml)";
    private const float REFERENCE_HEIGHT = 1080f;
    private const float HERO_NAME_BOTTOM_OFFSET = 993f;
    private const float HERO_NAME_FONT_SIZE = 66.66666f;
    private const float ICON_WIDTH_RATIO = 0.9f;

    [SerializeField]
    private GameObject prefabIconHero;

    private readonly List<TextMeshProUGUI> heroNames = new();
    private readonly List<(GameObject instance, RectTransform closeButton, TextMeshProUGUI heroName)> heroViewers = new();

    private ScrollRect scrollRect;
    private RectTransform content;
    private RectTransform closeButton;
    private RectTransform scrollRectTransform;
    private RectTransform verticalScrollbar;
    private GridLayoutGroup gridLayout;
    private AsyncOperationHandle<GameObject> heroViewerHandle;
    private int lastHeight;
    private int lastWidth;
    private bool isInitialized;

    #region Жизненный цикл

    private void Start()
    {
        InitializeScene();
        CreateHeroIcons();
        ResizeWindow();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || (Screen.height == lastHeight && Screen.width == lastWidth))
        {
            return;
        }

        ResizeWindow();
    }

    private void OnDestroy()
    {
        foreach ((GameObject instance, RectTransform closeButton, TextMeshProUGUI heroName) viewer in heroViewers)
        {
            if (viewer.instance != null)
            {
                Destroy(viewer.instance);
            }
        }

        heroViewers.Clear();
        heroNames.Clear();

        if (heroViewerHandle.IsValid())
        {
            Addressables.Release(heroViewerHandle);
        }
    }

    #endregion Жизненный цикл

    #region Каталог героев

    /// <summary>Получает ссылки на элементы каталога из сцены.</summary>
    private void InitializeScene()
    {
        scrollRect = GameObjectFinder.FindByName<ScrollRect>("Scroll View (id=2e9cbb1a)");
        content = GameObjectFinder.FindByName<RectTransform>("Content (id=0a40ce51)", startParent: scrollRect.transform);
        closeButton = GameObjectFinder.FindByName<RectTransform>("ButtonClose");
        verticalScrollbar = GameObjectFinder.FindByName<RectTransform>("Scrollbar Vertical (id=75511cdc)");
        scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        gridLayout = content.GetComponent<GridLayoutGroup>();
    }

    /// <summary>Создаёт иконки из уже загруженных данных в порядке убывания редкости.</summary>
    private void CreateHeroIcons()
    {
        foreach (BaseHero hero in Game03Client.GameData.container.baseHeroes.OrderByDescending(static hero => hero.rarity))
        {
            CreateHeroIcon(hero);
        }
    }

    /// <summary>Настраивает изображение, подпись и обработчики иконки героя.</summary>
    private void CreateHeroIcon(BaseHero hero)
    {
        GameObject icon = prefabIconHero.SafeInstant();
        if (icon == null)
        {
            return;
        }

        icon.name = hero.name;
        icon.transform.SetParent(content, false);

        Transform nameTransform = icon.transform.Find("TextCollectionElement");
        if (nameTransform == null || !nameTransform.TryGetComponent(out TextMeshProUGUI heroName))
        {
            throw new InvalidOperationException("В иконке героя отсутствует TextCollectionElement.");
        }

        heroName.text = hero.name.ToUpper1Char();
        heroNames.Add(heroName);

        Image portrait = icon.transform.Find("ImageMaskCollectionElement/ImageCollectionElement").GetComponent<Image>();
        Image rarity = icon.transform.Find("ImageMaskRarity/ImageRarity").GetComponent<Image>();
        SetImage(portrait, AddressablePrefabProvider.heroes[hero.name + "_face"]);
        SetImage(rarity, AddressablePrefabProvider.GetRarity(hero.rarity));

        UniTask OnClickAsync()
        {
            return HeroView(hero);
        }

        void OnPointerEnter()
        {
            rarity.sprite = AddressablePrefabProvider.raritySelected;
        }

        void OnPointerExit()
        {
            rarity.sprite = AddressablePrefabProvider.GetRarity(hero.rarity);
        }

        icon.SetClickOnGameObject(OnClickAsync);
        icon.SetHoverEvents(OnPointerEnter, OnPointerExit);
    }

    private static void SetImage(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        image.preserveAspect = true;
        image.type = Image.Type.Simple;
    }

    #endregion Каталог героев

    #region Размеры интерфейса

    /// <summary>Пересчитывает сетку каталога и размеры открытых окон при изменении разрешения.</summary>
    private void ResizeWindow()
    {
        lastHeight = Screen.height;
        lastWidth = Screen.width;

        float availableWidth = Mathf.Max(200f, (scrollRectTransform.rect.width * 0.95f) - verticalScrollbar.rect.width);
        int columnCount = availableWidth switch
        {
            <= 300f => 3,
            <= 800f => Mathf.RoundToInt(availableWidth / 100f),
            _ => 8
        };

        float cellWidth = availableWidth / columnCount * ICON_WIDTH_RATIO;
        float spacing = availableWidth / (columnCount - 1) * (1f - ICON_WIDTH_RATIO);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnCount;
        gridLayout.cellSize = new(cellWidth, cellWidth);
        gridLayout.spacing = new(spacing, spacing);

        // Один шаг колеса прокручивает каталог на высоту ячейки с отступом.
        scrollRect.scrollSensitivity = cellWidth + spacing;

        foreach (TextMeshProUGUI heroName in heroNames)
        {
            heroName.fontSize = cellWidth * 0.16f;
        }

        ButtonCloseHelper.UpdateSize(closeButton);

        foreach ((GameObject instance, RectTransform closeButton, TextMeshProUGUI heroName) viewer in heroViewers)
        {
            if (viewer.instance != null)
            {
                ResizeHeroViewer(viewer.closeButton, viewer.heroName);
            }
        }
    }

    private static void ResizeHeroViewer(RectTransform closeButton, TextMeshProUGUI heroName)
    {
        float heightRatio = Screen.height / REFERENCE_HEIGHT;
        ButtonCloseHelper.UpdateSize(closeButton);
        heroName.rectTransform.offsetMin = new(0f, HERO_NAME_BOTTOM_OFFSET * heightRatio);
        heroName.fontSize = HERO_NAME_FONT_SIZE * heightRatio;
    }

    #endregion Размеры интерфейса

    #region Окно выбранного героя

    /// <summary>Открывает сведения о герое; имя метода сохранено для совместимости с существующими вызовами.</summary>
    public UniTask HeroView(BaseHero hero)
    {
        return ShowHeroAsync(hero, this.GetCancellationTokenOnDestroy());
    }

    /// <summary>Загружает префаб окна и прекращает открытие при уничтожении каталога.</summary>
    private async UniTask ShowHeroAsync(BaseHero hero, CancellationToken cancellationToken)
    {
        if (hero is null)
        {
            throw new ArgumentNullException(nameof(hero));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Общий префаб удерживается, пока существует каталог и его окна.
        if (!heroViewerHandle.IsValid())
        {
            heroViewerHandle = Addressables.LoadAssetAsync<GameObject>(HERO_VIEWER_ADDRESS);
        }

        GameObject prefab;
        try
        {
            prefab = await heroViewerHandle.ToUniTask(cancellationToken: cancellationToken);
        }
        catch
        {
            if (heroViewerHandle.IsValid() && heroViewerHandle.Status == AsyncOperationStatus.Failed)
            {
                Addressables.Release(heroViewerHandle);
                heroViewerHandle = default;
            }

            throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        GameObject viewer = prefab.SafeInstant();
        if (viewer == null)
        {
            return;
        }

        Image portrait;
        try
        {
            portrait = InitializeHeroViewer(viewer, hero);
        }
        catch
        {
            CloseHeroViewer(viewer);
            throw;
        }

        await AllHeroesConsts.RunAnimationImageAsync(portrait, 500);
    }

    /// <summary>Настраивает только элементы созданного окна, не затрагивая другие открытые окна.</summary>
    private Image InitializeHeroViewer(GameObject viewer, BaseHero hero)
    {
        viewer.name = $"IconHero_{hero.name}";
        Transform root = viewer.transform;
        Canvas canvas = GameObjectFinder.FindByName<Canvas>(HERO_VIEWER_CANVAS_NAME, root);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Button viewerCloseButton = GameObjectFinder.FindByName<Button>(HERO_VIEWER_CLOSE_BUTTON_NAME, root);
        RectTransform viewerCloseButtonTransform = viewerCloseButton.GetComponent<RectTransform>();
        TextMeshProUGUI heroName = GameObjectFinder.FindByName<TextMeshProUGUI>(HERO_VIEWER_NAME_TEXT_NAME, root);
        heroName.text = hero.name.ToUpper1Char();
        CreateHeroDetails(hero, heroName);

        Image portrait = GameObjectFinder.FindByName<Image>(HERO_VIEWER_IMAGE_NAME, root);
        SetImage(portrait, AddressablePrefabProvider.heroes[hero.name]);

        heroViewers.Add((viewer, viewerCloseButtonTransform, heroName));
        viewerCloseButton.onClick.AddListener(() => CloseHeroViewer(viewer));
        ResizeHeroViewer(viewerCloseButtonTransform, heroName);
        return portrait;
    }

    private void CloseHeroViewer(GameObject viewer)
    {
        _ = heroViewers.RemoveAll(item => item.instance == viewer);
        if (viewer != null)
        {
            Destroy(viewer);
        }
    }

    #endregion Окно выбранного героя

    #region Подробные характеристики героя

    /// <summary>Размещает сведения о базовом герое под именем в правой части окна.</summary>
    private static void CreateHeroDetails(BaseHero hero, TextMeshProUGUI heroName)
    {
        TextMeshProUGUI details = Instantiate(heroName, heroName.transform.parent, false);
        details.name = "Text_HeroDetails";
        details.raycastTarget = false;
        details.alignment = TextAlignmentOptions.TopLeft;
        details.fontStyle = FontStyles.Normal;
        details.richText = true;
        details.textWrappingMode = TextWrappingModes.NoWrap;
        details.enableAutoSizing = true;
        details.fontSizeMin = 1f;
        details.fontSizeMax = 32f;
        details.margin = Vector4.zero;

        RectTransform rectTransform = details.rectTransform;
        rectTransform.anchorMin = new(0.52f, 0.04f);
        rectTransform.anchorMax = new(0.98f, 0.90f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        details.text = BuildHeroDetails(hero);
    }

    /// <summary>Формирует основные сведения и таблицу характеристик базового героя.</summary>
    private static string BuildHeroDetails(BaseHero hero)
    {
        string mainStat = hero.mainStat switch
        {
            EMainStat.universal => GetStatLabel(nameof(EMainStat.universal)),
            EMainStat.strength => GetStatLabel(nameof(BaseHero.strength)),
            EMainStat.agility => GetStatLabel(nameof(BaseHero.agility)),
            EMainStat.intelligence => GetStatLabel(nameof(BaseHero.intelligence)),
            _ => hero.mainStat.ToString()
        };

        StringBuilder text = new();
        _ = text.Append("<line-height=135%>");
        _ = text.AppendLine(GetRarityLabel(hero.rarity));
        _ = text.AppendLine($"{Game03Client.LocalizationManager.GetValue(L.UI.Label.mainStat)}: {mainStat}");
        _ = text.AppendLine();
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.health)), hero.health);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.damage)), hero.damage);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.strength)), hero.strength);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.agility)), hero.agility);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.intelligence)), hero.intelligence);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.critChance)), hero.critChance);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.critMultiplier)), hero.critMultiplier);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.haste)), hero.haste);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.versality)), hero.versality);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.endurancePhysical)), hero.endurancePhysical);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.enduranceMagical)), hero.enduranceMagical);
        AppendDiceDetails(text, GetStatLabel(nameof(BaseHero.initiative)), hero.initiative);
        _ = text.Append("</line-height>");
        return text.ToString();
    }

    /// <summary>Получает подпись по имени свойства DTO, сохраняя регистр ключей в JSON локализации.</summary>
    private static string GetStatLabel(string statName)
    {
        return Game03Client.LocalizationManager.GetValue(L.UI.Label.Stat.GetKey(statName));
    }

    /// <summary>Выводит название известной редкости и сохраняет её числовое значение.</summary>
    private static string GetRarityLabel(int rarity)
    {
        string localizationKey = rarity switch
        {
            1 => L.UI.Label.Rarity.r1,
            2 => L.UI.Label.Rarity.r2,
            3 => L.UI.Label.Rarity.r3,
            4 => L.UI.Label.Rarity.r4,
            5 => L.UI.Label.Rarity.r5,
            _ => null
        };

        return localizationKey is null
            ? rarity.ToString()
            : $"{Game03Client.LocalizationManager.GetValue(localizationKey)} ({rarity})";
    }

    /// <summary>Выводит границы и математическое ожидание из общего DTO без случайного броска.</summary>
    private static void AppendDiceDetails(StringBuilder text, string label, Dice dice)
    {
        if (dice is null)
        {
            _ = text.AppendLine($"{label}<pos=48%>—<pos=65%>—<pos=82%>—");
            return;
        }

        _ = text.AppendLine($"{label}<pos=48%>{dice.min}<pos=65%>{dice.max}<pos=82%>{dice.expected}");
    }

    #endregion Подробные характеристики героя

}
