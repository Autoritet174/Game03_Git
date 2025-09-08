using Assets.GameData.Scripts;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static General.Enums;
using static UnityEditor.Handles;
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
    private bool inited = false;

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


    public void HeroView(HeroBaseEntity hero)
    {
        //Debug.Log("Клик на изображение с параметром: " + inputString);
        // Здесь делайте что нужно с inputString

    }


    private void SetImage(AsyncOperationHandle<Sprite> handle, UnityEngine.UI.Image image)
    {
        image.sprite = handle.Result;

        // Настройка отображения:
        image.preserveAspect = true; // Сохраняет пропорции изображения
        image.type = Image.Type.Simple; // Режим без растягивания

        //// Настройки RectTransform
        //RectTransform rt = image.rectTransform;

        //// 1. Растягиваем по вертикали на 100%
        //rt.anchorMin = new Vector2(0f, 0.12f); // Якорь снизу по центру
        //rt.anchorMax = new Vector2(1f, 1f); // Якорь сверху по центру
        //rt.sizeDelta = new Vector2(0, 0);     // Растягиваем по якорям

        //// 2. Фиксируем ширину по пропорциям текстуры
        //float aspectRatio = (float)handle.Result.texture.width / handle.Result.texture.height;
        //float targetWidth = rt.rect.height * aspectRatio;
        //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);

        //// 3. Центрируем
        //rt.pivot = new Vector2(0.5f, 0.5f);
        //rt.anchoredPosition = Vector2.zero;
        ////image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
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

        // кнопка "Закрыть"
        float buttonClose_coefW = _lastWidth / 1920f;
        float buttonClose_coefH = _lastHeight / 1080f;
        float buttonClose_coef = buttonClose_coefW > buttonClose_coefH ? buttonClose_coefW : buttonClose_coefH;
        float pos = -55 * buttonClose_coef;
        float size = 100 * buttonClose_coef;
        buttonClose.anchoredPosition = new Vector2(pos, pos);
        buttonClose.sizeDelta = new Vector2(size, size);

    }
}
