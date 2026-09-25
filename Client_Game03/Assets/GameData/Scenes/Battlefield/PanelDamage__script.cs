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
        private RectTransform panelDamage__RectTransform;
        /// <summary>Область прокручиваемого списка показателей.</summary>
        private RectTransform panelProgressBars__RectTransform;
        /// <summary>Контейнер строк статистики.</summary>
        private RectTransform panelProgressBarsContent__RectTransform;

        /// <summary>Объект вертикальной полосы прокрутки.</summary>
        private GameObject scrollbarVertical__GameObject;
        /// <summary>Область вертикальной полосы прокрутки.</summary>
        private RectTransform scrollbarVertical__RectTransform;

        /// <summary>Область кнопки нанесённого урона.</summary>
        private RectTransform buttonDamageDone__RectTransform;
        /// <summary>Область кнопки выполненного лечения.</summary>
        private RectTransform buttonHealingDone__RectTransform;
        /// <summary>Область кнопки полученного урона.</summary>
        private RectTransform buttonDamageRecieved__RectTransform;
        /// <summary>Область кнопки полученного лечения.</summary>
        private RectTransform buttonHealingRecieved__RectTransform;
        /// <summary>Фон кнопки нанесённого урона.</summary>
        private Image buttonDamageDone__Image;
        /// <summary>Фон кнопки выполненного лечения.</summary>
        private Image buttonHealingDone__Image;
        /// <summary>Фон кнопки полученного урона.</summary>
        private Image buttonDamageRecieved__Image;
        /// <summary>Фон кнопки полученного лечения.</summary>
        private Image buttonHealingRecieved__Image;
        /// <summary>Сцена, предоставляющая текущую статистику боя.</summary>
        public BattlefieldSceneInitializator battlefieldSceneInitializator { get; set; }

        /// <summary>Прогресс бары, которые отображаются в панели.</summary>
        private readonly List<ProgressBar__prefab__script> listProgressBars = new();

        /// <summary>Выбранный показатель статистики.</summary>
        private ProgressBar__prefab__script.EDisplayMode displayMode = ProgressBar__prefab__script.EDisplayMode.damageDone;

        #region Подготовка и обновление статистики
        /// <summary>Находит элементы панели и подключает кнопки выбора показателя.</summary>
        public void Initialize()
        {
            panelDamage__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelDamage");

            panelProgressBars__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelProgressBars", panelDamage__RectTransform);
            buttonDamageDone__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamageDone", panelDamage__RectTransform);
            buttonHealingDone__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHealingDone", panelDamage__RectTransform);
            buttonDamageRecieved__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonDamageRecieved", panelDamage__RectTransform);
            buttonHealingRecieved__RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonHealingRecieved", panelDamage__RectTransform);
            buttonDamageDone__Image = buttonDamageDone__RectTransform.GetComponent<Image>();
            buttonHealingDone__Image = buttonHealingDone__RectTransform.GetComponent<Image>();
            buttonDamageRecieved__Image = buttonDamageRecieved__RectTransform.GetComponent<Image>();
            buttonHealingRecieved__Image = buttonHealingRecieved__RectTransform.GetComponent<Image>();
            buttonDamageDone__RectTransform.gameObject.SetClickOnButton(ButtonDamageDoneOnClick);
            buttonHealingDone__RectTransform.gameObject.SetClickOnButton(ButtonHealingDoneOnClick);
            buttonDamageRecieved__RectTransform.gameObject.SetClickOnButton(ButtonDamageRecievedOnClick);
            buttonHealingRecieved__RectTransform.gameObject.SetClickOnButton(ButtonHealingRecievedOnClick);

            RectTransform panelProgressBarsViewport__RectTransform = GameObjectFinder.FindByName<RectTransform>("Viewport", panelProgressBars__RectTransform);
            panelProgressBarsContent__RectTransform = GameObjectFinder.FindByName<RectTransform>("Content", panelProgressBarsViewport__RectTransform);

            scrollbarVertical__GameObject = GameObjectFinder.FindByName("ScrollbarVertical", panelProgressBars__RectTransform);
            scrollbarVertical__RectTransform = scrollbarVertical__GameObject.GetComponent<RectTransform>();

            initialized = true;
            OnResized(G.GetCoefHeight());
        }

        /// <summary>Создаёт строку статистики и обновляет раскладку панели.</summary>
        public void AddProgressBar()
        {
            GameObject gameObject = AddressablePrefabProvider.progressBar.SafeInstant(panelProgressBarsContent__RectTransform.transform);
            if (gameObject == null)
            {
                return;
            }

            gameObject.name = $"ProgressBar__prefab {listProgressBars.Count + 1}";
            ProgressBar__prefab__script bar = gameObject.GetComponent<ProgressBar__prefab__script>();
            listProgressBars.Add(bar);
            bar.Initialize();
            bar.this__RectTransform.anchorMin = new(0, 1);
            bar.this__RectTransform.anchorMax = new(0, 1);
            bar.this__RectTransform.pivot = new(0, 1);
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
                ProgressBar__prefab__script.EDisplayMode.damageDone => hero.damageDone,
                ProgressBar__prefab__script.EDisplayMode.damageRecieved => hero.damageReceived,
                ProgressBar__prefab__script.EDisplayMode.healingDone => hero.healingDone,
                ProgressBar__prefab__script.EDisplayMode.healingRecieved => hero.healingReceived,
                _ => 0f
            };
        }

        #endregion Подготовка и обновление статистики

        /// <summary>Обновляет размеры и положение элементов панели.</summary>
        public void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0)
        {
            panelDamage__RectTransform.sizeDelta = new(343 * coefHeight, 820 * coefHeight);// 10 + 10 + 0.75*30 + 30*24
            panelDamage__RectTransform.anchoredPosition = new(20 * coefHeight, 0);

            float buttonsSize = 70 * coefHeight;
            float buttonPos = 4 * coefHeight;

            buttonDamageDone__RectTransform.sizeDelta = new(buttonsSize, buttonsSize);
            buttonHealingDone__RectTransform.sizeDelta = new(buttonsSize, buttonsSize);
            buttonDamageRecieved__RectTransform.sizeDelta = new(buttonsSize, buttonsSize);
            buttonHealingRecieved__RectTransform.sizeDelta = new(buttonsSize, buttonsSize);

            buttonDamageDone__RectTransform.anchoredPosition = new(buttonPos, -buttonPos);
            buttonHealingDone__RectTransform.anchoredPosition = new((buttonPos * 2) + buttonsSize, -buttonPos);
            buttonDamageRecieved__RectTransform.anchoredPosition = new((buttonPos * 3) + (buttonsSize * 2), -buttonPos);
            buttonHealingRecieved__RectTransform.anchoredPosition = new((buttonPos * 4) + (buttonsSize * 3), -buttonPos);

            float verticalBarWidth = 13 * coefHeight;
            scrollbarVertical__RectTransform.anchoredPosition = new(verticalBarWidth, 0);
            scrollbarVertical__RectTransform.sizeDelta = new(verticalBarWidth, 0);

            float panelProgressBars__offsets = 4 * coefHeight;
            panelProgressBars__RectTransform.SetOffsets(
                left: panelProgressBars__offsets,
                right: panelProgressBars__offsets + verticalBarWidth,
                top: (panelProgressBars__offsets * 2) + buttonsSize,
                bottom: panelProgressBars__offsets);

            float barHeight = 30 * coefHeight;
            float barHeightShift = barHeight * 2f;
            int i = 0;
            foreach (ProgressBar__prefab__script v in listProgressBars)
            {
                RectTransform r = v.this__RectTransform;

                r.anchoredPosition = new(0, (-barHeight * (i + 1)) - barHeightShift);
                r.sizeDelta = new(panelProgressBars__RectTransform.rect.width - (10 * coefHeight), barHeight);
                i++;
            }
            Refresh();
        }

        #region Переключение показателей

        /// <summary>Выбирает отображение нанесённого урона.</summary>
        private void ButtonDamageDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.EDisplayMode.damageDone);
        }

        /// <summary>Выбирает отображение выполненного лечения.</summary>
        private void ButtonHealingDoneOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.EDisplayMode.healingDone);
        }

        /// <summary>Выбирает отображение полученного урона.</summary>
        private void ButtonDamageRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.EDisplayMode.damageRecieved);
        }

        /// <summary>Выбирает отображение полученного лечения.</summary>
        private void ButtonHealingRecievedOnClick()
        {
            ChangeDisplayMode(ProgressBar__prefab__script.EDisplayMode.healingRecieved);
        }

        /// <summary>Переключает показатель и немедленно обновляет панель без покадрового опроса.</summary>
        private void ChangeDisplayMode(ProgressBar__prefab__script.EDisplayMode displayMode)
        {
            this.displayMode = displayMode;
            switch (displayMode)
            {
                case ProgressBar__prefab__script.EDisplayMode.damageDone:
                    buttonDamageDone__Image.color = Color.white;
                    buttonHealingDone__Image.color = Color.gray;
                    buttonDamageRecieved__Image.color = Color.gray;
                    buttonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.EDisplayMode.healingDone:
                    buttonDamageDone__Image.color = Color.gray;
                    buttonHealingDone__Image.color = Color.white;
                    buttonDamageRecieved__Image.color = Color.gray;
                    buttonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.EDisplayMode.damageRecieved:
                    buttonDamageDone__Image.color = Color.gray;
                    buttonHealingDone__Image.color = Color.gray;
                    buttonDamageRecieved__Image.color = Color.white;
                    buttonHealingRecieved__Image.color = Color.gray;
                    break;
                case ProgressBar__prefab__script.EDisplayMode.healingRecieved:
                    buttonDamageDone__Image.color = Color.gray;
                    buttonHealingDone__Image.color = Color.gray;
                    buttonDamageRecieved__Image.color = Color.gray;
                    buttonHealingRecieved__Image.color = Color.white;
                    break;
                default:
                    throw new NotImplementedException();
            }
            Refresh();
        }

        #endregion Переключение показателей
    }
}
