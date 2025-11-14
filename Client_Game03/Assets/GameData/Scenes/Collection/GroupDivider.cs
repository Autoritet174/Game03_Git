using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;

/// <summary>
/// Управляет сворачиванием/разворачиванием группы UI-элементов (ячеек)
/// с асинхронной анимацией высоты.
/// </summary>
public class GroupDivider : MonoBehaviour
{
    // --- Публичные поля, которые настраиваются в Инспекторе ---

    /// <summary>
    /// Контейнер, содержащий все ячейки инвентаря для этой группы.
    /// На этом объекте должен быть RectTransform.
    /// </summary>
    [SerializeField]
    [Header("Контейнер для ячеек")]
    private GameObject gameObjectCellsContainer;

    /// <summary>
    /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
    /// </summary>
    [SerializeField]
    [Header("Кнопка-разделитель")]
    private Button buttonDivider;
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

        Task.Run(AnimateHeightAsync);
        //UpdateDividerVisual(Expanded);
    }


    /// <summary>
    /// Вызывается при уничтожении объекта для отмены всех активных задач.
    /// </summary>
    private void OnDestroy()
    {
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

    
    private async void AnimateHeightAsync()
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
    void SetHeight(float h)
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
