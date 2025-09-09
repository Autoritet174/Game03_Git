using Assets.GameData.Scripts;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static General.Enums;
public class AllHeroes_Script : MonoBehaviour
{
    private ScrollRect scrollView;
    private RectTransform content;
    private RectTransform buttonClose;


    /// <summary>
    /// Количество колонок в сетке.
    /// </summary>
    private int columnCount = 8;

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
    /// Компонент RectTransform у вертикальной полосы прокрутки.
    /// </summary>
    private RectTransform verticalScrollbar;
    private readonly System.Collections.Concurrent.ConcurrentBag<TextMeshProUGUI> list_TextMeshProUGUI_heroNames = new();

    private float _lastHeight;
    private float _lastWidth;
    private const string heroImageNull = "hero-image-null";
    private static readonly List<HeroBaseEntity> allHeroes = new();

    [SerializeField]
    private GameObject prefabIconHero;

    [SerializeField]
    private GameObject prefabHeroViewer;
    private bool inited = false;

    private readonly Dictionary<string, RectTransform> dictOnResizeButtonClose = new();
    private readonly Dictionary<string, RectTransform> dictOnResizeHeroName = new();
    private readonly Dictionary<string, TextMeshProUGUI> dictOnResizeHeroNameFont = new();

    //private readonly int r = 4;
    private async void Start()
    {
        Task taskLoad = null;
        if (allHeroes.Count == 0)
        {
            taskLoad = FullListAllHeroes();
        }


        scrollView = GameObjectFinder.FindByName<ScrollRect>("Scroll View (id=2e9cbb1a)");
        content = GameObjectFinder.FindByName<RectTransform>("Content (id=0a40ce51)", startParent: scrollView.transform);
        buttonClose = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=5cd5cc79)");
        scrollRect = scrollView.GetComponent<ScrollRect>();
        GameObject ScrollbarVertical = GameObjectFinder.FindByName("Scrollbar Vertical (id=75511cdc)");
        verticalScrollbar = ScrollbarVertical.GetComponent<RectTransform>();
        scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        gridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();

        //ButtonCloseHelper.UpdateSize(_lastWidth, _lastHeight, buttonClose);
        OnResize();

        if (taskLoad != null)
        {
            await taskLoad;
        }

        await AddAllImageOnContent();



