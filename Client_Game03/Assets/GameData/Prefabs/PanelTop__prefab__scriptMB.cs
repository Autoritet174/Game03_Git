using Assets.GameData.Scripts;
using System;
using UnityEngine;

/// <summary>Настраивает верхнюю панель сцены и действие кнопки закрытия.</summary>
[RequireComponent(typeof(RectTransform))]
public class PanelTop__prefab__scriptMB : MonoBehaviour, IPrefab
{
    private RectTransform this__RectTransform;
    private RectTransform buttonClose__RectTransform;

    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    public void Initialize()
    {
        this__RectTransform = GetComponent<RectTransform>();
        buttonClose__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose", this__RectTransform.transform);
        initialized = true;
    }

    public void SetActionOnButtonClose(Action action)
    {
        buttonClose__RectTransform.gameObject.SetClickOnButton(action);
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        if (!initialized)
        {
            return;
        }

        width = Screen.width;
        height = G.PANELTOP_HEIGHT * coefHeight;

        this__RectTransform.sizeDelta = new(width, height);
        buttonClose__RectTransform.sizeDelta = new(height, height);
    }

}
