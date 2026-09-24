using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Показывает статистику уже воспроизведённых событий боя.</summary>
    public class PanelDamage__script : IPrefab
    {
        /// <summary>Признак готовности элементов панели.</summary>
        public bool initialized { get; private set; }

        /// <summary>Ширина панели для интерфейса IPrefab.</summary>
        public float width { get; private set; }

        /// <summary>Высота панели для интерфейса IPrefab.</summary>
        public float height { get; private set; }

        /// <summary>Корневая область панели статистики.</summary>
        private RectTransform PanelDamage__RectTransform;
        /// <summary>Область прокручиваемого списка показателей.</summary>
        private RectTransform PanelProgressBars__RectTransform;
        /// <summary>Контейнер строк статистики.</summary>
        private RectTransform PanelProgressBarsContent__RectTransform;

        /// <summary>Объект вертикальной полосы прокрутки.</summary>
        private GameObject ScrollbarVertical__GameObject;
        /// <summary>Область вертикальной полосы прокрутки.</summary>
        private RectTransform ScrollbarVertical__RectTransform;

        /// <summary>Область кнопки нанесённого урона.</summary>
        private RectTransform ButtonDamageDone__RectTransform;
        /// <summary>Область кнопки выполненного лечения.</summary>
        private RectTransform ButtonHealingDone__RectTransform;
        /// <summary>Область кнопки полученного урона.</summary>
        private RectTransform ButtonDamageRecieved__RectTransform;
        /// <summary>Область кнопки полученного лечения.</summary>
        private RectTransform ButtonHealingRecieved__RectTransform;
        /// <summary>Фон кнопки нанесённого урона.</summary>
        private Image ButtonDamageDone__Image;
        /// <summary>Фон кнопки выполненного лечения.</summary>
        private Image ButtonHealingDone__Image;
        /// <summary>Фон кнопки полученного урона.</summary>
        private Image ButtonDamageRecieved__Image;
        /// <summary>Фон кнопки полученного лечения.</summary>
        private Image ButtonHealingRecieved__Image;
        /// <summary>Сцена, предоставляющая текущую статистику боя.</summary>
        public BattlefieldSceneInitializator battlefieldSceneInitializator { get; set; }

        /// <summary>
        /// Прогресс бары, которые отображаются в панели.
        /// </summary>
        private readonly List<ProgressBar__prefab__script> listProgressBars = new();

        /// <summary>Выбранный показатель статистики.</summary>
        private ProgressBar__prefab__script.DisplayMode displayMode = ProgressBar__prefab__script.DisplayMode.DamageDone;
        /// <summary>Находит элементы панели и подключает кнопки выбора показателя.</summary>
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

            initialized = true;
            OnResized(G.GetCoefHeight());
        }

        /// <summary>Создаёт строку статистики и обновляет раскладку панели.</summary>
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

        /// <summary>Цвет полосы героя команды игрока.</summary>
        private Color color1 = new(0f, 228 / 255f, 0f, 1f);
        /// <summary>Цвет полосы героя противника.</summary>
        private Color color2 = new(228 / 255f, 0f, 0f, 1f);

        /// <summary>Сортирует и отображает текущий показатель; вызывается при изменении данных или режима.</summary>
        public void Refresh()
        {
            if (listProgressBars.Count == 0)
            {
                return;
            }
            StatisticsHero[] heroes = battlefieldSceneInitializator.statisticsBattle.list_StatisticsHero
                .OrderByDescending(GetValue).ToArray();
            float max = heroes.Length > 0 ? GetValue(heroes[0]) : 0f;
            for (int i = 0; i < listProgressBars.Count; i++)
            {
                StatisticsHero stat = heroes[i];
                ProgressBar__prefab__script bar = listProgressBars[i];
                bar.SetTextLeft(stat.name);
                bar.SetTextRight(GetValue(stat).ToStr());
                bar.SetColorBar(stat.inTeam1 ? color1 : color2);
                bar.value = GetValue(stat);
                bar.valueMax = max;

                bar.Refresh();
            }
        }

        /// <summary>Возвращает показатель героя для выбранного режима панели.</summary>
        private float GetValue(StatisticsHero hero)
        {
            return displayMode switch
            {
                ProgressBar__prefab__script.DisplayMode.DamageDone => hero.damageDone,
                ProgressBar__prefab__script.DisplayMode.DamageRecieved => hero.damageReceived,
                ProgressBar__prefab__script.DisplayMode.HealingDone => hero.healingDone,
                ProgressBar__prefab__script.DisplayMode.HealingRecieved => hero.healingReceived,
                _ => 0f
            };
        }

        /// <summary>Обновляет размеры и положение элементов панели.</summary>
        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            PanelDamage__RectTransform.sizeDelta = new Vector2(343 * coefHeight, 820 * coefHeight);// 10 + 10 + 0.75*30 + 30*24
            PanelDamage__RectTransform.anchoredPosition = new Vector2(20 * coefHeight, 0);

            float buttonsSize = 70 * coefHeight;
            float buttonPos = 4 * coefHeight;

            ButtonDamageDone__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonHealingDone__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonDamageRecieved__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);
            ButtonHealingRecieved__RectTransform.sizeDelta = new Vector2(buttonsSize, buttonsSize);

            ButtonDamageDone__RectTransform.anchoredPosition = new Vector2(buttonPos, -buttonPos);
            ButtonHealingDone__RectTransform.anchoredPosition = new Vector2((buttonPos * 2) + buttonsSize, -buttonPos);
            ButtonDamageRecieved__RectTransform.anchoredPosition = new Vector2((buttonPos * 3) + (buttonsSize * 2), -buttonPos);
            ButtonHealingRecieved__RectTransform.anchoredPosition = new Vector2((buttonPos * 4) + (buttonsSize * 3), -buttonPos);

            float verticalBarWidth = 13 * coefHeight;
            ScrollbarVertical__RectTransform.anchoredPosition = new Vector2(verticalBarWidth, 0);
            ScrollbarVertical__RectTransform.sizeDelta = new Vector2(verticalBarWidth, 0);

            float PanelProgressBars__offsets = 4 * coefHeight;
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

        /// <summary>Выбирает отображение нанесённого урона.</summary>
        private void ButtonDamageDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageDone);
        }

        /// <summary>Выбирает отображение выполненного лечения.</summary>
        private void ButtonHealingDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.HealingDone);
        }

        /// <summary>Выбирает отображение полученного урона.</summary>
        private void ButtonDamageRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.DamageRecieved);
        }

        /// <summary>Выбирает отображение полученного лечения.</summary>
        private void ButtonHealingRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode.HealingRecieved);
        }

        /// <summary>Переключает показатель и немедленно обновляет панель без покадрового опроса.</summary>
        private void ChangeDisplayMode(ProgressBar__prefab__script.DisplayMode displayMode)
        {
            this.displayMode = displayMode;
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
            Refresh();
        }
    }
}
