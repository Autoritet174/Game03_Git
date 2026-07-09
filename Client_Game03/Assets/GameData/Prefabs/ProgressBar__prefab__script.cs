using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

public class ProgressBar__prefab__script : MonoBehaviour, IPrefab
{
    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }
    private const float HEALTH_IMAGE_COLOR_BAR_RIGHT = 0.5f;
    private const float POSY_SHIFT = -1.5f;

    private float textLeft_left = 3;
    private float textRight_right = 3;

    private RectTransform _HealthImagePercent__RectTransform;
    private RectTransform _HealthImageColorBar__RectTransform;

    public GameObject this__GameObject { get; set; }
    public RectTransform this__RectTransform { get; private set; }
    private RectTransform _TextLeft__RectTransform;
    private TextMeshProUGUI _TextLeft__TextMeshProUGUI;
    private RectTransform _TextRight__RectTransform;
    private TextMeshProUGUI _TextRight__TextMeshProUGUI;
    private float healthimagecolorbar_right = HEALTH_IMAGE_COLOR_BAR_RIGHT;

    public float value { get; set; }
    public float valueMax { get; set; }
    public void Initialize()
    {
        if (this__GameObject == null)
        {
            this__GameObject = gameObject;
        }

        this__RectTransform = this__GameObject.GetComponent<RectTransform>();
        _HealthImagePercent__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImagePercent", this__GameObject);
        _HealthImageColorBar__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageColorBar", this__GameObject);

        _TextLeft__RectTransform = GameObjectFinder.FindByName<RectTransform>("TextLeft", this__GameObject);
        _TextRight__RectTransform = GameObjectFinder.FindByName<RectTransform>("TextRight", this__GameObject);
        _TextLeft__TextMeshProUGUI = _TextLeft__RectTransform.GetComponent<TextMeshProUGUI>();
        _TextRight__TextMeshProUGUI = _TextRight__RectTransform.GetComponent<TextMeshProUGUI>();

        SetTextRight("");
        SetTextLeft("");

        initialized = true;
        OnResized(G.GetCoefHeight());
    }

    public void SetTextLeft(string text)
    {
        _TextLeft__TextMeshProUGUI.SetText(text);
    }

    public void SetTextRight(string text)
    {
        _TextRight__TextMeshProUGUI.SetText(text);
    }

    public void SetTextLeftOffsetLeft(float value)
    {
        textLeft_left = value;
    }

    public void SetTextRightOffsetRight(float value)
    {
        textRight_right = value;
    }

    public void Refresh()
    {
        float width = this__RectTransform.sizeDelta.x * value / valueMax;
        if (width < 0)
        {
            width = 0;
        }
        else if (width > this__RectTransform.sizeDelta.x)
        {
            width = this__RectTransform.sizeDelta.x;
        }
        _HealthImagePercent__RectTransform.sizeDelta = new Vector2(width, 0);

        float widthColorBar = width - healthimagecolorbar_right;
        if (widthColorBar < 0)
        {
            widthColorBar = 0;
        }
        else if (widthColorBar > this__RectTransform.sizeDelta.x)
        {
            widthColorBar = this__RectTransform.sizeDelta.x;
        }
        _HealthImageColorBar__RectTransform.sizeDelta = new Vector2(widthColorBar, 0);
    }


    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        //_HealthImagePercent__RectTransform.sizeDelta = new Vector2(this__RectTransform.sizeDelta.x, 0);

        //float width = (_Width - (1f * 2)) * SpawnedHero.HealthPercent;
        //_HealthImagePercent__RectTransform.sizeDelta = new Vector2(width, _Health_Height * coefHeight);
        this__RectTransform.anchoredPosition = new Vector2(0, POSY_SHIFT * coefHeight);

        healthimagecolorbar_right = HEALTH_IMAGE_COLOR_BAR_RIGHT * coefHeight;

        _TextLeft__RectTransform.SetHorizontalOffsets(textLeft_left * coefHeight, 0);
        _TextRight__RectTransform.SetHorizontalOffsets(0, textRight_right * coefHeight);
    }

}
