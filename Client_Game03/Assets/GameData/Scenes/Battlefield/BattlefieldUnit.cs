using Assets.GameData.Scenes.Battlefield.Animations;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
using System;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Карточка героя с независимыми асинхронными анимациями атаки и последствий.</summary>
    public partial class BattlefieldUnit
    {
        /// <summary>Цвет уровня героя силы.</summary>
        private static readonly Color colorStrength = new(224f / 255f, 0, 0);
        /// <summary>Цвет уровня героя ловкости.</summary>
        private static readonly Color colorAgility = new(0, 239f / 255f, 17f / 255f);
        /// <summary>Цвет уровня героя интеллекта.</summary>
        private static readonly Color colorIntelligence = new(0, 160f / 255f, 255f / 255f);
        /// <summary>Цвет уровня универсального героя.</summary>
        private static readonly Color colorUniversal = Color.white;

        /// <summary>Исходный масштаб живого героя.</summary>
        private static readonly float scaleAlive = 0.85f;
        /// <summary>Итоговый масштаб погибшего героя.</summary>
        private static readonly float scaleDead = scaleAlive * 0.65f;
        /// <summary>Длительность уменьшения погибшего героя при скорости ×1.</summary>
        private const float AnimationDeathScaleTime = 2f;
        /// <summary>Ширина карточки при базовом разрешении.</summary>
        private static readonly float width = 150;
        /// <summary>Высота карточки при базовом разрешении.</summary>
        private static readonly float height = 200;

        /// <summary>Вертикальное смещение внутреннего ряда.</summary>
        private static readonly float yShift1 = height * 0.6f * scaleAlive;
        /// <summary>Общее вертикальное смещение построения.</summary>
        private static readonly float yShift = 40;
        /// <summary>Вертикальное смещение внешнего ряда.</summary>
        private static readonly float yShift2 = yShift1 * 3;
        /// <summary>Вертикальные позиции четырёх рядов.</summary>
        private static readonly float[] yShiftArray = new float[] {
            -yShift2 + yShift,//1
            -yShift1 + yShift,//2
            yShift1 + yShift,//3
            yShift2 + yShift,//4
        };
        /// <summary>Горизонтальное расстояние между колонками.</summary>
        private static readonly float xShift = 200f * scaleAlive;

        /// <summary>Корневой Transform карточки.</summary>
        private readonly RectTransform rectTransform;

        /// <summary>Базовая высота полосы здоровья.</summary>
        private static readonly float health_Height = 30;

        /// <summary>Маска портрета героя.</summary>
        private readonly RectTransform imageHeroMask__RectTransform;

        /// <summary>Значок основной характеристики у полосы здоровья.</summary>
        private readonly RectTransform healthImageStat__RectTransform;

        /// <summary>Область уровня героя.</summary>
        private readonly RectTransform level_RectTransform;

        /// <summary>Подпись уровня героя.</summary>
        private readonly TextMeshProUGUI levelText_TextMeshProUGUI;

        /// <summary>Область очков действия.</summary>
        private readonly RectTransform actionPoints_RectTransform;
        /// <summary>Значок очков действия.</summary>
        private readonly RectTransform actionPointsImage_RectTransform;
        /// <summary>Область подписи очков действия.</summary>
        private readonly RectTransform actionPointsText_RectTransform;
        /// <summary>Подпись очков действия.</summary>
        private readonly TextMeshProUGUI actionPointsText_TextMeshProUGUI;

        /// <summary>Объект отметки смерти.</summary>
        private readonly GameObject imageDead_GameObject;
        /// <summary>Область отметки смерти.</summary>
        private readonly RectTransform imageDead_RectTransform;

        /// <summary>Полоса здоровья героя.</summary>
        private readonly ProgressBar__prefab__script progressBar;

        /// <summary>Текущее отображаемое состояние героя.</summary>
        public SpawnedHero spawnedHero { get; }

        /// <summary>Принадлежность карточки команде игрока.</summary>
        private readonly bool isMyUnit;
        /// <summary>Позиция героя в построении команды.</summary>
        private readonly int position;
        /// <summary>Локализованная подпись погибшего героя.</summary>
        private readonly string textDead = "Dead";
        /// <summary>Проигрыватель анимаций, принадлежащих текущей сцене.</summary>
        private readonly BattlefieldAnimationPlayer animations;

        /// <summary>Пул чисел изменения здоровья.</summary>
        private readonly HealthHub healthHub;

        /// <summary>Позиция карточки в координатах базового разрешения.</summary>
        private Vector2 animationPosition;

        /// <summary>Текущая позиция; DOTween интерполирует её независимо от разрешения экрана.</summary>
        private Vector2 animationPositionValue
        {
            get => animationPosition;
            set
            {
                animationPosition = value;
                rectTransform.anchoredPosition = value * G.GetCoefHeight();
            }
        }

        /// <summary>Множитель увеличения карточки во время атаки.</summary>
        private float attackScale = 1f;

        /// <summary>Масштаб жизни или смерти, независимый от анимации атаки.</summary>
        private float lifeScale = scaleAlive;

        /// <summary>Свойство масштаба атаки, изменяемое DOTween.</summary>
        private float attackScaleValue
        {
            get => attackScale;
            set
            {
                attackScale = value;
                RefreshScale();
            }
        }

        /// <summary>Свойство масштаба жизни, изменяемое DOTween.</summary>
        private float lifeScaleValue
        {
            get => lifeScale;
            set
            {
                lifeScale = value;
                RefreshScale();
            }
        }

        /// <summary>Создаёт карточку героя, связывает элементы интерфейса и задаёт начальное состояние.</summary>
        public BattlefieldUnit(SpawnedHero spawnedHeroes,
            int position, bool isMyUnit, Transform canvasUnits__Transform,
            BattlefieldAnimationPlayer animations, HealthHub healthHub)
        {
            spawnedHero = spawnedHeroes;
            this.position = position;
            this.isMyUnit = isMyUnit;
            this.healthHub = healthHub;
            this.animations = animations;

            GameObject gameObject = AddressablePrefabProvider.battlefieldUnit.SafeInstant(canvasUnits__Transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.baseHeroes.First(a => a.id == spawnedHeroes.baseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.name}";

            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new(0.5f, 0.5f);
            rectTransform.anchorMax = new(0.5f, 0.5f);
            rectTransform.pivot = new(0.5f, 0.5f);
            rectTransform.localScale = new(scaleAlive, scaleAlive, 1);

            Image imageRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", gameObject.transform);
            imageRarity_Image.sprite = AddressablePrefabProvider.GetRarity(dtoBaseHero.rarity);
            imageRarity_Image.preserveAspect = true;
            imageRarity_Image.type = Image.Type.Simple;

            imageHeroMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageHeroMask", gameObject.transform);

            Image imageHero_Image = GameObjectFinder.FindByName<Image>("ImageHero", gameObject.transform);
            imageHero_Image.sprite = AddressablePrefabProvider.heroes[$"{dtoBaseHero.name}_face"];
            imageHero_Image.preserveAspect = true;
            imageHero_Image.type = Image.Type.Simple;

            progressBar = GameObjectFinder.FindByName<ProgressBar__prefab__script>("ProgressBar__prefab", gameObject.transform);
            progressBar.SetTextRightOffsetRight(20);
            progressBar.Initialize();
            textDead = LM.GetValue(L.UI.Label.Dead).ToUpperInvariant();

            healthImageStat__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageStat", gameObject.transform);

            level_RectTransform = GameObjectFinder.FindByName<RectTransform>("Level", gameObject.transform);
            levelText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LevelText", gameObject.transform);
            levelText_TextMeshProUGUI.SetText(spawnedHeroes.level.ToString());
            Image level_Image = level_RectTransform.GetComponent<Image>();
            level_Image.color = dtoBaseHero.mainStat switch
            {
                EMainStat.strength => colorStrength,
                EMainStat.agility => colorAgility,
                EMainStat.intelligence => colorIntelligence,
                EMainStat.universal => colorUniversal,
                _ => throw new NotImplementedException()
            };

            actionPoints_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPoints", gameObject.transform);
            actionPointsImage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsImage", gameObject.transform);
            actionPointsText_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsText", gameObject.transform);
            actionPointsText_TextMeshProUGUI = actionPointsText_RectTransform.GetComponent<TextMeshProUGUI>();

            imageDead_GameObject = GameObjectFinder.FindByName("ImageDead", gameObject.transform);
            imageDead_RectTransform = imageDead_GameObject.GetComponent<RectTransform>();

            animationPositionValue = GetFormationPosition();
            lifeScaleValue = spawnedHero.health > 0 ? scaleAlive : scaleDead;
            OnResize();

            RefreshHealth();
            RefreshActionPoints(spawnedHero.actionPoints);
        }

        /// <summary>Обновляет размеры интерфейса, сохраняя текущие значения анимации.</summary>
        public void OnResize()
        {
            float coefHeight = G.GetCoefHeight();

            float text_Width = 130 * coefHeight;
            float text_Height = health_Height * coefHeight;
            float miniIconStat_X = -2 * coefHeight;
            float miniIconStat_Size = 17 * coefHeight;

            rectTransform.anchoredPosition = animationPositionValue * coefHeight;
            rectTransform.sizeDelta = new(width * coefHeight, height * coefHeight);

            float imageHeroMask_Padding = 10 * coefHeight;
            imageHeroMask__RectTransform.offsetMin = new(imageHeroMask_Padding, imageHeroMask_Padding);
            imageHeroMask__RectTransform.offsetMax = new(-imageHeroMask_Padding, -imageHeroMask_Padding);

            Vector2 miniIconStat_Size_Vector2 = new(miniIconStat_Size, miniIconStat_Size);
            Vector2 miniIconStat_X_Vector2 = new(miniIconStat_X, 0);

            healthImageStat__RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            healthImageStat__RectTransform.anchoredPosition = miniIconStat_X_Vector2;

            level_RectTransform.sizeDelta = new(50 * coefHeight, 25 * coefHeight);
            levelText_TextMeshProUGUI.fontSize = 22 * coefHeight;

            RefreshHealth();

            actionPoints_RectTransform.anchoredPosition = new(0, text_Height);
            actionPoints_RectTransform.sizeDelta = new(0, text_Height);
            actionPointsImage_RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            actionPointsImage_RectTransform.anchoredPosition = miniIconStat_X_Vector2;
            actionPointsText_RectTransform.sizeDelta = new(text_Width, text_Height);
            actionPointsText_TextMeshProUGUI.fontSize = 22 * coefHeight;

            float imageDead = 115 * coefHeight;
            imageDead_RectTransform.sizeDelta = new(imageDead, imageDead);
        }

        /// <summary>Обновляет здоровье и отметку смерти, не сбрасывая активные анимации масштаба.</summary>
        private void RefreshHealth()
        {
            progressBar.SetTextRight(spawnedHero.health > 0 ? spawnedHero.health.ToStr() : textDead);
            imageDead_GameObject.SetActive(spawnedHero.health <= 0);
            progressBar.value = spawnedHero.health;
            progressBar.valueMax = spawnedHero.healthMax;
            progressBar.Refresh();
        }

        /// <summary>Применяет серверный урон в момент попадания и ожидает параллельные визуальные последствия.</summary>
        public async UniTask ApplyDamageAsync(float damage, bool isCrit, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            bool wasAlive = spawnedHero.health > 0;
            spawnedHero.health -= damage;
            RefreshHealth();
            UniTask death = UniTask.CompletedTask;
            if (wasAlive && spawnedHero.health <= 0)
            {
                death = animations.PlayAsync(
                    DOTween.To(() => lifeScaleValue, value => lifeScaleValue = value, scaleDead, AnimationDeathScaleTime)
                        .SetEase(Ease.Linear), token);
            }
            await UniTask.WhenAll(death, healthHub.PlayAsync(-damage, isCrit, rectTransform, token));
        }

        /// <summary>Применяет фактическое серверное исцеление и показывает зелёное число над целью.</summary>
        public async UniTask ApplyHealingAsync(float healing, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            spawnedHero.health = Mathf.Min(spawnedHero.healthMax, spawnedHero.health + healing);
            RefreshHealth();
            await healthHub.PlayAsync(healing, false, rectTransform, cancellationToken);
        }

        /// <summary>Совмещает независимые масштабы атаки и смерти без конкурирующих записей в Transform.</summary>
        private void RefreshScale()
        {
            if (rectTransform != null)
            {
                rectTransform.localScale = new(lifeScale * attackScale, lifeScale * attackScale, 1f);
            }
        }

        /// <summary>Обновляет очки действия в модели и подписи карточки.</summary>
        public void RefreshActionPoints(int ap)
        {
            spawnedHero.actionPoints = ap;
            actionPointsText_TextMeshProUGUI.text = ap.ToString();
        }

        /// <summary>Возвращает исходную позицию героя в координатах базового разрешения.</summary>
        private Vector2 GetFormationPosition()
        {
            float x = xShift * ((position / 4) + 1);
            return new(isMyUnit ? -x : x, yShiftArray[position % 4]);
        }
    }
}
