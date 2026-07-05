using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class EventHelper
{
   
    /// <summary>
    /// Метод для навешивания событий наведения и ухода курсора с асинхронными делегатами.
    /// </summary>
    public static void SetHoverEvents(this GameObject gameObject, Func<UniTask> onPointerEnter, Func<UniTask> onPointerExit)
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
    /// Метод для навешивания событий наведения и ухода курсора с синхронными делегатами.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="onPointerEnter"></param>
    /// <param name="onPointerExit"></param>
    public static void SetHoverEvents(this GameObject gameObject, Action onPointerEnter, Action onPointerExit)
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
    /// Назначает событие клика на кнопку, удаляя все предыдущие слушатели. Если компонент Button отсутствует, выбрасывается исключение.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void SetClickEvent(this GameObject gameObject, Action action)
    {
        if (!gameObject.TryGetComponent(out Button button))
        {
            throw new InvalidOperationException($"GameObject '{gameObject.name}' does not have a Button component");
        }
        Button.ButtonClickedEvent onClick = button.onClick;
        onClick.RemoveAllListeners();
        onClick.AddListener(() => action());
    }


    /// <summary>
    /// Метод для навешивания события клика на GameObject или его Button компонент. Удаляет все другие Listener.
    /// </summary>
    public static void SetClickEvent(this GameObject gameObject, Func<UniTask> onClick, bool useButtonComponent)
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
            if (!gameObject.TryGetComponent(out ButtonClickHandlerCustom clickHandler))
            {
                clickHandler = gameObject.AddComponent<ButtonClickHandlerCustom>();
            }

            clickHandler.SetupClickEvent(onClick);
        }
    }

    /// <summary>
    /// Метод для навешивания события клика на UI элемент с поддержкой параметра.
    /// </summary>
    public static void SetClickEvent<T>(this GameObject gameObject, Func<T, UniTask> onClick, T parameter)
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
    public static void SetClickEvent<T>(this GameObject gameObject, Func<T, UniTask> asyncOnClick, T parameter, bool handleExceptions = true)
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

}

// Класс-обработчик, который будет добавляться к кнопкам
internal class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Func<UniTask> onEnterAsync;
    private Func<UniTask> onExitAsync;
    private Action onEnterSync;
    private Action onExitSync;

    public void SetupHoverEvents(Func<UniTask> enterAction, Func<UniTask> exitAction)
    {
        onEnterAsync = enterAction;
        onExitAsync = exitAction;
        onEnterSync = null;
        onExitSync = null;
    }

    public void SetupHoverEvents(Action enterAction, Action exitAction)
    {
        onEnterSync = enterAction;
        onExitSync = exitAction;
        onEnterAsync = null;
        onExitAsync = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnterSync != null)
        {
            onEnterSync();
            return;
        }

        onEnterAsync?.Invoke().Forget();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExitSync != null)
        {
            onExitSync();
            return;
        }

        onExitAsync?.Invoke().Forget();
    }
}

// Класс-обработчик для кликов (используется как альтернатива Button)
internal class ButtonClickHandlerCustom : MonoBehaviour, IPointerClickHandler
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
