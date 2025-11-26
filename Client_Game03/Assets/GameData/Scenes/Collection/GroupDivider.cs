using Assets.GameData.Scripts;
using Game03Client.PlayerCollection;
using General;
using General.GameEntities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using G = Assets.GameData.Scripts.G;

/// <summary>
/// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
/// с асинхронной анимацией высоты.
/// </summary>
public class GroupDivider : MonoBehaviour
{
    private readonly string group_name;

    public GroupDivider(string group_name)
    {
        this.group_name = group_name;
    }
    public async Task Init(GameObject gameObjectGroupDivider, IEnumerable<General.GameEntities.CollectionHero> listHeroes)
    {
        //DividerButton
        GameObject gameObjectDividerButton = GameObjectFinder.FindByName("DividerButton", gameObjectGroupDivider.transform);
        TextMeshProUGUI textMeshProUGUIDividerButton = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", gameObjectDividerButton.transform);
        Transform transformCellsContainer = GameObjectFinder.FindByName<Transform>("CellsContainer", gameObjectGroupDivider.transform);
        if (group_name.IsEmpty())
        {
            textMeshProUGUIDividerButton.text = "---No Group---";
            textMeshProUGUIDividerButton.fontStyle = FontStyles.Italic;
        }
        else
        {
            textMeshProUGUIDividerButton.text = group_name;
        }


        Sprite raritySelectedSprite = await Addressables.LoadAssetAsync<Sprite>($"raritySelected").Task;

        foreach (General.GameEntities.CollectionHero groupHero in listHeroes)
        {
            var heroBase = groupHero.HeroBase;
            GameObject gameObject = await Addressables.LoadAssetAsync<GameObject>("IconHero").Task;
            GameObject _prefabIconHero = gameObject.SafeInstant();
            _prefabIconHero.transform.SetParent(transformCellsContainer);

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
                clickHandler.Initialize(heroBase, imageRarity.sprite, raritySelectedSprite, HeroView, imageRarity);
            }
        }
    }

    private async Task HeroView(HeroBase entity)
    {
        Debug.Log(entity.Name);
        //throw new NotImplementedException();
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
        if (!destroying) {
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
