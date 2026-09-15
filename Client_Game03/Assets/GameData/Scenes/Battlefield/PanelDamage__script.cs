using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Battlefield
{
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

        private RectTransform ButtonDamageDone__RectTransform;
        private RectTransform ButtonHealingDone__RectTransform;
        private RectTransform ButtonDamageRecieved__RectTransform;
        private RectTransform ButtonHealingRecieved__RectTransform;
        private Image ButtonDamageDone__Image;
        private Image ButtonHealingDone__Image;
        private Image ButtonDamageRecieved__Image;
        private Image ButtonHealingRecieved__Image;
        public BattlefieldSceneInitializator battlefieldSceneInitializator { get; set; }

        //private readonly List<Bar> bars = new();

        /// <summary>
        /// Прогресс бары, которые отображаются в панели.
        /// </summary>
        private readonly List<ProgressBar__prefab__script> listProgressBars = new();


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
        public void Initialize()
        {
            PanelDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelDamage");

            PanelProgressBars__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelProgressBars", PanelDamage__RectTransform);
            ButtonDamageDone__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamageDone", PanelDamage__RectTransform);
            ButtonHealingDone__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHealingDone", PanelDamage__RectTransform);
            ButtonDamageRecieved__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamageRecieved", PanelDamage__RectTransform);
            ButtonHealingRecieved__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHealingRecieved", PanelDamage__RectTransform);
            ButtonDamageDone__Image = ButtonDamageDone__RectTransform.GetComponent<Image>();
            ButtonHealingDone__Image = ButtonHealingDone__RectTransform.GetComponent<Image>();
            ButtonDamageRecieved__Image = ButtonDamageRecieved__RectTransform.GetComponent<Image>();
            ButtonHealingRecieved__Image = ButtonHealingRecieved__RectTransform.GetComponent<Image>();
            ButtonDamageDone__RectTransform.gameObject.SetClickOnButton(ButtonDamageDoneOnClick);
            ButtonHealingDone__RectTransform.gameObject.SetClickOnButton(ButtonHealingDoneOnClick);
            ButtonDamageRecieved__RectTransform.gameObject.SetClickOnButton(ButtonDamageRecievedOnClick);
            ButtonHealingRecieved__RectTransform.gameObject.SetClickOnButton(ButtonHealingRecievedOnClick);

            RectTransform PanelProgressBarsViewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", PanelProgressBars__RectTransform);
            PanelProgressBarsContent__RectTransform = GameObjectFinder.FindByName<RectTransform>("Content", PanelProgressBarsViewport__RectTransform);

            ScrollbarVertical__GameObject = GameObjectFinder.FindByName("ScrollbarVertical", PanelProgressBars__RectTransform);
            ScrollbarVertical__RectTransform = ScrollbarVertical__GameObject.GetComponent<RectTransform>();

            OnResized(G.GetCoefHeight());
        }

        public void AddProgressBar()
        {
            GameObject gameObject = AddressablePrefabProvider.ProgressBar.SafeInstant(PanelProgressBarsContent__RectTransform.transform);
            if (gameObject == null)
            {
                return;
            }

            gameObject.name = $"ProgressBar__prefab {listProgressBars.Count + 1}";
            ProgressBar__prefab__script bar = gameObject.GetComponent<ProgressBar__prefab__script>();
            listProgressBars.Add(bar);
            bar.Initialize();
            bar.this__RectTransform.anchorMin = new Vector2(0, 1);
            bar.this__RectTransform.anchorMax = new Vector2(0, 1);
            bar.this__RectTransform.pivot = new Vector2(0, 1);
            OnResized(G.GetCoefHeight());
        }

        private Color color1 = new(0f, 228 / 255f, 0f, 1f);
        private Color color2 = new(228 / 255f, 0f, 0f, 1f);

        public void Refresh()
        {
            if (listProgressBars.Count == 0)
            {
                return;
            }
            float max = battlefieldSceneInitializator.statisticsBattle.list_StatisticsHero.Max(a => a.damageDone);
            for (int i = 0; i < listProgressBars.Count; i++)
            {
                StatisticsHero stat = battlefieldSceneInitializator.statisticsBattle.list_StatisticsHero[i];
                ProgressBar__prefab__script bar = listProgressBars[i];
                bar.SetTextLeft(stat.name);
                bar.SetTextRight(stat.damageDone.ToStr());
                //bar.SetColorTextLeft(stat.inTeam1 ? color1 : color2);
                //bar.SetColorTextRight(stat.inTeam1 ? color1 : color2);
                bar.SetColorBar(stat.inTeam1 ? color1 : color2);
                bar.value = stat.damageDone;
                bar.valueMax = max;


                bar.Refresh();
            }
        }

        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            PanelDamage__RectTransform.sizeDelta = new Vector2(343 * coefHeight, 820 * coefHeight);// 10 + 10 + 0.75*30 + 30*24
            PanelDamage__RectTransform.anchoredPosition = new Vector2(20 * coefHeight, 0);


            float buttonsSize = 70 * coefHeight;
            float buttonPos = 10 * coefHeight;
            ButtonDamageDone__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonHealingDone__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonDamageRecieved__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonDamageDone__RectTransform.anchoredPosition = new Vector2(buttonPos, -buttonPos);
            ButtonHealingDone__RectTransform.anchoredPosition = new Vector2((buttonPos * 2) + buttonsSize, -buttonPos);
            ButtonDamageRecieved__RectTransform.anchoredPosition = new Vector2((buttonPos * 3) + (buttonsSize * 2), -buttonPos);

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
            foreach (ProgressBar__prefab__script v in listProgressBars)
            {
                RectTransform r = v.this__RectTransform;

                r.anchoredPosition = new Vector2(0, (-barHeight * (i + 1)) - barHeightShift);
                r.sizeDelta = new Vector2(PanelProgressBars__RectTransform.rect.width - (10 * coefHeight), barHeight);
                i++;
            }
            Refresh();
        }

        private void ButtonDamageDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageDone);
        }

        private void ButtonHealingDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.HealingDone);
        }

        private void ButtonDamageRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageRecieved);
        }

        private void ButtonHealingRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.HealingRecieved);
        }

        private void ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode displayMode)
        {
            this.displayMode = displayMode;
            //ButtonDamageDone__Image.color = Color.white;
            //ButtonHealingDone__Image.color = Color.white;
            //ButtonDamageRecieved__Image.color = Color.white;
            switch (displayMode)
            {
                case ProgressBar__prefab__script.DisplayMode.DamageDone:
                    ButtonDamageDone__Image.color = Color.white;
                    ButtonHealingDone__Image.color = Color.gray;
                    ButtonDamageRecieved__Image.color = Color.gray;
                    ButtonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.DisplayMode.HealingDone:
                    ButtonDamageDone__Image.color = Color.gray;
                    ButtonHealingDone__Image.color = Color.white;
                    ButtonDamageRecieved__Image.color = Color.gray;
                    ButtonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.DisplayMode.DamageRecieved:
                    ButtonDamageDone__Image.color = Color.gray;
                    ButtonHealingDone__Image.color = Color.gray;
                    ButtonDamageRecieved__Image.color = Color.white;
                    ButtonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.DisplayMode.HealingRecieved:
                    ButtonDamageDone__Image.color = Color.gray;
                    ButtonHealingDone__Image.color = Color.gray;
                    ButtonDamageRecieved__Image.color = Color.gray;
                    ButtonHealingRecieved__Image.color = Color.white;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
