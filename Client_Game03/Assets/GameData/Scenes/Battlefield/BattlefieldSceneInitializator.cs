using Assets.GameData.Scenes.Battlefield.Animations;
using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

namespace Assets.GameData.Scenes.Battlefield
{
    /// <summary>Создаёт поле боя и асинхронно воспроизводит полученный от сервера лог.</summary>
    public class BattlefieldSceneInitializator : MonoBehaviour
    {
        /// <summary>Начальное состояние боя, передаваемое между сценами.</summary>
        public static SpawnedBattlefield spawnedBattlefield { get; set; }

        /// <summary>Карточки героев по серверным идентификаторам.</summary>
        private readonly Dictionary<Guid, BattlefieldUnit> battlefieldUnits = new();

        /// <summary>Параллельные визуальные последствия, завершения которых дожидается сцена.</summary>
        private readonly List<UniTask> feedbackTasks = new();

        /// <summary>Последняя ширина экрана, для которой выполнена раскладка.</summary>
        public static float width { get; private set; }

        /// <summary>Последняя высота экрана, для которой выполнена раскладка.</summary>
        public static float height { get; private set; }

        /// <summary>Текущий множитель скорости воспроизведения боя.</summary>
        public static float animationSpeed { get; private set; } = 1f;

        /// <summary>Ключ сохранения выбранной скорости.</summary>
        private const string ANIMATION_SPEED_PREFS_KEY = "Battlefield.AnimationSpeed";

        /// <summary>Базовый размер кнопки скорости.</summary>
        private const float ANIMATION_SPEED_BUTTON_SIZE = 128f;

        /// <summary>Базовый отступ кнопки скорости от края экрана.</summary>
        private const float BUTTON_PADDING = 25f;

        /// <summary>Базовый размер шрифта кнопки скорости.</summary>
        private const float ANIMATION_SPEED_BUTTON_FONT_SIZE = 50f;

        /// <summary>Область кнопки скорости.</summary>
        private RectTransform animationSpeedButtonRect;

        /// <summary>Подпись кнопки скорости.</summary>
        private TextMeshProUGUI animationSpeedButtonText;

        /// <summary>Кнопка изменения скорости.</summary>
        private Button animationSpeedButton;

        /// <summary>Контейнер всплывающих чисел.</summary>
        public static Transform canvasDamage__Transform { get; private set; }

        /// <summary>Область надписи текущего хода.</summary>
        private RectTransform turnRect;

        /// <summary>Надпись текущего хода.</summary>
        private TextMeshProUGUI turnText;

        /// <summary>Фактический серверный индекс последнего начатого события.</summary>
        public int battlefieldIndexAnimationStarted { get; private set; } = -1;

        /// <summary>Накопленная статистика уже показанных последствий.</summary>
        public StatisticsBattle statisticsBattle { get; private set; }

        /// <summary>Панель статистики боя.</summary>
        private PanelDamage__script damagePanel;

        /// <summary>Проигрыватель DOTween-анимаций этой сцены.</summary>
        private BattlefieldAnimationPlayer animations;

        /// <summary>Пул всплывающих чисел изменения здоровья.</summary>
        private HealthHub healthHub;

        /// <summary>Отмена загрузки и воспроизведения при отключении или уничтожении сцены.</summary>
        private CancellationTokenSource playbackCancellation;

        /// <summary>Признак завершённой инициализации интерфейса.</summary>
        private bool initialized;

        #region Жизненный цикл сцены

        /// <summary>Подготавливает представление и запускает единственный асинхронный сценарий боя.</summary>
        private void Start()
        {
            if (!TryInitialize())
            {
                return;
            }

            Canvas.willRenderCanvases += RefreshLayoutIfNeeded;
            this.RunAsync(StartAsync);
        }

        /// <summary>Отменяет воспроизведение и снимает подписки при выходе со сцены.</summary>
        private void OnDisable()
        {
            initialized = false;
            Canvas.willRenderCanvases -= RefreshLayoutIfNeeded;
            if (animationSpeedButton != null)
            {
                animationSpeedButton.onClick.RemoveListener(AnimationSpeedChange);
            }

            playbackCancellation?.Cancel();
            animations?.Dispose();
            canvasDamage__Transform = null;
        }

        #endregion Жизненный цикл сцены

        #region Инициализация сцены

