using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PanelDamage__script : IPrefab
{
    
    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    private RectTransform PanelDamage__RectTransform;
    private RectTransform PanelProgressBars__RectTransform;
    private RectTransform ButtonDamage__RectTransform;
    private RectTransform ButtonHeal__RectTransform;
    private RectTransform ButtonTank__RectTransform;

    public List<Bar> bars { get; private set; } = new();

    public class Bar
    {
        public Guid heroId { get; }
        public ProgressBar__prefab__script bar { get; }
        public Bar(Guid heroId, ProgressBar__prefab__script bar)
        {
            this.heroId = heroId;
            this.bar = bar;
        }

    }

    public void Initialize()
    {
        PanelDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelDamage");

        PanelProgressBars__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelProgressBars", PanelDamage__RectTransform);
        ButtonDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamage", PanelDamage__RectTransform);
        ButtonHeal__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHeal", PanelDamage__RectTransform);
        ButtonTank__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTank", PanelDamage__RectTransform);

        OnResized(G.GetCoefHeight());
    }

    public void AddProgressBar(Guid heroId, string textLeft, string textRight, string type = "", Color? colorLeft = null, Color? colorRight = null)
    {
        GameObject gameObject = AddressableCache.ProgressBar.SafeInstant(PanelProgressBars__RectTransform.transform);
        gameObject.name = $"ProgressBar__prefab ({textRight}) [{heroId}]";
        ProgressBar__prefab__script bar = gameObject.GetComponent<ProgressBar__prefab__script>();
        bars.Add(new Bar(heroId, bar));
        bar.Initialize();
        bar.this__RectTransform.anchorMin = new Vector2(0, 1);
        bar.this__RectTransform.anchorMax = new Vector2(0, 1);
        bar.this__RectTransform.pivot = new Vector2(0, 1);
        //bar.value = UnityEngine.Random.Range(1, 100);
        bar.type = type;
        if (colorLeft != null)
        {
            bar.SetColorTextLeft(colorLeft.Value);
        }
        if (colorRight != null)
        {
            bar.SetColorTextRight(colorRight.Value);
        }
        //ProgressBarsSort();


        bar.SetTextLeft(textLeft);
        bar.SetTextRight(textRight);
        OnResized(G.GetCoefHeight());
    }

    public void ProgressBarsSort()
    {
        float valueMax = bars.Count > 0 ? bars.Max(a => a.bar.value) : 1;
        if (valueMax < 1)
        {
            valueMax = 1;
        }
        bars.ForEach(a => a.bar.valueMax = valueMax);

        bars.Sort((b, a) => a.bar.value.CompareTo(b.bar.value));

        OnResized(G.GetCoefHeight());
    }

    public void Refresh()
    {
        bars.ForEach(a => a.bar.Refresh());
    }

    public void ProgressBarsSortAndRefresh()
    {
        ProgressBarsSort();
        Refresh();
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        PanelDamage__RectTransform.sizeDelta = new Vector2(300 * coefHeight, 762.5f * coefHeight);// 10 + 10 + 0.75*30 + 30*24
        PanelDamage__RectTransform.anchoredPosition = new Vector2(20 * coefHeight, 0);

        float PanelProgressBars__offsets = 10 * coefHeight;
        PanelProgressBars__RectTransform.SetOffsets(PanelProgressBars__offsets, PanelProgressBars__offsets, PanelProgressBars__offsets, PanelProgressBars__offsets);

        float buttonsSize = 70 * coefHeight;
        float buttonPos = 10 * coefHeight;
        ButtonDamage__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonHeal__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonTank__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonDamage__RectTransform.anchoredPosition = new Vector2(buttonPos, -buttonPos);
        ButtonHeal__RectTransform.anchoredPosition = new Vector2((buttonPos * 2) + buttonsSize, -buttonPos);
        ButtonTank__RectTransform.anchoredPosition = new Vector2((buttonPos * 3) + (buttonsSize * 2), -buttonPos);

        float barHeight = 30 * coefHeight;
        float barHeightShift = barHeight * 2f;
        int i = 0;
        foreach (Bar v in bars)
        {
            RectTransform r = v.bar.this__RectTransform;

            r.anchoredPosition = new Vector2(0, (-barHeight * (i + 1)) - barHeightShift);
            r.sizeDelta = new Vector2(PanelProgressBars__RectTransform.rect.width, barHeight);
            i++;
        }
        Refresh();
    }
}
