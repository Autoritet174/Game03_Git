using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonHoverHelper
{
    /// <summary>
    /// Метод для навешивания событий наведения и ухода курсора.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="onPointerEnter"></param>
    /// <param name="onPointerExit"></param>
    public static void AddHoverEvents(this Button button,
        Action onPointerEnter,
        Action onPointerExit)
    {
        if (button == null)
        {
            Debug.LogError("Button is null!");
            return;
        }

        // Получаем или добавляем компонент обработчика
        if (!button.TryGetComponent(out ButtonHoverHandler handler))
        {
            handler = button.gameObject.AddComponent<ButtonHoverHandler>();
        }

        // Настраиваем события
        handler.SetupHoverEvents(onPointerEnter, onPointerExit);
    }

    // Перегруженный метод для использования UnityEvent
    public static void AddHoverEvents(Button button,
        UnityEngine.Events.UnityEvent onPointerEnter,
        UnityEngine.Events.UnityEvent onPointerExit)
    {
        if (button == null)
        {
            Debug.LogError("Button is null!");
            return;
        }

        if (!button.TryGetComponent(out ButtonHoverHandler handler))
        {
            handler = button.gameObject.AddComponent<ButtonHoverHandler>();
        }

        handler.SetupUnityEvents(onPointerEnter, onPointerExit);
    }
}

// Класс-обработчик, который будет добавляться к кнопкам
public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