        /// <summary>Создаёт карточки героев, статистику и обработчики интерфейса.</summary>
        private bool TryInitialize()
        {
            if (spawnedBattlefield?.spawnedHeroPlayerList == null || spawnedBattlefield.spawnedHeroEnemyList == null)
            {
                GameMessage.Show("Отсутствует начальное состояние боя.", true);
                return false;
            }

            animationSpeed = LoadAnimationSpeed();
            animations = new(animationSpeed);
            statisticsBattle = new();
            Transform unitsCanvas = GameObjectFinder.FindByName("CanvasUnits").transform;
            canvasDamage__Transform = GameObjectFinder.FindByName("CanvasDamage").transform;
            healthHub = new(animations, canvasDamage__Transform);
            CreateUnits(spawnedBattlefield.spawnedHeroPlayerList, true, unitsCanvas);
            CreateUnits(spawnedBattlefield.spawnedHeroEnemyList, false, unitsCanvas);

            InitializeControls();
            InitializeStatisticsPanel();

            initialized = true;
            OnResized();
            return true;
        }

        /// <summary>Настраивает кнопку скорости и надпись текущего хода.</summary>
        private void InitializeControls()
        {
            animationSpeedButtonRect = GameObjectFinder.FindByName<RectTransform>("AnimationSpeedButton");
            animationSpeedButtonText = GameObjectFinder.FindByName<TextMeshProUGUI>("Text", animationSpeedButtonRect);
            animationSpeedButtonText.text = $"X{animationSpeed:0}";
            animationSpeedButton = animationSpeedButtonRect.GetComponent<Button>();
            animationSpeedButton.onClick.AddListener(AnimationSpeedChange);
            turnRect = GameObjectFinder.FindByName<RectTransform>("TurnText");
            turnText = turnRect.GetComponent<TextMeshProUGUI>();
        }

        /// <summary>Создаёт панель статистики и строки для всех участников боя.</summary>
        private void InitializeStatisticsPanel()
        {
            damagePanel = new()
            {
                battlefieldSceneInitializator = this
            };
            damagePanel.Initialize();
            foreach (BattlefieldUnit unit in battlefieldUnits.Values)
            {
                damagePanel.AddProgressBar();
            }

            damagePanel.Refresh();
        }

