using Assets.GameData.Scripts;
using Assets.GameData.Scenes.Battlefield.Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
using System;
using System.Linq;
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
        private static readonly float _ScaleAlive = 0.85f;
        /// <summary>Итоговый масштаб погибшего героя.</summary>
        private static readonly float _ScaleDead = _ScaleAlive * 0.65f;
        /// <summary>Длительность уменьшения погибшего героя при скорости ×1.</summary>
        private const float AnimationDeathScaleTime = 2f;
        /// <summary>Ширина карточки при базовом разрешении.</summary>
        private static readonly float _Width = 150;
        /// <summary>Высота карточки при базовом разрешении.</summary>
        private static readonly float _Height = 200;

        /// <summary>Вертикальное смещение внутреннего ряда.</summary>
        private static readonly float yShift1 = _Height * 0.6f * _ScaleAlive;
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
        private static readonly float xShift = 200f * _ScaleAlive;

        /// <summary>Корневой Transform карточки.</summary>
        private readonly RectTransform _RectTransform;

        /// <summary>Базовая высота полосы здоровья.</summary>
        private static readonly float _Health_Height = 30;

        /// <summary>Маска портрета героя.</summary>
        private readonly RectTransform _ImageHeroMask__RectTransform;

        /// <summary>Значок основной характеристики у полосы здоровья.</summary>
        private readonly RectTransform _HealthImageStat__RectTransform;

        /// <summary>Область уровня героя.</summary>
        private readonly RectTransform _Level_RectTransform;

        /// <summary>Подпись уровня героя.</summary>
        private readonly TextMeshProUGUI _LevelText_TextMeshProUGUI;

        /// <summary>Область очков действия.</summary>
        private readonly RectTransform _ActionPoints_RectTransform;
        /// <summary>Значок очков действия.</summary>
        private readonly RectTransform _ActionPointsImage_RectTransform;
        /// <summary>Область подписи очков действия.</summary>
        private readonly RectTransform _ActionPointsText_RectTransform;
        /// <summary>Подпись очков действия.</summary>
        private readonly TextMeshProUGUI _ActionPointsText_TextMeshProUGUI;

        /// <summary>Объект отметки смерти.</summary>
        private readonly GameObject _ImageDead_GameObject;
        /// <summary>Область отметки смерти.</summary>
        private readonly RectTransform _ImageDead_RectTransform;

        /// <summary>Полоса здоровья героя.</summary>
        private readonly ProgressBar__prefab__script progressBar;

        /// <summary>Текущее отображаемое состояние героя.</summary>
        public SpawnedHero SpawnedHero { get; }

        /// <summary>Принадлежность карточки команде игрока.</summary>
        private readonly bool _IsMyUnit;
        /// <summary>Позиция героя в построении команды.</summary>
        private readonly int _Position;
        /// <summary>Локализованная подпись погибшего героя.</summary>
        private readonly string textDead = "Dead";
        /// <summary>Проигрыватель анимаций, принадлежащих текущей сцене.</summary>
        private readonly BattlefieldAnimationPlayer animations;

        /// <summary>Пул чисел изменения здоровья.</summary>
        private readonly HealthHub healthHub;

        /// <summary>Позиция карточки в координатах базового разрешения.</summary>
        private Vector2 animationPosition;

        /// <summary>Текущая позиция; DOTween интерполирует её независимо от разрешения экрана.</summary>
        private Vector2 AnimationPosition
        {
            get => animationPosition;
            set
            {
                animationPosition = value;
                _RectTransform.anchoredPosition = value * G.GetCoefHeight();
            }
        }

        /// <summary>Множитель увеличения карточки во время атаки.</summary>
        private float attackScale = 1f;

        /// <summary>Масштаб жизни или смерти, независимый от анимации атаки.</summary>
        private float lifeScale = _ScaleAlive;

        /// <summary>Свойство масштаба атаки, изменяемое DOTween.</summary>
        private float AttackScale
        {
            get => attackScale;
            set { attackScale = value; RefreshScale(); }
        }

        /// <summary>Свойство масштаба жизни, изменяемое DOTween.</summary>
        private float LifeScale
        {
            get => lifeScale;
            set { lifeScale = value; RefreshScale(); }
        }

        /// <summary>Создаёт карточку героя, связывает элементы интерфейса и задаёт начальное состояние.</summary>
        public BattlefieldUnit(SpawnedHero spawnedHeroes,
            int position, bool isMyUnit, Transform canvasUnits__Transform,
            BattlefieldAnimationPlayer animations, HealthHub healthHub)
        {
            SpawnedHero = spawnedHeroes;
            _Position = position;
            _IsMyUnit = isMyUnit;
            this.healthHub = healthHub;
            this.animations = animations;

            GameObject gameObject = AddressablePrefabProvider.BattlefieldUnit.SafeInstant(canvasUnits__Transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.baseHeroes.First(a => a.id == spawnedHeroes.baseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.name}";

            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchorMin = new(0.5f, 0.5f);
            _RectTransform.anchorMax = new(0.5f, 0.5f);
            _RectTransform.pivot = new(0.5f, 0.5f);
            _RectTransform.localScale = new Vector3(_ScaleAlive, _ScaleAlive, 1);

            Image _ImageRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", gameObject.transform);
            _ImageRarity_Image.sprite = AddressablePrefabProvider.GetRarity(dtoBaseHero.rarity);
            _ImageRarity_Image.preserveAspect = true;
            _ImageRarity_Image.type = Image.Type.Simple;

            _ImageHeroMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageHeroMask", gameObject.transform);

            Image _ImageHero_Image = GameObjectFinder.FindByName<Image>("ImageHero", gameObject.transform);
            _ImageHero_Image.sprite = AddressablePrefabProvider.Heroes[$"{dtoBaseHero.name}_face"];
            _ImageHero_Image.preserveAspect = true;
            _ImageHero_Image.type = Image.Type.Simple;

            progressBar = GameObjectFinder.FindByName<ProgressBar__prefab__script>("ProgressBar__prefab", gameObject.transform);
            progressBar.SetTextRightOffsetRight(20);
            progressBar.Initialize();
            textDead = LM.GetValue(L.UI.Label.Dead).ToUpperInvariant();

            _HealthImageStat__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageStat", gameObject.transform);

            _Level_RectTransform = GameObjectFinder.FindByName<RectTransform>("Level", gameObject.transform);
            _LevelText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LevelText", gameObject.transform);
            _LevelText_TextMeshProUGUI.SetText(spawnedHeroes.level.ToString());
            Image _Level_Image = _Level_RectTransform.GetComponent<Image>();
            _Level_Image.color = dtoBaseHero.mainStat switch
            {
                EMainStat.strength => colorStrength,
                EMainStat.agility => colorAgility,
                EMainStat.intelligence => colorIntelligence,
                EMainStat.universal => colorUniversal,
                _ => throw new NotImplementedException()
            };

            _ActionPoints_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPoints", gameObject.transform);
            _ActionPointsImage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsImage", gameObject.transform);
            _ActionPointsText_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsText", gameObject.transform);
            _ActionPointsText_TextMeshProUGUI = _ActionPointsText_RectTransform.GetComponent<TextMeshProUGUI>();

            _ImageDead_GameObject = GameObjectFinder.FindByName("ImageDead", gameObject.transform);
            _ImageDead_RectTransform = _ImageDead_GameObject.GetComponent<RectTransform>();

            AnimationPosition = GetFormationPosition();
            LifeScale = SpawnedHero.health > 0 ? _ScaleAlive : _ScaleDead;
            OnResize();

            RefreshHealth();
            RefreshActionPoints(SpawnedHero.actionPoints);
        }

        /// <summary>Обновляет размеры интерфейса, сохраняя текущие значения анимации.</summary>
        public void OnResize()
        {
            float coefHeight = G.GetCoefHeight();

            float text_Width = 130 * coefHeight;
            float text_Height = _Health_Height * coefHeight;
            float miniIconStat_X = -2 * coefHeight;
            float miniIconStat_Size = 17 * coefHeight;

            _RectTransform.anchoredPosition = AnimationPosition * coefHeight;
            _RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Height * coefHeight);

            float imageHeroMask_Padding = 10 * coefHeight;
            _ImageHeroMask__RectTransform.offsetMin = new(imageHeroMask_Padding, imageHeroMask_Padding);
            _ImageHeroMask__RectTransform.offsetMax = new(-imageHeroMask_Padding, -imageHeroMask_Padding);

            Vector2 miniIconStat_Size_Vector2 = new(miniIconStat_Size, miniIconStat_Size);
            Vector2 miniIconStat_X_Vector2 = new(miniIconStat_X, 0);

            _HealthImageStat__RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            _HealthImageStat__RectTransform.anchoredPosition = miniIconStat_X_Vector2;

            _Level_RectTransform.sizeDelta = new Vector2(50 * coefHeight, 25 * coefHeight);
            _LevelText_TextMeshProUGUI.fontSize = 22 * coefHeight;

            RefreshHealth();

            _ActionPoints_RectTransform.anchoredPosition = new Vector2(0, text_Height);
            _ActionPoints_RectTransform.sizeDelta = new Vector2(0, text_Height);
            _ActionPointsImage_RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            _ActionPointsImage_RectTransform.anchoredPosition = miniIconStat_X_Vector2;
            _ActionPointsText_RectTransform.sizeDelta = new Vector2(text_Width, text_Height);
            _ActionPointsText_TextMeshProUGUI.fontSize = 22 * coefHeight;

            float imageDead = 115 * coefHeight;
            _ImageDead_RectTransform.sizeDelta = new Vector2(imageDead, imageDead);
        }

        /// <summary>Обновляет здоровье и отметку смерти, не сбрасывая активные анимации масштаба.</summary>
        private void RefreshHealth()
        {
            progressBar.SetTextRight(SpawnedHero.health > 0 ? SpawnedHero.health.ToStr() : textDead);
            _ImageDead_GameObject.SetActive(SpawnedHero.health <= 0);
            progressBar.value = SpawnedHero.health;
            progressBar.valueMax = SpawnedHero.healthMax;
            progressBar.Refresh();
        }

        /// <summary>Применяет серверный урон в момент попадания и ожидает параллельные визуальные последствия.</summary>
        public async UniTask ApplyDamageAsync(float damage, bool isCrit, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            bool wasAlive = SpawnedHero.health > 0;
            SpawnedHero.health -= damage;
            RefreshHealth();
            UniTask death = UniTask.CompletedTask;
            if (wasAlive && SpawnedHero.health <= 0)
            {
                death = animations.PlayAsync(
                    DOTween.To(() => LifeScale, value => LifeScale = value, _ScaleDead, AnimationDeathScaleTime)
                        .SetEase(Ease.Linear), token);
            }
            await UniTask.WhenAll(death, healthHub.PlayAsync(-damage, isCrit, _RectTransform, token));
        }

        /// <summary>Совмещает независимые масштабы атаки и смерти без конкурирующих записей в Transform.</summary>
        private void RefreshScale()
        {
            if (_RectTransform != null)
                _RectTransform.localScale = new Vector3(lifeScale * attackScale, lifeScale * attackScale, 1f);
        }

        /// <summary>Обновляет очки действия в модели и подписи карточки.</summary>
        public void RefreshActionPoints(int ap)
        {
            SpawnedHero.actionPoints = ap;
            _ActionPointsText_TextMeshProUGUI.text = ap.ToString();
        }

        /// <summary>Возвращает исходную позицию героя в координатах базового разрешения.</summary>
        private Vector2 GetFormationPosition()
        {
            float x = xShift * ((_Position / 4) + 1);
            return new Vector2(_IsMyUnit ? -x : x, yShiftArray[_Position % 4]);
        }
    }
}
