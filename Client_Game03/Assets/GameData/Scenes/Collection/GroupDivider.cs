using Assets.GameData.Scripts;
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
    public async Task Init(GameObject gameObjectGroupDivider, List<JToken> listHeroes)
    {
        //DividerButton
        GameObject gameObjectDividerButton = GameObjectFinder.FindByName("DividerButton", gameObjectGroupDivider.transform);
        TextMeshProUGUI textMeshProUGUIDividerButton = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", gameObjectDividerButton.transform);
        Transform transformCellsContainer = GameObjectFinder.FindByName<Transform>("CellsContainer", gameObjectGroupDivider.transform);
        if (group_name == null)
        {
            textMeshProUGUIDividerButton.text = "---No Group---";
            textMeshProUGUIDividerButton.fontStyle = FontStyles.Italic;
        }
        else
        {
            textMeshProUGUIDividerButton.text = group_name;
        }

        // Сортировка
        listHeroes.Sort((a, b) =>
        {
            var a_hero_id = Guid.Parse(a["hero_id"].ToString());
            var b_hero_id = Guid.Parse(b["hero_id"].ToString());
            HeroBaseEntity a_hero = G.Game.GlobalFunctions.GetHeroById(a_hero_id);
            HeroBaseEntity b_hero = G.Game.GlobalFunctions.GetHeroById(b_hero_id);
            if (a_hero == null || b_hero==null)
            {
                return 0;
            }

            int sort1 = b_hero.Rarity.CompareTo(a_hero.Rarity);
            if (sort1 != 0)
            {
                return sort1;
            }

            int sort2 = a_hero.Name.CompareTo(b_hero.Name);
            return sort2;
        });


        Sprite raritySelectedSprite = await Addressables.LoadAssetAsync<Sprite>($"raritySelected").Task;

        for (int i = 0; i < listHeroes.Count; i++)
        {
            JToken j = listHeroes[i];
            var hero_id = Guid.Parse(j["hero_id"].ToString());
            General.GameEntities.HeroBaseEntity hero = G.Game.GlobalFunctions.GetHeroById(hero_id);
            if (hero == null)
            {
                continue;
            }

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
                    textMeshPro.text = hero.Name.ToUpper1Char();
                    textMeshPro.fontSize = 12;
                }

                imageRarity.sprite = await Addressables.LoadAssetAsync<Sprite>($"rarity{(int)hero.Rarity}").Task;
                imageRarity.preserveAspect = true; // Сохраняет пропорции изображения
                imageRarity.type = Image.Type.Simple; // Режим без растягивания;

                string addressableKey = $"hero-image-{hero.Name.ToLower()}_face";
                imageHero.sprite = await Addressables.LoadAssetAsync<Sprite>(addressableKey).Task;
                imageHero.preserveAspect = true; // Сохраняет пропорции изображения
                imageHero.type = Image.Type.Simple; // Режим без растягивания;

                ImageHeroHandler clickHandler = _prefabIconHero.AddComponent<ImageHeroHandler>();
                clickHandler.Initialize(hero, imageRarity.sprite, raritySelectedSprite, HeroView, imageRarity);
            }
        }
    }

    private async Task HeroView(HeroBaseEntity entity)
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