        /// <summary>Создаёт карточки одной команды и начальные строки статистики.</summary>
        private void CreateUnits(List<SpawnedHero> heroes, bool isPlayer, Transform canvas)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                SpawnedHero hero = heroes[i];
                battlefieldUnits.Add(hero.spawnedId, new BattlefieldUnit(hero, i, isPlayer, canvas, animations, healthHub));
                BaseHero baseHero = Game03Client.GameData.GetBaseHeroById(hero.baseHeroId);
                statisticsBattle.AddHero(hero.spawnedId, isPlayer, baseHero.name);
            }
        }

        #endregion Инициализация сцены

        #region Воспроизведение боя

        /// <summary>Загружает лог, ожидает все действия и параллельные визуальные последствия.</summary>
        private async UniTask StartAsync(CancellationToken destructionToken)
        {
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(destructionToken);
            playbackCancellation = cancellation;
            CancellationToken token = cancellation.Token;
            try
            {
                spawnedBattlefield.battlefieldLog = await Game03Client.Battlefield.BattlefieldProvider.GetBattleLogAsync(token);
                token.ThrowIfCancellationRequested();
                if (spawnedBattlefield.battlefieldLog == null)
                {
                    return;
                }

                BattlefieldLogPlayer player = new(spawnedBattlefield.battlefieldLog, message => Debug.LogWarning(message));
                player.recordStarted += SetCurrentRecord;
                player.RegisterRecord<BattlefieldLogRecord_TurnStart>(PlayTurnAsync);
                player.RegisterRecord<BattlefieldLogRecord_ChangeActionPoints>(PlayActionPointsAsync);
                player.RegisterRecord<BattlefieldLogRecord_Damage>(PlayDamageAsync);
                player.RegisterImpactEffect<BattlefieldLogRecord_Damage>(record => record.isPerodic ? null : record.indexReason);
                player.RegisterAbility(EBattlefieldLogAbility.attack, PlayAttackAsync);

                try
                {
                    await player.PlayAsync(token);
                }
                catch
                {
                    cancellation.Cancel();
                    _ = await UniTask.WhenAll(feedbackTasks).SuppressCancellationThrow();
                    throw;
                }

                await UniTask.WhenAll(feedbackTasks);
                feedbackTasks.Clear();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // Уход со сцены является штатным завершением воспроизведения.
            }
            finally
            {
                cancellation.Cancel();
                animations.Dispose();
                playbackCancellation = null;
            }
        }

        /// <summary>Запоминает реальный индекс записи, а не её порядковый номер в коллекции.</summary>
        private void SetCurrentRecord(int index)
        {
            battlefieldIndexAnimationStarted = index;
        }

        /// <summary>Отображает начало серверного хода.</summary>
        private UniTask PlayTurnAsync(BattlefieldLogRecord_TurnStart record, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            turnText.text = $"{LM.GetValue(L.UI.Label.Turn)}: {record.turn}";
            return UniTask.CompletedTask;
        }

        /// <summary>Применяет серверное изменение очков действия как приращение текущего значения.</summary>
        private UniTask PlayActionPointsAsync(BattlefieldLogRecord_ChangeActionPoints record, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (TryGetUnit(record.spawnedHeroId, out BattlefieldUnit unit))
            {
                unit.RefreshActionPoints(unit.spawnedHero.actionPoints + record.countAP);
            }

            return UniTask.CompletedTask;
        }

        /// <summary>Применяет урон и статистику сразу, сохраняя ожидание параллельных чисел и анимации смерти.</summary>
        private UniTask PlayDamageAsync(BattlefieldLogRecord_Damage record, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (TryGetUnit(record.hero2Id, out BattlefieldUnit target))
            {
                feedbackTasks.Add(target.ApplyDamageAsync(record.damage, record.isCrit, token));
            }

            statisticsBattle.ApplyDamage(record);
            damagePanel.Refresh();
            return UniTask.CompletedTask;
        }

        /// <summary>Воспроизводит атаку по первой доступной цели; все записанные последствия обрабатываются в момент попадания.</summary>
        private async UniTask PlayAttackAsync(BattlefieldLogRecord_UseAbility record,
            Func<CancellationToken, UniTask> impact, CancellationToken token)
        {
            if (TryGetUnit(record.spawnedHero1Id, out BattlefieldUnit attacker) && record.spawnedHeroTargets != null)
            {
                foreach (Guid targetId in record.spawnedHeroTargets)
                {
                    if (battlefieldUnits.TryGetValue(targetId, out BattlefieldUnit target))
                    {
                        await attacker.PlayAttackAsync(target, impact, token);
                        return;
                    }
                }
            }

            Debug.LogWarning($"Атака {record.index}: нет доступной карточки атакующего или цели; применяются последствия.");
            await impact(token);
        }

        /// <summary>Находит карточку или сообщает об отсутствующем участнике серверного события.</summary>
        private bool TryGetUnit(Guid id, out BattlefieldUnit unit)
        {
            if (battlefieldUnits.TryGetValue(id, out unit))
            {
                return true;
            }

            Debug.LogWarning($"В событии боя указан отсутствующий герой {id}.");
            return false;
        }

        #endregion Воспроизведение боя

        #region Скорость анимаций

        /// <summary>Циклически переключает и сохраняет скорость всех анимаций текущего боя.</summary>
        private void AnimationSpeedChange()
        {
            animationSpeed = animationSpeed switch
            {
                1f => 2f,
                2f => 5f,
                5f => 10f,
                _ => 1f
            };
            animations.SetSpeed(animationSpeed);
            animationSpeedButtonText.text = $"X{animationSpeed:0}";
            PlayerPrefs.SetFloat(ANIMATION_SPEED_PREFS_KEY, animationSpeed);
            PlayerPrefs.Save();
        }

        /// <summary>Возвращает сохранённую скорость либо стандартное значение.</summary>
        private static float LoadAnimationSpeed()
        {
            float value = PlayerPrefs.GetFloat(ANIMATION_SPEED_PREFS_KEY, 1f);
            return value is 1f or 2f or 5f or 10f ? value : 1f;
        }

        #endregion Скорость анимаций

        #region Раскладка интерфейса

        /// <summary>Реагирует на изменение экрана перед отрисовкой Canvas, не управляя временем анимаций.</summary>
        private void RefreshLayoutIfNeeded()
        {
            if (initialized && (Screen.width != width || Screen.height != height))
            {
                OnResized();
            }
        }

        /// <summary>Пересчитывает раскладку, сохраняя текущую позицию и масштаб анимируемых карточек.</summary>
        private void OnResized()
        {
            width = Screen.width;
            height = Screen.height;
            foreach (BattlefieldUnit unit in battlefieldUnits.Values)
            {
                unit.OnResize();
            }

            healthHub.OnResize();
            float coefficient = G.GetCoefHeight();
            animationSpeedButtonRect.sizeDelta = Vector2.one * (ANIMATION_SPEED_BUTTON_SIZE * coefficient);
            animationSpeedButtonRect.anchoredPosition = new Vector2(-BUTTON_PADDING, BUTTON_PADDING) * coefficient;
            animationSpeedButtonText.fontSize = ANIMATION_SPEED_BUTTON_FONT_SIZE * coefficient;
            turnRect.anchoredPosition = new Vector2(-25, -108) * coefficient;
            turnText.fontSize = 70 * coefficient;
            damagePanel.OnResized(coefficient);
        }

        #endregion Раскладка интерфейса
    }
}
