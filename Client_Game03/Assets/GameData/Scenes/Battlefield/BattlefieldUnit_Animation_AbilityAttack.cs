using Assets.GameData.Scripts;
using System;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {
        //private static readonly double AnimationSpeed = 1.5;
        public static readonly double AnimationAttackTimeStage1 = 0.3;
        public static readonly double AnimationAttackTimeStage2 = 0.5;
        public static readonly double AnimationAttackTimeStage3 = 0.4;
        public static readonly double AnimationAttackTimeStage4 = 0.5;

        public int AnimationAttackStage { get; private set; } = 0;
        private DateTime AtimationAttackStart = DateTime.Now;
        private DateTime AtimationAttackEnd = DateTime.Now;
        private BattlefieldUnit AtimationAttackUnitTarget;
        private Vector2 AtimationAttackPosEnd = Vector2.zero;
        private float AnimationAttackDamage = 0;
        private bool AnimationAttackDamageIsCrit = false;
        private readonly Animations.HealthHub healthHub;
        public void AnimationStartAttackUnit(BattlefieldUnit unitTarget, float animationAttackDamage, bool animationAttackDamageIsCrit)
        {
            AtimationAttackUnitTarget = unitTarget;
            AnimationAttackStage = 1;
            AtimationAttackStart = DateTime.Now;
            AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage1 / BattlefieldSceneInitializator.AnimationSpeed);
            AnimationAttackDamage = animationAttackDamage;
            AnimationAttackDamageIsCrit = animationAttackDamageIsCrit;
            _RectTransform.transform.SetAsLastSibling();
        }

        public void UpdateAnimationAttack()
        {
            if (AnimationAttackStage == 0)
            {
                return;
            }

            float animationPercent = Math.Clamp((float)((DateTime.Now - AtimationAttackStart).TotalSeconds / (AtimationAttackEnd - AtimationAttackStart).TotalSeconds), 0, 1);

            if (AnimationAttackStage == 1) // увеличение масштаба
            {
                float coef = (1f + (0.3f * animationPercent)) * _ScaleAlive;
                _RectTransform.localScale = new(coef, coef, 1);
                if (animationPercent == 1)
                {
                    AnimationAttackStage = 2;
                    AtimationAttackStart = DateTime.Now;
                    AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage2 / BattlefieldSceneInitializator.AnimationSpeed);
                }
            }
            else if (AnimationAttackStage == 2) // движение от базовой точки до цели
            {
                float animationPercentForPos = AnimationShiftPower(animationPercent * 1.2f);

                Vector2 posStart = GetCoords();
                Vector2 posEnd = AtimationAttackUnitTarget.GetCoords();
                float distX = posEnd.x - posStart.x;
                float distY = posEnd.y - posStart.y;
                float x = posStart.x + (distX * animationPercentForPos);
                float y = posStart.y + (distY * animationPercentForPos);
                AtimationAttackPosEnd = new Vector2(x, y);
                _RectTransform.anchoredPosition = AtimationAttackPosEnd;
                if (animationPercent == 1 || MathF.Sqrt(MathF.Pow(posEnd.x - x, 2) + MathF.Pow(posEnd.y - y, 2)) < _Width * G.GetCoefHeight())
                {
                    AnimationAttackStage = 3;
                    AtimationAttackStart = DateTime.Now;
                    AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage3 / BattlefieldSceneInitializator.AnimationSpeed);
                    AtimationAttackUnitTarget.RefreshHealth();
                    healthHub.Create(
                        -AnimationAttackDamage,
                        AnimationAttackDamageIsCrit,
                        AtimationAttackUnitTarget._RectTransform);

                    // тут добавляем весь урон (урон при ударе, кливы, проки в момент удара)
                    panelDamage__script.Update();
                }
            }
            else if (AnimationAttackStage == 3) // движение от цели до базовой точки
            {
                Vector2 posStart = AtimationAttackPosEnd;
                Vector2 posEnd = GetCoords();
                float distX = posEnd.x - posStart.x;
                float distY = posEnd.y - posStart.y;
                float x = posStart.x + (distX * animationPercent);
                float y = posStart.y + (distY * animationPercent);
                _RectTransform.anchoredPosition = new Vector2(x, y);

                float coef = (1f + (0.3f * (1 - animationPercent))) * _ScaleAlive;
                _RectTransform.localScale = new(coef, coef, 1);
                if (animationPercent == 1)
                {
                    AnimationAttackStage = 4;
                    AtimationAttackStart = DateTime.Now;
                    AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage4 / BattlefieldSceneInitializator.AnimationSpeed);
                }
            }
            else if (AnimationAttackStage == 4) // ожидание перед сбросом параметров
            {
                if (animationPercent == 1)
                {
                    AnimationAttackStage = 0;
                }
            }

        }
    }
}
