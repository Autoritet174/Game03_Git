using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {
        /// <summary>Длительность увеличения карточки при скорости ×1.</summary>
        public const float AnimationAttackTimeStage1 = 0.3f;

        /// <summary>Номинальная длительность рывка; фактический рывок заканчивается на расстоянии ширины карточки.</summary>
        public const float AnimationAttackTimeStage2 = 0.5f;

        /// <summary>Длительность возврата и уменьшения карточки.</summary>
        public const float AnimationAttackTimeStage3 = 0.4f;

        /// <summary>Пауза после возврата перед следующим действием.</summary>
        public const float AnimationAttackTimeStage4 = 0.5f;

        /// <summary>Последовательно ожидает увеличение, рывок, попадание, возврат и заключительную паузу.</summary>
        public async UniTask PlayAttackAsync(BattlefieldUnit target, Func<CancellationToken, UniTask> impact, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _RectTransform.SetAsLastSibling();
            Vector2 origin = GetFormationPosition();
            try
            {
                await animations.PlayAsync(
                    DOTween.To(() => AttackScale, value => AttackScale = value, 1.3f, AnimationAttackTimeStage1)
                        .SetEase(Ease.Linear), token);

                Vector2 destination = target.GetFormationPosition();
                float distance = Vector2.Distance(origin, destination);
                if (distance > _Width)
                {
                    // Аналитическое время прежнего порога попадания: 1 - (1 - 1.2t)^6.
                    float remaining = _Width / distance;
                    float impactProgress = (1f - Mathf.Pow(remaining, 1f / 6f)) / 1.2f;
                    float travelled = 1f - remaining;
                    Vector2 contact = Vector2.LerpUnclamped(origin, destination, travelled);
                    await animations.PlayAsync(
                        DOTween.To(() => AnimationPosition, value => AnimationPosition = value,
                            contact, AnimationAttackTimeStage2 * impactProgress)
                            .SetEase((time, duration, overshoot, period) =>
                                (1f - Mathf.Pow(1f - 1.2f * impactProgress * time / duration, 6f)) / travelled), token);
                }

                await impact(token);
                token.ThrowIfCancellationRequested();
                Sequence returning = DOTween.Sequence()
                    .Join(DOTween.To(() => AnimationPosition, value => AnimationPosition = value,
                        origin, AnimationAttackTimeStage3).SetEase(Ease.Linear))
                    .Join(DOTween.To(() => AttackScale, value => AttackScale = value,
                        1f, AnimationAttackTimeStage3).SetEase(Ease.Linear));
                await animations.PlayAsync(returning, token);
                await animations.DelayAsync(AnimationAttackTimeStage4, token);
            }
            finally
            {
                if (_RectTransform != null)
                {
                    AnimationPosition = origin;
                    AttackScale = 1f;
                }
            }
        }
    }
}
