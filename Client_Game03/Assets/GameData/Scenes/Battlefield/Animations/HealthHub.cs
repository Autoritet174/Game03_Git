using Assets.GameData.Scenes.Battlefield;
using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield.Animations
{
    public static class HealthHub
    {
        public static double AnimationHealthChangeTime { get; } = 3;
        public static double AnimationSpeed { get; } = 1;
        public static float Height { get; } = 25;
        public static float FontSize { get; } = 35;
        public static float Distance { get; } = 80;

        private static readonly List<Health> animationsList = new();


        public static void Create(float value, bool isCrit, RectTransform posParent)
        {
            if (value == 0)
            {
                return;
            }

            // поиск свободного объекта в пулле
            Health health = animationsList.FirstOrDefault(a => !a.Active);
            if (health == null)
            {
                health = new Health(AddressableCache.HealthChange.SafeInstant(BattlefieldSceneInitializator.CanvasDamage__Transform));
                animationsList.Add(health);
            }

            health.Start(value, isCrit, posParent);
        }

        public static void Update()
        {
            for (int i = 0; i < animationsList.Count; i++)
            {
                animationsList[i].Update();
            }
        }

        /// <summary>
        /// Вычисляет координаты точки по расстоянию и углу от (0,0).
        /// Угол 0° соответствует направлению вверх (0, 1).
        /// Допустимый угол: [-90°, 90°].
        /// </summary>
        /// <param name="distance">Расстояние от начала координат (должно быть >= 0)</param>
        /// <param name="angleDegrees">Угол в градусах</param>
        /// <returns>Координаты точки Vector2 с Y >= 0</returns>
        public static Vector2 GetPointFromAngle(float distance, float angleDegrees)
        {
            if (distance < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "Расстояние не может быть отрицательным");
            }

            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            float x = distance * Mathf.Sin(angleRadians);
            float y = distance * Mathf.Cos(angleRadians);

            return new Vector2(x, y);
        }
    }
}
