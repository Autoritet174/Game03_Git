using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class EventHelper
{
    /// <summary>
    /// Метод для навешивания событий наведения и ухода курсора.
    /// </summary>
    public static void AddHoverEvents(this GameObject gameObject, Func<UniTask> onPointerEnter, Func<UniTask> onPointerExit)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        if (!gameObject.TryGetComponent(out ButtonHoverHandler handler))
        {
            handler = gameObject.AddComponent<ButtonHoverHandler>();
        }

        handler.SetupHoverEvents(onPointerEnter, onPointerExit);
    }

    /// <summary>
    /// Метод для навешивания события клика на GameObject.
    /// </summary>
    public static void AddClickEvent(this GameObject gameObject, Func<UniTask> onClick, bool useButtonComponent = true)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        if (useButtonComponent)
        {
            if (!gameObject.TryGetComponent(out Button button))
            {
                button = gameObject.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke().Forget());
        }
        else
        {
            if (!gameObject.TryGetComponent(out ButtonClickHandler clickHandler))
            {
                clickHandler = gameObject.AddComponent<ButtonClickHandler>();
            }

            clickHandler.SetupClickEvent(onClick);
        }
    }

    /// <summary>
    /// Метод для навешивания события клика на UI элемент с поддержкой параметра.
    /// </summary>
    public static void AddClickEvent<T>(this GameObject gameObject, Func<T, UniTask> onClick, T parameter)
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
        button.onClick.AddListener(() => onClick?.Invoke(parameter).Forget());
    }

    /// <summary>
    /// Метод для навешивания асинхронного события клика на UI элемент с поддержкой параметра.
    /// </summary>
    public static void AddClickEvent<T>(this GameObject gameObject, Func<T, UniTask> asyncOnClick, T parameter, bool handleExceptions = true)
    {
        if (gameObject == null)
        {
            Debug.LogError("gameObject is null!");
            return;
        }

        async UniTaskVoid ExecuteWithExceptionHandling()
        {
            try
            {
                if (asyncOnClick != null)
                {
                    await asyncOnClick.Invoke(parameter);
                }
            }
            catch (Exception ex)
            {
                if (handleExceptions)
                {
                    Debug.LogError($"Error in async click handler: {ex.Message}");
                }
                else
                {
                    throw;
                }
            }
        }

        if (!gameObject.TryGetComponent(out Button button))
        {
            button = gameObject.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ExecuteWithExceptionHandling().Forget());
    }

    // УДАЛИТЬ этот метод - он некорректен
    // public static void AddClickEvent(this GameObject gameObject, UniTask asyncOnClick, ...)
    // UniTask нельзя передавать как делегат
}

// Класс-обработчик, который будет добавляться к кнопкам
internal class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Func<UniTask> onEnter;
    private Func<UniTask> onExit;

    public void SetupHoverEvents(Func<UniTask> enterAction, Func<UniTask> exitAction)
    {
        onEnter = enterAction;
        onExit = exitAction;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke().Forget();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke().Forget();
    }
}

// Класс-обработчик для кликов (используется как альтернатива Button)
internal class ButtonClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Func<UniTask> onClick;

    public void SetupClickEvent(Func<UniTask> clickAction)
    {
        onClick = clickAction;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke().Forget();
    }
}
