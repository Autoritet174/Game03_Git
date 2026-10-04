using Assets.GameData.Scenes.AllHeroes;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private const int HERO_TABLE_ROW_COUNT = 13;

    [SerializeField]
    private GameObject prefabIconHero;

    private readonly List<TextMeshProUGUI> heroNames = new();
    private readonly List<(GameObject instance, RectTransform closeButton, TextMeshProUGUI heroName)> heroViewers = new();

    private ScrollRect scrollRect;
    private RectTransform content;

    public PanelTop__prefab__scriptMB panelTop__prefab__context { get; private set; }
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

        panelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
        panelTop__prefab__context.Initialize();
        panelTop__prefab__context.SetActionOnButtonClose(G.ButtonCloseOnClick);

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


        float coefHeight = G.GetCoefHeight();
        panelTop__prefab__context.OnResized(coefHeight);

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

    /// <summary>Размещает основные сведения и таблицу под именем в правой части окна.</summary>
    private static void CreateHeroDetails(BaseHero hero, TextMeshProUGUI heroName)
    {
        RectTransform details = CreateDetailsContainer("HeroDetails", heroName.transform.parent);
        details.anchorMin = new(0.52f, 0.04f);
        details.anchorMax = new(0.98f, 0.90f);
        BuildHeroDetails(hero, details, heroName);
    }

    /// <summary>Формирует сведения о герое и двенадцать строк таблицы с четырьмя колонками.</summary>
    private static void BuildHeroDetails(BaseHero hero, RectTransform details, TextMeshProUGUI template)
    {
        string mainStat = hero.mainStat switch
        {
            EMainStat.universal => GetStatLabel(nameof(EMainStat.universal)),
            EMainStat.strength => GetStatLabel(nameof(BaseHero.strength)),
            EMainStat.agility => GetStatLabel(nameof(BaseHero.agility)),
            EMainStat.intelligence => GetStatLabel(nameof(BaseHero.intelligence)),
            _ => hero.mainStat.ToString()
        };

        string summary = $"{Game03Client.LocalizationManager.GetValue(L.UI.Label.rarity)}: {GetRarityLabel(hero.rarity)}"
            + $"\n{Game03Client.LocalizationManager.GetValue(L.UI.Label.mainStat)}: {mainStat}";
        TextMeshProUGUI summaryText = CreateDetailsCell(details, template, summary,
            new(0f, 0.84f), Vector2.one, TextAlignmentOptions.TopLeft, 32f);
        summaryText.name = "HeroDetailsSummary";

        RectTransform table = CreateDetailsContainer("HeroDetailsTable", details);
        table.anchorMax = new(1f, 0.8f);

        AppendTableRow(table, template, 0,
            Game03Client.LocalizationManager.GetValue(L.UI.Label.characteristic),
            Game03Client.LocalizationManager.GetValue(L.UI.Label.expectedValue),
            Game03Client.LocalizationManager.GetValue(L.UI.Label.dice),
            Game03Client.LocalizationManager.GetValue(L.UI.Label.range), true);

        AppendDiceDetails(table, template, 0, GetStatLabel(nameof(BaseHero.health)), hero.health);
        AppendDiceDetails(table, template, 1, GetStatLabel(nameof(BaseHero.damage)), hero.damage);
        AppendDiceDetails(table, template, 2, GetStatLabel(nameof(BaseHero.strength)), hero.strength);
        AppendDiceDetails(table, template, 3, GetStatLabel(nameof(BaseHero.agility)), hero.agility);
        AppendDiceDetails(table, template, 4, GetStatLabel(nameof(BaseHero.intelligence)), hero.intelligence);
        AppendDiceDetails(table, template, 5, GetStatLabel(nameof(BaseHero.critChance)), hero.critChance);
        AppendDiceDetails(table, template, 6, GetStatLabel(nameof(BaseHero.critMultiplier)), hero.critMultiplier);
        AppendDiceDetails(table, template, 7, GetStatLabel(nameof(BaseHero.haste)), hero.haste);
        AppendDiceDetails(table, template, 8, GetStatLabel(nameof(BaseHero.versality)), hero.versality);
        AppendDiceDetails(table, template, 9, GetStatLabel(nameof(BaseHero.endurancePhysical)), hero.endurancePhysical);
        AppendDiceDetails(table, template, 10, GetStatLabel(nameof(BaseHero.enduranceMagical)), hero.enduranceMagical);
        AppendDiceDetails(table, template, 11, GetStatLabel(nameof(BaseHero.initiative)), hero.initiative);

        CreateTableGrid(table);
    }

    /// <summary>Создаёт контейнер, растянутый по родителю и не зависящий от размера окна.</summary>
    private static RectTransform CreateDetailsContainer(string name, Transform parent)
    {
        GameObject container = new(name, typeof(RectTransform));
        container.layer = parent.gameObject.layer;
        RectTransform rectTransform = container.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return rectTransform;
    }

    /// <summary>Создаёт текстовую ячейку с собственным выравниванием и автоподбором размера шрифта.</summary>
    private static TextMeshProUGUI CreateDetailsCell(RectTransform parent, TextMeshProUGUI template,
        string text, Vector2 anchorMin, Vector2 anchorMax, TextAlignmentOptions alignment, float fontSizeMax, FontStyles fontStyle = FontStyles.Normal, bool wrapText = false)
    {
        TextMeshProUGUI cell = Instantiate(template, parent, false);
        cell.name = "Cell";
        cell.raycastTarget = false;
        cell.alignment = alignment;
        cell.fontStyle = fontStyle;
        cell.richText = false;
        cell.textWrappingMode = wrapText ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
        cell.overflowMode = TextOverflowModes.Ellipsis;
        cell.enableAutoSizing = true;
        cell.fontSizeMin = 1f;
        cell.fontSizeMax = fontSizeMax;
        cell.margin = new(8f, 2f, 8f, 2f);
        cell.text = text;

        RectTransform rectTransform = cell.rectTransform;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return cell;
    }

    /// <summary>Рисует внешнюю рамку и разделители всех строк и колонок.</summary>
    private static void CreateTableGrid(RectTransform table)
    {
        float[] columnEdges = { 0f, 0.44f, 0.58f, 0.78f, 1f };
        foreach (float edge in columnEdges)
        {
            CreateTableLine(table, new(edge, 0f), new(edge, 1f), true);
        }

        for (int row = 0; row <= HERO_TABLE_ROW_COUNT; row++)
        {
            float edge = row / (float)HERO_TABLE_ROW_COUNT;
            CreateTableLine(table, new(0f, edge), new(1f, edge), false);
        }
    }

    private static void CreateTableLine(RectTransform table, Vector2 anchorMin, Vector2 anchorMax, bool vertical)
    {
        RectTransform line = CreateDetailsContainer(vertical ? "ColumnBorder" : "RowBorder", table);
        line.anchorMin = anchorMin;
        line.anchorMax = anchorMax;
        line.sizeDelta = vertical ? new(1f, 0f) : new(0f, 1f);
        line.anchoredPosition = Vector2.zero;

        Image image = line.gameObject.AddComponent<Image>();
        image.color = new(1f, 1f, 1f, 0.5f);
        image.raycastTarget = false;
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
            : $"{Game03Client.LocalizationManager.GetValue(localizationKey)}";
    }

    /// <summary>Добавляет ячейки строки: название слева, ожидание справа, Dice и диапазон по центру.</summary>
    private static void AppendDiceDetails(RectTransform table, TextMeshProUGUI template, int row, string label, Dice dice)
    {
        string expected = dice is null ? "—" : dice.expected.ToString();
        string expression = dice is null ? "—" : dice.ToStr().Replace("d", "<color=#00FF00>d</color>");
        string range = dice is null ? "—" : $"{dice.min} - {dice.max}";
        AppendTableRow(table, template, row + 1, label, expected, expression, range);
    }

    /// <summary>Создаёт строку таблицы; заголовки выделяет полужирным шрифтом.</summary>
    private static void AppendTableRow(RectTransform table, TextMeshProUGUI template, int row,
        string label, string expected, string expression, string range, bool isHeader = false)
    {
        float bottom = 1f - (row + 1) / (float)HERO_TABLE_ROW_COUNT;
        float top = 1f - row / (float)HERO_TABLE_ROW_COUNT;
        FontStyles fontStyle = isHeader ? FontStyles.Bold : FontStyles.Normal;
        float secondaryFontSize = isHeader ? 32f : 24f;

        _ = CreateDetailsCell(table, template, label,
            new(0f, bottom), new(0.44f, top), TextAlignmentOptions.Left, 32f, fontStyle, isHeader);
        _ = CreateDetailsCell(table, template, expected,
            new(0.44f, bottom), new(0.58f, top), TextAlignmentOptions.Right, 32f, fontStyle, isHeader);
        TextMeshProUGUI diceCell = CreateDetailsCell(table, template, expression,
            new(0.58f, bottom), new(0.78f, top), TextAlignmentOptions.Center, secondaryFontSize, fontStyle, isHeader);
        diceCell.richText = !isHeader;
        _ = CreateDetailsCell(table, template, range,
            new(0.78f, bottom), new(1f, top), TextAlignmentOptions.Center, secondaryFontSize, fontStyle, isHeader);
    }

    #endregion Подробные характеристики героя

}
