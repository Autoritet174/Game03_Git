using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using General;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield.Animations
{
    /// <summary>Хранит пул всплывающих чисел и запускает их асинхронные анимации.</summary>
    public class HealthHub
    {
        /// <summary>Длительность показа одного числа при скорости ×1.</summary>
        public float animationHealthChangeTime { get; } = 3f;

        /// <summary>Базовый размер шрифта числа.</summary>
        public float fontSize { get; } = 35f;

        /// <summary>Базовое расстояние перемещения числа.</summary>
        public float distance { get; } = 80f;

        /// <summary>Переиспользуемые объекты чисел.</summary>
        private readonly List<Health> animationsList = new();

        /// <summary>Родительский Canvas чисел.</summary>
        private readonly Transform canvas;

        /// <summary>Общий проигрыватель анимаций текущего боя.</summary>
        private readonly BattlefieldAnimationPlayer animations;

        /// <summary>Привязывает пул к Canvas и времени текущего боя.</summary>
        public HealthHub(BattlefieldAnimationPlayer animations, Transform canvas)
        {
            this.animations = animations;
            this.canvas = canvas;
        }

        /// <summary>Берёт свободное число из пула и ожидает его полного показа или отмены.</summary>
        public async UniTask PlayAsync(float value, bool isCrit, RectTransform parent, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (value == 0)
            {
                return;
            }

            Health health = animationsList.Find(item => !item.active);
            if (health == null)
            {
                health = new(AddressablePrefabProvider.healthChange.SafeInstant(canvas), this, animations);
                animationsList.Add(health);
            }
            Vector2 offset = GetPointFromAngle(distance, RandomShared.NextSingle(-180, 180));
            await health.PlayAsync(value, isCrit, parent, offset, token);
        }

        /// <summary>Обновляет размеры активных чисел после изменения разрешения.</summary>
        public void OnResize()
        {
            foreach (Health health in animationsList)
            {
                if (health.active)
                {
                    health.OnResize();
                }
            }
        }

        /// <summary>Возвращает смещение по расстоянию и углу; нулевой угол направлен вверх.</summary>
        private static Vector2 GetPointFromAngle(float distance, float angleDegrees)
        {
            if (distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }

            float radians = angleDegrees * Mathf.Deg2Rad;
            return new(distance * Mathf.Sin(radians), distance * Mathf.Cos(radians));
        }
    }
}
