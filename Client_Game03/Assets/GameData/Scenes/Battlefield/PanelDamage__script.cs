using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using General.DTO.Battlefield;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelDamage__script : IPrefab
{
    public enum Team { MyHeroes, EnemyHeroes }
    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    private RectTransform PanelDamage__RectTransform;
    private RectTransform PanelProgressBars__RectTransform;
    //private RectTransform PanelProgressBarsViewport__RectTransform;
    private RectTransform PanelProgressBarsContent__RectTransform;

    private GameObject ScrollbarVertical__GameObject;
    private RectTransform ScrollbarVertical__RectTransform;

    private RectTransform ButtonDamage__RectTransform;
    private RectTransform ButtonHeal__RectTransform;
    private RectTransform ButtonTank__RectTransform;
    private Image ButtonDamage__Image;
    private Image ButtonHeal__Image;
    private Image ButtonTank__Image;

    //private readonly List<Bar> bars = new();

    /// <summary>
    /// Прогресс бары, которые отображаются в панели.
    /// </summary>
    private readonly List<ProgressBar__prefab__script> bars_List = new();


    private ProgressBar__prefab__script.DisplayMode displayMode = ProgressBar__prefab__script.DisplayMode.DamageDone;

    //private class Bar
    //{
    //    //public ProgressBar__prefab__script bar { get; }
    //    public Bar(//Guid heroId, ProgressBar__prefab__script bar
    //        )
    //    {
    //        //this.heroId = heroId;
    //        //this.bar = bar;
    //    }
    //}
    public BattlefieldSceneInitializator battlefieldSceneInitializator { get; set; }
    public void Initialize()
    {
        PanelDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelDamage");

        PanelProgressBars__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelProgressBars", PanelDamage__RectTransform);
        ButtonDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamage", PanelDamage__RectTransform);
        ButtonHeal__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHeal", PanelDamage__RectTransform);
        ButtonTank__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTank", PanelDamage__RectTransform);
        ButtonDamage__Image = ButtonDamage__RectTransform.GetComponent<Image>();
        ButtonHeal__Image = ButtonHeal__RectTransform.GetComponent<Image>();
        ButtonTank__Image = ButtonTank__RectTransform.GetComponent<Image>();
        ButtonDamage__RectTransform.gameObject.SetClickOnButton(ButtonDamageOnClick);
        ButtonHeal__RectTransform.gameObject.SetClickOnButton(ButtonHealOnClick);
        ButtonTank__RectTransform.gameObject.SetClickOnButton(ButtonTankOnClick);

        RectTransform PanelProgressBarsViewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", PanelProgressBars__RectTransform);
        PanelProgressBarsContent__RectTransform = GameObjectFinder.FindByName<RectTransform>("Content", PanelProgressBarsViewport__RectTransform);

        ScrollbarVertical__GameObject = GameObjectFinder.FindByName("ScrollbarVertical", PanelProgressBars__RectTransform);
        ScrollbarVertical__RectTransform = ScrollbarVertical__GameObject.GetComponent<RectTransform>();

        OnResized(G.GetCoefHeight());
    }

    public void AddProgressBar(Guid spawnedId, string textLeft, string textRight, Team type, Color? colorLeft = null, Color? colorRight = null)
    {
        GameObject gameObject = AddressablePrefabProvider.ProgressBar.SafeInstant(PanelProgressBarsContent__RectTransform.transform);
        if (gameObject == null)
        {
            return;
        }

        gameObject.name = $"ProgressBar__prefab {bars_List.Count + 1}";
        ProgressBar__prefab__script bar = gameObject.GetComponent<ProgressBar__prefab__script>();
        bars_List.Add(bar);
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
        OnResized(G.GetCoefHeight());
    }

    public void Refresh()
    {
        bars_List.ForEach(a => a.Refresh(displayMode));
    }

    public void ProgressBarsSortAndRefresh()
    {
        ProgressBarsSort();
        Refresh();
    }

    public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
    {
        PanelDamage__RectTransform.sizeDelta = new Vector2(343 * coefHeight, 820 * coefHeight);// 10 + 10 + 0.75*30 + 30*24
        PanelDamage__RectTransform.anchoredPosition = new Vector2(20 * coefHeight, 0);


        float buttonsSize = 70 * coefHeight;
        float buttonPos = 10 * coefHeight;
        ButtonDamage__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonHeal__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonTank__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
        ButtonDamage__RectTransform.anchoredPosition = new Vector2(buttonPos, -buttonPos);
        ButtonHeal__RectTransform.anchoredPosition = new Vector2((buttonPos * 2) + buttonsSize, -buttonPos);
        ButtonTank__RectTransform.anchoredPosition = new Vector2((buttonPos * 3) + (buttonsSize * 2), -buttonPos);

        float verticalBarWidth = 13 * coefHeight;
        ScrollbarVertical__RectTransform.anchoredPosition = new Vector2(verticalBarWidth, 0);
        ScrollbarVertical__RectTransform.sizeDelta = new Vector2(verticalBarWidth, 0);

        float PanelProgressBars__offsets = 10 * coefHeight;
        PanelProgressBars__RectTransform.SetOffsets(
            left: PanelProgressBars__offsets,
            right: PanelProgressBars__offsets + verticalBarWidth,
            top: (PanelProgressBars__offsets * 2) + buttonsSize,
            bottom: PanelProgressBars__offsets);

        float barHeight = 30 * coefHeight;
        float barHeightShift = barHeight * 2f;
        int i = 0;
        foreach (ProgressBar__prefab__script v in bars_List)
        {
            RectTransform r = v.this__RectTransform;

            r.anchoredPosition = new Vector2(0, (-barHeight * (i + 1)) - barHeightShift);
            r.sizeDelta = new Vector2(PanelProgressBars__RectTransform.rect.width, barHeight);
            i++;
        }
        Refresh();
    }

    private void ButtonDamageOnClick()
    {
        ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageDone);
    }

    private void ButtonHealOnClick()
    {
        ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.HealingDone);
    }

    private void ButtonTankOnClick()
    {
        ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageRecieved);
    }

    private void ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode displayMode)
    {
        this.displayMode = displayMode;
        ButtonDamage__Image.color = Color.white;
        ButtonHeal__Image.color = Color.white;
        ButtonTank__Image.color = Color.white;
        switch (displayMode)
        {
            case ProgressBar__prefab__script.DisplayMode.DamageDone:
                ButtonDamage__Image.color = Color.white;
                ButtonHeal__Image.color = Color.gray;
                ButtonTank__Image.color = Color.gray;
                break;
            case ProgressBar__prefab__script.DisplayMode.HealingDone:
                ButtonDamage__Image.color = Color.gray;
                ButtonHeal__Image.color = Color.white;
                ButtonTank__Image.color = Color.gray;
                break;
            case ProgressBar__prefab__script.DisplayMode.DamageRecieved:
                ButtonDamage__Image.color = Color.gray;
                ButtonHeal__Image.color = Color.gray;
                ButtonTank__Image.color = Color.white;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private int indexAdded = 0;

    /// <summary>
    /// Обновление данных на текущий ход
    /// </summary>
    public void UpdateData()
    {
        
        int i = battlefieldSceneInitializator.battlefieldIndexAnimationStarted;
        IEnumerable<BattlefieldLogRecordBase> logs = BattlefieldSceneInitializator.spawnedBattlefield.battlefieldLog.Where(a => a.index <= i && a.index >= indexAdded);
        foreach (BattlefieldLogRecordBase log in logs)
        {
            switch (log)
            {
                case BattlefieldLogRecord_Damage d:

                    // Запись нанесённого урона
                    {
                        ProgressBar__prefab__script v = bars_List.First(a=>a.heroId == d.hero1Id);
                        v.damageDone += d.damage;
                    }


                    // Запись полученного урона
                    {
                        ProgressBar__prefab__script v = bars_List.First(a => a.heroId == d.hero2Id);
                        v.damageRecieved += d.damage;
                    }

                    break;
                    //case BattlefieldLogRecord_TurnStart t:
                    //    break;
            }
        }

        indexAdded = i + 1;
    }

    public void AddDamage(Guid heroIdSource, Guid heroIdTarget, float value)
    {
        if (true)
        {

        }
    }
    public void AddHeal(Guid heroId, float value)
    {

    }
    public void AddTank(Guid heroId, float value)
    {

    }
}
