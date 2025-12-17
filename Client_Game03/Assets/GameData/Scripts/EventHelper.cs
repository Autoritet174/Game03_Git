using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class EventHelper
{
    /// <summary>
    /// Метод для навешивания событий наведения и ухода курсора.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="onPointerEnter"></param>
    /// <param name="onPointerExit"></param>
    public static void AddHoverEvents(this GameObject gameObject, Action onPointerEnter, Action onPointerExit)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        // Получаем или добавляем компонент обработчика
        if (!gameObject.TryGetComponent(out ButtonHoverHandler handler))
        {
            handler = gameObject.AddComponent<ButtonHoverHandler>();
        }

        // Настраиваем события
        handler.SetupHoverEvents(onPointerEnter, onPointerExit);
    }

    /// <summary>
    /// Метод для навешивания события клика на GameObject.
    /// </summary>
    /// <param name="gameObject">Объект, на который добавляется событие клика</param>
    /// <param name="onClick">Действие, выполняемое при клике</param>
    /// <param name="useButtonComponent">Использовать ли компонент Button для обработки клика (рекомендуется для UI)</param>
    public static void AddClickEvent(this GameObject gameObject, Action onClick, bool useButtonComponent = true)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        if (useButtonComponent)
        {
            // Используем компонент Button для стандартной обработки UI кликов
            if (!gameObject.TryGetComponent(out Button button))
            {
                button = gameObject.AddComponent<Button>();
            }

            // Удаляем старые слушатели и добавляем новый
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
        else
        {
            // Альтернативный способ через IPointerClickHandler
            if (!gameObject.TryGetComponent(out ButtonClickHandler clickHandler))
            {
                clickHandler = gameObject.AddComponent<ButtonClickHandler>();
            }

            clickHandler.SetupClickEvent(onClick);
        }
    }

    /// <summary>
    /// Метод для навешивания асинхронного события клика на GameObject.
    /// </summary>
    /// <param name="gameObject">Объект, на который добавляется событие клика</param>
    /// <param name="asyncOnClick">Асинхронное действие, выполняемое при клике</param>
    /// <param name="useButtonComponent">Использовать ли компонент Button для обработки клика</param>
    /// <param name="handleExceptions">Обрабатывать ли исключения внутри метода</param>
    public static void AddClickEvent(this GameObject gameObject, Func<Task> asyncOnClick, bool useButtonComponent = true, bool handleExceptions = true)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        // Создаем обертку для асинхронного метода
        Action syncWrapper = async () =>
        {
            try
            {
                if (asyncOnClick != null)
                    await asyncOnClick.Invoke();
            }
            catch (Exception ex)
            {
                if (handleExceptions)
                    Debug.LogError($"Error in async click handler: {ex.Message}");
                else
                    throw;
            }
        };

        // Используем существующий синхронный метод
        AddClickEvent(gameObject, syncWrapper, useButtonComponent);
    }

    /// <summary>
    /// Метод для навешивания события клика на UI элемент с поддержкой параметра.
    /// </summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <param name="gameObject">Объект, на который добавляется событие клика</param>
    /// <param name="onClick">Действие, выполняемое при клике с параметром</param>
    /// <param name="parameter">Параметр, передаваемый в обработчик клика</param>
    public static void AddClickEvent<T>(this GameObject gameObject, Action<T> onClick, T parameter)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        if (!gameObject.TryGetComponent(out Button button))
        {
            button = gameObject.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(parameter));
    }

    /// <summary>
    /// Метод для навешивания асинхронного события клика на UI элемент с поддержкой параметра.
    /// </summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <param name="gameObject">Объект, на который добавляется событие клика</param>
    /// <param name="asyncOnClick">Асинхронное действие, выполняемое при клике с параметром</param>
    /// <param name="parameter">Параметр, передаваемый в обработчик клика</param>
    /// <param name="handleExceptions">Обрабатывать ли исключения внутри метода</param>
    public static void AddClickEvent<T>(this GameObject gameObject, Func<T, Task> asyncOnClick, T parameter, bool handleExceptions = true)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        // Создаем обертку для асинхронного метода
        Action syncWrapper = async () =>
        {
            try
            {
                if (asyncOnClick != null)
                    await asyncOnClick.Invoke(parameter);
            }
            catch (Exception ex)
            {
                if (handleExceptions)
                    Debug.LogError($"Error in async click handler: {ex.Message}");
                else
                    throw;
            }
        };

        if (!gameObject.TryGetComponent(out Button button))
        {
            button = gameObject.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => syncWrapper?.Invoke());
    }

    // Перегруженный метод для использования UnityEvent
    //public static void AddHoverEvents(Button button, UnityEngine.Events.UnityEvent onPointerEnter, UnityEngine.Events.UnityEvent onPointerExit)
    //{
    //    if (button == null)
    //    {
    //        Debug.LogError("Button is null!");
    //        return;
    //    }
    //
    //    if (!button.TryGetComponent(out ButtonHoverHandler handler))
    //    {
    //        handler = button.gameObject.AddComponent<ButtonHoverHandler>();
    //    }
    //
    //    handler.SetupUnityEvents(onPointerEnter, onPointerExit);
    //}
}

// Класс-обработчик, который будет добавляться к кнопкам
internal class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Action onEnter;
    private Action onExit;

    private UnityEngine.Events.UnityEvent unityOnEnter;
    private UnityEngine.Events.UnityEvent unityOnExit;

    // Настройка событий через Action
    public void SetupHoverEvents(Action enterAction, Action exitAction)
    {
        onEnter = enterAction;
        onExit = exitAction;
    }

    // Настройка событий через UnityEvent
    public void SetupUnityEvents(UnityEngine.Events.UnityEvent enterEvent, UnityEngine.Events.UnityEvent exitEvent)
    {
        unityOnEnter = enterEvent;
        unityOnExit = exitEvent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        unityOnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
        unityOnExit?.Invoke();
    }
}

// Класс-обработчик для кликов (используется как альтернатива Button)
internal class ButtonClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Action onClick;

    public void SetupClickEvent(Action clickAction)
    {
        onClick = clickAction;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}
