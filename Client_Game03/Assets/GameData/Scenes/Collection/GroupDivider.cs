using Assets.GameData.Scripts;
using General;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
/// с асинхронной анимацией высоты.
/// </summary>
public class GroupDivider : MonoBehaviour
{
    private readonly string group_name;

    private readonly GameObject _GameObject;
    private readonly RectTransform _RectTransform;

    private readonly GameObject _DividerButton_GameObject;
    private readonly RectTransform _DividerButton_RectTransform;

    private readonly GameObject _CellsContainer_GameObject;
    private readonly RectTransform _CellsContainer_RectTransform;
    private readonly GridLayoutGroup _CellsContainer_GridLayoutGroup;
    private readonly IEnumerable<General.GameEntities.CollectionHero> _listHeroes;

    public GroupDivider(string group_name, GameObject gameObject, IEnumerable<General.GameEntities.CollectionHero> listHeroes)
    {
        this.group_name = group_name;
        _GameObject = gameObject;
        _RectTransform = gameObject.GetComponent<RectTransform>();
        _DividerButton_GameObject = GameObjectFinder.FindByName("DividerButton", _GameObject.transform);
        _DividerButton_RectTransform = _DividerButton_GameObject.GetComponent<RectTransform>();
        _CellsContainer_GameObject = GameObjectFinder.FindByName("CellsContainer", _GameObject.transform);
        _CellsContainer_RectTransform = _CellsContainer_GameObject.GetComponent<RectTransform>();
        _CellsContainer_GridLayoutGroup = _CellsContainer_GameObject.GetComponent<GridLayoutGroup>();
        _listHeroes = listHeroes;
    }
    public async Task Init()
    {
        //DividerButton
        TextMeshProUGUI dividerButton_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", _DividerButton_GameObject.transform);
        Transform cellsContainer_Transform = _CellsContainer_GameObject.transform;
        if (group_name.IsEmpty())
        {
            dividerButton_TextMeshProUGUI.text = "---No Group---";
            dividerButton_TextMeshProUGUI.fontStyle = FontStyles.Italic;
        }
        else
        {
            dividerButton_TextMeshProUGUI.text = group_name;
        }


        Sprite raritySelected_Sprite = await Addressables.LoadAssetAsync<Sprite>($"raritySelected").Task;

        foreach (General.GameEntities.CollectionHero groupHero in _listHeroes)
        {
            HeroBase heroBase = groupHero.HeroBase;
            GameObject gameObject = await Addressables.LoadAssetAsync<GameObject>("IconHero").Task;
            GameObject _prefabIconHero = gameObject.SafeInstant();
            _prefabIconHero.transform.SetParent(cellsContainer_Transform);

            RectTransform _prefabIconHero_Transform = _prefabIconHero.GetComponent<RectTransform>();
            _prefabIconHero_Transform.anchoredPosition3D = Vector3.zero;
            _prefabIconHero_Transform.localScale = Vector3.one;



            Transform childImageMaskHero = _prefabIconHero.transform.Find("ImageMaskHero");
            Transform childImageMaskRarity = _prefabIconHero.transform.Find("ImageMaskRarity");
            Transform childImageHero = childImageMaskHero.Find("ImageHero");
            Transform childImageRarity = childImageMaskRarity.Find("ImageRarity");
            if (childImageHero != null && childImageHero.TryGetComponent(out Image imageHero) && childImageRarity != null && childImageRarity.TryGetComponent(out Image imageRarity))
            {

                Transform childText = _prefabIconHero.transform.Find("TextHero");
                if (childText != null && childText.TryGetComponent(out TextMeshProUGUI textMeshPro))
                {
                    textMeshPro.text = heroBase.Name.ToUpper1Char();
                    textMeshPro.fontSize = 12;
                }

                imageRarity.sprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{(int)heroBase.Rarity}").Task;
                imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
                imageRarity.type = Image.Type.Simple; // Режим без растягивания;

                string addressableKey = $"hero-image-{heroBase.Name.ToLower()}_face";
                imageHero.sprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).Task;
                imageHero.preserveAspect = true; // Сохраняет пропорции изображения
                imageHero.type = Image.Type.Simple; // Режим без растягивания;

                ImageHeroHandler clickHandler = _prefabIconHero.AddComponent<ImageHeroHandler>();
                clickHandler.Initialize(heroBase, imageRarity.sprite, raritySelected_Sprite, HeroView, imageRarity);
            }
        }
    }

    private async Task HeroView(HeroBase entity)
    {
        Debug.Log(entity.Name);
        //throw new NotImplementedException();
    }
    public void Resize(float width, float coefHeight)
    {
        float cellSize1080 = 106f;
        float spacing1080 = 10f;
        int paddingR = (int)(40f * coefHeight);
        float spacing = spacing1080 * coefHeight;
        float cellSize = cellSize1080 * coefHeight;
        //расчитываем сколько при этих параметрах войдет ячеек
        float widthWithoutPadding = width - (paddingR + spacing);
        int countCellInRow = (int)(widthWithoutPadding / cellSize);
        float spacingDelta = widthWithoutPadding / ((countCellInRow * (cellSize1080 / spacing1080)) + (countCellInRow - 1));
        float cellSizeDelta = spacingDelta * cellSize1080 / spacing1080;


        _CellsContainer_GridLayoutGroup.padding.left = (int)spacingDelta;
        _CellsContainer_GridLayoutGroup.padding.right = paddingR;
        _CellsContainer_GridLayoutGroup.padding.top = (int)spacingDelta;
        _CellsContainer_GridLayoutGroup.padding.bottom = (int)spacingDelta;
        _CellsContainer_GridLayoutGroup.spacing = new Vector2(spacingDelta, spacingDelta);
        _CellsContainer_GridLayoutGroup.cellSize = new Vector2(cellSizeDelta, cellSizeDelta);

        // вычисляем количество строк
        int countHeroes = _listHeroes.Count();
        int countRows = (countHeroes / countCellInRow) + (countHeroes % countCellInRow == 0 ? 0 : 1);
        if (countRows < 1)
        {
            countRows = 1;
        }
        
        float heightContainer = (countRows * cellSizeDelta) + ((countRows+1) * spacingDelta);

        float heightButton = 45f * coefHeight;
        _DividerButton_RectTransform.sizeDelta = new Vector2(width, heightButton);
        _CellsContainer_RectTransform.sizeDelta = new Vector2(width, heightContainer);// в зависимости от героев и отступов
        _RectTransform.sizeDelta = new Vector2(width, heightButton + heightContainer);

        _CellsContainer_RectTransform.anchoredPosition = new Vector2(0, -45f * coefHeight);
    }


    // --- Публичные поля, которые настраиваются в Инспекторе ---

    /// <summary>
    /// Контейнер, содержащий все ячейки инвентаря для этой группы.
    /// На этом объекте должен быть RectTransform.
    /// </summary>
    private readonly GameObject gameObjectCellsContainer;

    /// <summary>
    /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
    /// </summary>
    private readonly Button buttonDivider;
    //[SerializeField]
    //private RectTransform rectTransformDividerButton

    //[SerializeField]
    //private RectTransform rectTransformGroupDivider;

    /// <summary>
    /// Скорость анимации (единиц в секунду).
    /// </summary>
    //[Tooltip("Скорость анимации в единицах высоты в секунду.")]
    //[SerializeField]
    //private float animationSpeed = 1000f;

    /// <summary>
    /// Текущее состояние группы (true - развернута, false - свернута).
    /// </summary>
    private bool expanded = true;

    /// <summary>
    /// Флаг, переключаем в true при вызове OnDestroy для остановки анимаций.
    /// </summary>
    private bool destroying = false;


    //private RectTransform cellsRectTransform;
    //private float expandedHeight; // Сохраняемая полная высота контейнера
    //private CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameGroup"></param>
    /// <param name="data"></param>
    /// <param name="countInRow">Количество ячеек в строке.</param>
    public void Initialize(string nameGroup, JObject data, int countInRow = 12)
    {

        //cellsRectTransform = gameObjectCellsContainer.GetComponent<RectTransform>();

        //if (cellsRectTransform == null)
        //{
        //    throw new MissingComponentException("cellsContainer не имеет компонента RectTransform.");
        //}

        //// Привязываем метод ToggleGroup к событию клика
        //buttonDivider.onClick.RemoveAllListeners();
        //buttonDivider.onClick.AddListener(() => { ToggleGroup(); });

        //// Если группа должна быть свернута по умолчанию, устанавливаем высоту в 0,
        //// иначе сохраняем текущую высоту.
        //if (!expanded)
        //{
        //    // Установка начальной высоты в 0, но нужно сохранить полную высоту
        //    // Для корректного расчета полной высоты, сначала активируем объект
        //    gameObjectCellsContainer.SetActive(true);
        //    // Принудительная перестройка макета для получения корректной высоты
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform);
        //    expandedHeight = cellsRectTransform.rect.height;
        //    cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        //    gameObjectCellsContainer.SetActive(false);
        //}
        //else
        //{
        //    // Принудительная перестройка макета для получения корректной высоты
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform);
        //    expandedHeight = cellsRectTransform.rect.height;
        //}

        _ = Task.Run(AnimateHeight);
        //UpdateDividerVisual(Expanded);
    }


    /// <summary>
    /// Вызывается при уничтожении объекта для отмены всех активных задач.
    /// </summary>
    private void OnDestroy()
    {
        if (!destroying)
        {
            // позже удалить
        }
        destroying = true;
        //cancellationTokenSource?.Cancel();
        //cancellationTokenSource?.Dispose();
    }

    // --- Логика Группировки ---

    /// <summary>
    /// Переключает состояние группы и запускает анимацию.
    /// </summary>
    public void ToggleGroup()
    {
        expanded = !expanded;

        //if (isExpanded)
        //{
        //    // Разворачивание
        //    // Сначала активируем контейнер, чтобы он участвовал в макете, но с высотой 0
        //    cellsContainer.SetActive(true);
        //    await AnimateHeightAsync(0, expandedHeight, token);
        //}
        //else
        //{
        //    // Сворачивание
        //    await AnimateHeightAsync(expandedHeight, 0, token);
        //    // После завершения анимации деактивируем контейнер
        //    cellsContainer.SetActive(false);
        //}

        //UpdateDividerVisual(isExpanded);
    }


    private void AnimateHeight()
    {
        //while (true)
        //{
        //    if (destroying) {
        //        return;
        //    }
        //    // Вычисляем смещение
        //    float delta = direction * animationSpeed * Time.deltaTime;
        //    currentHeight += delta;

        //    // Ограничиваем значение конечной точкой, чтобы избежать перескока
        //    if (direction > 0 && currentHeight > endHeight)
        //    {
        //        currentHeight = endHeight;
        //    }
        //    else if (direction < 0 && currentHeight < endHeight)
        //    {
        //        currentHeight = endHeight;
        //    }

        //    SetHeight(currentHeight);


        //    // Асинхронная задержка до следующего кадра (аналог yield return null в корутинах)
        //    await Task.Yield();
        //}
        ////SetHeight(endHeight);
    }
    private void SetHeight(float h)
    {
        //cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        //rectTransformGroupDivider.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h + 45.36f);

        //// Принудительно перестраиваем родительский макет после каждого шага
        //// Это необходимо, чтобы ScrollView и Vertical Layout Group реагировали на изменение высоты.
        //LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Обновляет визуальное представление разделителя (например, меняет иконку-стрелку).
    /// </summary>
    /// <param name="expanded">Текущее состояние: true - развернута, false - свернута.</param>
    //private void UpdateDividerVisual(bool expanded){
    // Здесь можно добавить логику для изменения иконки-стрелки на кнопке
    // Например: arrow.transform.localRotation = expanded ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
    //}
}
