using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;

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
    [Tooltip("Контейнер с ячейками, который будет скрываться/показываться.")]
    [SerializeField]
    private GameObject cellsContainer;

    /// <summary>
    /// Кнопка, при клике на которую происходит сворачивание/разворачивание.
    /// </summary>
    [Tooltip("Кнопка-разделитель, которая запускает сворачивание.")]
    [SerializeField]
    private Button dividerButton;
    [SerializeField]
    private RectTransform rectTransformDividerButton;

    [SerializeField]
    private RectTransform rectTransformGroupDivider;

    /// <summary>
    /// Скорость анимации (единиц в секунду).
    /// </summary>
    [Tooltip("Скорость анимации в единицах высоты в секунду.")]
    [SerializeField]
    private float animationSpeed = 1000f;

    /// <summary>
    /// Текущее состояние группы (true - развернута, false - свернута).
    /// </summary>
    [Tooltip("Начальное состояние группы: true - развернута, false - свернута.")]
    [SerializeField]
    private bool isExpanded = true;

    // --- Приватные поля ---

    private RectTransform cellsRectTransform;
    private float expandedHeight; // Сохраняемая полная высота контейнера
    private CancellationTokenSource cancellationTokenSource;

    // --- Методы Инициализации ---

    /// <summary>
    /// Инициализирует скрипт, получает компоненты и устанавливает начальное состояние.
    /// Должен быть вызван вручную после инстанцирования, если объект создается динамически.
    /// </summary>
    public void Initialize()
    {
        
        // Проверка валидности входных данных
        if (cellsContainer == null)
        {
            throw new MissingComponentException($"cellsContainer не назначен на объекте {gameObject.name}.");
        }
        if (dividerButton == null)
        {
            throw new MissingComponentException($"dividerButton не назначен на объекте {gameObject.name}.");
        }

        cellsRectTransform = cellsContainer.GetComponent<RectTransform>();

        if (cellsRectTransform == null)
        {
            throw new MissingComponentException("cellsContainer не имеет компонента RectTransform.");
        }

        // Привязываем метод ToggleGroup к событию клика
        dividerButton.onClick.RemoveAllListeners();
        dividerButton.onClick.AddListener(() => { ToggleGroupAsync(); });

        // Если группа должна быть свернута по умолчанию, устанавливаем высоту в 0,
        // иначе сохраняем текущую высоту.
        if (!isExpanded)
        {
            // Установка начальной высоты в 0, но нужно сохранить полную высоту
            // Для корректного расчета полной высоты, сначала активируем объект
            cellsContainer.SetActive(true);
            // Принудительная перестройка макета для получения корректной высоты
            LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform);
            expandedHeight = cellsRectTransform.rect.height;
            cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            cellsContainer.SetActive(false);
        }
        else
        {
            // Принудительная перестройка макета для получения корректной высоты
            LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform);
            expandedHeight = cellsRectTransform.rect.height;
        }

        UpdateDividerVisual(isExpanded);
    }

    /// <summary>
    /// Вызывается при загрузке сцены, если объект статический.
    /// </summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Вызывается при уничтожении объекта для отмены всех активных задач.
    /// </summary>
    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    // --- Логика Группировки ---

    /// <summary>
    /// Переключает состояние группы и запускает анимацию.
    /// </summary>
    public async void ToggleGroupAsync()
    {
        // Отменяем предыдущую анимацию, если она еще активна
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        isExpanded = !isExpanded;

        if (isExpanded)
        {
            // Разворачивание
            // Сначала активируем контейнер, чтобы он участвовал в макете, но с высотой 0
            cellsContainer.SetActive(true);
            await AnimateHeightAsync(0, expandedHeight, token);
        }
        else
        {
            // Сворачивание
            await AnimateHeightAsync(expandedHeight, 0, token);
            // После завершения анимации деактивируем контейнер
            cellsContainer.SetActive(false);
        }

        UpdateDividerVisual(isExpanded);
    }

    /// <summary>
    /// Асинхронно анимирует высоту RectTransform от начального до конечного значения.
    /// </summary>
    /// <param name="startHeight">Начальная высота.</param>
    /// <param name="endHeight">Конечная высота.</param>
    /// <param name="token">Токен отмены для прерывания анимации.</param>
    /// <returns>Task, завершающийся по окончании анимации.</returns>
    private async Task AnimateHeightAsync(float startHeight, float endHeight, CancellationToken token)
    {
        float currentHeight = startHeight;
        float direction = Mathf.Sign(endHeight - startHeight);
        float totalDistance = Mathf.Abs(endHeight - startHeight);

        cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);

        while (Mathf.Abs(currentHeight - endHeight) > 0.1f) // 0.1f - небольшой допуск
        {
            if (token.IsCancellationRequested)
            {
                // Завершаем анимацию и устанавливаем высоту, куда двигались, чтобы не было "прыжка"
                cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endHeight);
                return;
            }

            // Вычисляем смещение
            float delta = direction * animationSpeed * Time.deltaTime;
            currentHeight += delta;

            // Ограничиваем значение конечной точкой, чтобы избежать перескока
            if (direction > 0 && currentHeight > endHeight)
            {
                currentHeight = endHeight;
            }
            else if (direction < 0 && currentHeight < endHeight)
            {
                currentHeight = endHeight;
            }

            cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            rectTransformGroupDivider.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight + 45.36f);
            //rectTransformDividerButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight + 45.36f);

            // Принудительно перестраиваем родительский макет после каждого шага
            // Это необходимо, чтобы ScrollView и Vertical Layout Group реагировали на изменение высоты.
            LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform.parent.GetComponent<RectTransform>());

            // Асинхронная задержка до следующего кадра (аналог yield return null в корутинах)
            await Task.Yield();
        }

        cellsRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endHeight);
        // Финальная перестройка макета после завершения
        LayoutRebuilder.ForceRebuildLayoutImmediate(cellsRectTransform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Обновляет визуальное представление разделителя (например, меняет иконку-стрелку).
    /// </summary>
    /// <param name="expanded">Текущее состояние: true - развернута, false - свернута.</param>
    private void UpdateDividerVisual(bool expanded)
    {
        // Здесь можно добавить логику для изменения иконки-стрелки на кнопке
        // Например: arrow.transform.localRotation = expanded ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
    }
}