        inited = true;
    }

    private void Update()
    {
        if (inited && (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth)))
        {
            OnResize();
        }
    }

    private async Task FullListAllHeroes()
    {
        JObject jsonObject = await HttpRequester.GetResponceAsync(General.URLs.Uri_GetListAllHeroes);

        if (jsonObject == null)
        {
            throw new ArgumentNullException(nameof(jsonObject));
        }

        JToken heroesToken = jsonObject["heroes"];
        if (heroesToken is not JArray heroesArray)
        {
            throw new InvalidOperationException("Ключ 'heroes' отсутствует или не является массивом.");
        }

        allHeroes.Clear();
        foreach (JObject heroObj in heroesArray.Cast<JObject>())
        {
            Guid id = new(heroObj["id"]?.ToString());
            string name = heroObj["name"]?.ToString();
            RarityLevel rarity = (RarityLevel)Convert.ToInt32(heroObj["rarity"]);
            //Debug.Log(id);
            allHeroes.Add(new HeroBaseEntity(id, name, rarity));
        }

        allHeroes.Sort(static (a, b) =>
        {
            int result = b.Rarity.CompareTo(a.Rarity);
            if (result != 0)
            {// сортировка по редкости по убыванию
                return result;
            }

            // сортировка по имени по возрастанию
            return a.Name.CompareTo(b.Name);
        });
    }

    private async Task AddAllImageOnContent()
    {
        List<Task> list = new();
        foreach (HeroBaseEntity heroStats in allHeroes)
        {
            list.Add(LoadHeroByName(heroStats));
        }
        OnResize();
        await Task.WhenAll(list);
    }

    private async Task LoadHeroByName(HeroBaseEntity hero)
    {
        GameObject _prefabIconHero = Instantiate(prefabIconHero);
        string heroName = hero.Name;
        _prefabIconHero.name = heroName;

        Transform transform = _prefabIconHero.transform;
        transform.SetParent(content.transform, false);

        // Текст (может быть установлен сразу)
        Transform childText = _prefabIconHero.transform.Find("TextHero");
        if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
        {
            textMeshPro.text = heroName;
            list_TextMeshProUGUI_heroNames.Add(textMeshPro);
        }

        // Изображение (загружаем через Addressable)
        Transform childImageMaskHero = _prefabIconHero.transform.Find("ImageMaskHero");
        Transform childImageMaskRarity = _prefabIconHero.transform.Find("ImageMaskRarity");
        Transform childImageHero = childImageMaskHero.Find("ImageHero");
        Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");
        if (childImageHero != null && childImageHero.TryGetComponent(out UnityEngine.UI.Image imageHero)
            && childImageRarity != null && childImageRarity.TryGetComponent(out UnityEngine.UI.Image imageRarity)
            )
        {
            string addressableKey = $"hero-image-{heroName.ToLower()}-face"; // Ключ без расширения .png

            try
            {
                AsyncOperationHandle<Sprite> handleHero;
                bool loadNull = true;
                bool loadColor = true;
                AsyncOperationHandle<Sprite> handleRarity = Addressables.LoadAssetAsync<Sprite>($"rarity{(int)hero.Rarity}");
                AsyncOperationHandle<Sprite> handleEntered = Addressables.LoadAssetAsync<Sprite>($"raritySelected");
                if (await AddressableHelper.CheckIfKeyExists(addressableKey))
                {
                    handleHero = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                    _ = await handleHero.Task;
                    _ = await handleRarity.Task;
                    if (handleHero.Status == AsyncOperationStatus.Succeeded && handleHero.Result != null)
                    {
                        SetImage(handleHero, imageHero);
                        SetImage(handleRarity, imageRarity);
                        loadNull = false;
                        loadColor = false;

                        // Добавляем компонент для обработки кликов
                        ImageHeroHandler clickHandler = _prefabIconHero.AddComponent<ImageHeroHandler>();
                        clickHandler.Initialize(hero, handleRarity.Result, handleEntered.Result, HeroView, imageRarity);

                        //Addressables.Release(handle);
                    }
                }


                if (loadNull && await AddressableHelper.CheckIfKeyExists(heroImageNull))
                {
                    handleHero = Addressables.LoadAssetAsync<Sprite>(heroImageNull);
                    _ = await handleHero.Task;
                    if (handleHero.Status == AsyncOperationStatus.Succeeded && handleHero.Result != null)
                    {
                        SetImage(handleHero, imageHero);
                        SetImage(handleRarity, imageRarity);
                        loadColor = false;
                        //Addressables.Release(handle);
                    }
                }


                if (loadColor)
                {
                    Debug.LogError($"Не удалось загрузить изображение {heroName} из Addressable Assets!");
                    imageHero.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    SetImage(handleRarity, imageRarity);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке изображения {heroName}: {ex.Message}");
                imageHero.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }
        }
    }


    public async Task HeroView(HeroBaseEntity hero)
    {
        GameObject _prefabHeroViewer = Instantiate(prefabHeroViewer);
        Canvas canvas = _prefabHeroViewer.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //Кнопка "Закрыть"
        const string _buttonClose__Name = "ButtonClose (id=1berxtk2)";
        Button _buttonClose = GameObjectFinder.FindByName<Button>(_buttonClose__Name, _prefabHeroViewer.transform);

        RectTransform _buttonClose__RT = GameObjectFinder.FindByName<RectTransform>(_buttonClose__Name);
        _ = dictOnResizeButtonClose.TryAdd($"{_buttonClose__Name}{_buttonClose__RT.GetHashCode()}", _buttonClose__RT);

        await Task.Delay(0);

        OnResizeAllDictotaries();



        //Имя героя
        const string _Text_HeroName__Name = "Text_HeroName (id=rw8uftqp)";
        TextMeshProUGUI _Text_HeroName = GameObjectFinder.FindByName<TextMeshProUGUI>(_Text_HeroName__Name, _prefabHeroViewer.transform);
        _Text_HeroName.text = hero.Name;

        RectTransform _Text_HeroName__RT = GameObjectFinder.FindByName<RectTransform>(_Text_HeroName__Name);
        _ = dictOnResizeHeroName.TryAdd($"{_Text_HeroName__Name}{_Text_HeroName__RT.GetHashCode()}", _Text_HeroName__RT);

        TextMeshProUGUI textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(_Text_HeroName__Name);
        _ = dictOnResizeHeroNameFont.TryAdd($"{_Text_HeroName__Name}{textMeshProUGUI.GetHashCode()}", textMeshProUGUI);


        //Изображение героя
        const string imageHeroFull__Name = "Image_HeroFull (id=6z1ddxml)";
        Image imageHero = GameObjectFinder.FindByName<Image>(imageHeroFull__Name);




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

            Destroy(_prefabHeroViewer);
        });
    }


    private void SetImage(AsyncOperationHandle<Sprite> handle, UnityEngine.UI.Image image)
    {
        image.sprite = handle.Result;
        image.preserveAspect = true; // Сохраняет пропорции изображения
        image.type = Image.Type.Simple; // Режим без растягивания;
    }

    private void OnResize()
    {
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        // Устанавливаем Constraint как FixedColumnCount
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        float scrollViewWidth = scrollRectTransform.rect.width * 0.95f;
        float scrollbarWidth = verticalScrollbar.rect.width;

        float percentWidthForImage = 0.9f;

        //float totalAvailableWidth = scrollViewWidth - scrollbarWidth - (horizontalPadding * 2) - (spacing * (columnCount - 1));
        float totalAvailableWidth = scrollViewWidth - scrollbarWidth;
        //Debug.Log("totalAvailableWidth=" + totalAvailableWidth);
        if (totalAvailableWidth < 200)
        {
            totalAvailableWidth = 200;
        }

        columnCount = totalAvailableWidth switch
        {
            <= 300 => 3,
            <= 800 => Mathf.RoundToInt(totalAvailableWidth / 100),
            _ => 8,
        };

        // Устанавливаем количество колонокя
        gridLayout.constraintCount = columnCount;
        float cellWidth = totalAvailableWidth / columnCount * percentWidthForImage;
        gridLayout.cellSize = new Vector2(cellWidth, cellWidth);

        // Отступ между элементами в пикселях.
        float spacing = totalAvailableWidth / (columnCount - 1) * (1 - percentWidthForImage);
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
}
