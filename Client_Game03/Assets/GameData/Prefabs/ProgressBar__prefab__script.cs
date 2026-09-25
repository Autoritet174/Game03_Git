using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Отображает полосу значения с подписями и настраиваемыми цветами.</summary>
public class ProgressBar__prefab__script : MonoBehaviour, IPrefab
{
    /// <summary>Определяет показатель, отображаемый полосой статистики.</summary>
    public enum EDisplayMode
    {
        damageDone, healingDone, damageRecieved, healingRecieved, healthBar
    }

    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    private string textLeft = "";
    private string textRight = "";

    private const float HEALTH_IMAGE_COLOR_BAR_RIGHT = 0.5f;
    private const float POSY_SHIFT = -1.5f;

    private float textLeft_left = 3;
    private float textRight_right = 3;

    private RectTransform healthImagePercent__RectTransform;
    private RectTransform healthImageColorBar__RectTransform;

    //public GameObject this__GameObject { get; set; }
    public RectTransform this__RectTransform { get; private set; }

    private RectTransform textLeft__RectTransform;
    private TextMeshProUGUI textLeft__TextMeshProUGUI;
    private RectTransform textRight__RectTransform;
    private TextMeshProUGUI textRight__TextMeshProUGUI;
    private Image healthImageColorBar__Image;
    private float healthimagecolorbar_right = HEALTH_IMAGE_COLOR_BAR_RIGHT;

    public float value { get; set; } = 0f;
    public float valueMax { get; set; } = 1f;

    public void Initialize()
    {
        //if (this__GameObject == null)
        //{
        //    this__GameObject = gameObject;
        //}

        this__RectTransform = gameObject.GetComponent<RectTransform>();
        healthImagePercent__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImagePercent", gameObject);
        healthImageColorBar__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageColorBar", gameObject);

        textLeft__RectTransform = GameObjectFinder.FindByName<RectTransform>("TextLeft", gameObject);
        textRight__RectTransform = GameObjectFinder.FindByName<RectTransform>("TextRight", gameObject);
        textLeft__TextMeshProUGUI = textLeft__RectTransform.GetComponent<TextMeshProUGUI>();
        textRight__TextMeshProUGUI = textRight__RectTransform.GetComponent<TextMeshProUGUI>();

        healthImageColorBar__Image = healthImageColorBar__RectTransform.gameObject.GetComponent<Image>();

        SetTextRight("");
        SetTextLeft("");

        initialized = true;
        OnResized(G.GetCoefHeight());
        Refresh();
    }

    #region Настройка подписей и цветов

    public void SetTextLeft(string text)
    {
        textLeft = text;
        textLeft__TextMeshProUGUI.SetText(text);
    }

    public void SetTextRight(string text)
    {
        textRight = text;
        textRight__TextMeshProUGUI.SetText(text);
    }

    public void SetColorTextLeft(Color color)
    {
        textLeft__TextMeshProUGUI.color = color;
    }

    public void SetColorTextRight(Color color)
    {
        textRight__TextMeshProUGUI.color = color;
    }

    public void SetColorBar(Color color)
    {
        healthImageColorBar__Image.color = color;
    }

    public void SetTextLeftOffsetLeft(float value)
    {
        textLeft_left = value;
    }

    public void SetTextRightOffsetRight(float value)
    {
        textRight_right = value;
    }

    #endregion Настройка подписей и цветов

    #region Обновление полосы

    /// <summary>Обновляет визуальное состояние прогресс бара на панели</summary>
    public void Refresh()
    {
        float progressBarWidth = this__RectTransform.rect.width;
        float width = valueMax > 0 ? progressBarWidth * value / valueMax : 0f;

        //if (type != "")
        //{
        //    Debug.Log($"{value}/{valueMax} [{textRight}]");
        //}

        if (width < 0)
        {
            width = 0;
        }
        else if (width > progressBarWidth)
        {
            width = progressBarWidth;
        }
        healthImagePercent__RectTransform.sizeDelta = new(width, 0);

        float widthColorBar = width - healthimagecolorbar_right;
        if (widthColorBar < 0)
        {
            widthColorBar = 0;
        }
        else if (widthColorBar > progressBarWidth)
        {
            widthColorBar = progressBarWidth;
        }
        healthImageColorBar__RectTransform.sizeDelta = new(widthColorBar, 0);
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        //_HealthImagePercent__RectTransform.sizeDelta = new Vector2(this__RectTransform.sizeDelta.x, 0);

        //float width = (_Width - (1f * 2)) * SpawnedHero.HealthPercent;
        //_HealthImagePercent__RectTransform.sizeDelta = new Vector2(width, _Health_Height * coefHeight);
        this__RectTransform.anchoredPosition = new(0, POSY_SHIFT * coefHeight);

        healthimagecolorbar_right = HEALTH_IMAGE_COLOR_BAR_RIGHT * coefHeight;

        textLeft__RectTransform.SetHorizontalOffsets(textLeft_left * coefHeight, 0);
        textRight__RectTransform.SetHorizontalOffsets(0, textRight_right * coefHeight);
    }

    #endregion Обновление полосы

}
