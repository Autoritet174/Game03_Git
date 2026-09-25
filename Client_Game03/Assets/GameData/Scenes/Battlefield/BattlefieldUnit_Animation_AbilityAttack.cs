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
        public const float ANIMATION_ATTACK_TIME_STAGE_1 = 0.3f;

        /// <summary>Номинальная длительность рывка; фактический рывок заканчивается на расстоянии ширины карточки.</summary>
        public const float ANIMATION_ATTACK_TIME_STAGE_2 = 0.5f;

        /// <summary>Длительность возврата и уменьшения карточки.</summary>
        public const float ANIMATION_ATTACK_TIME_STAGE_3 = 0.4f;

        /// <summary>Пауза после возврата перед следующим действием.</summary>
        public const float ANIMATION_ATTACK_TIME_STAGE_4 = 0.5f;

        /// <summary>Последовательно ожидает увеличение, рывок, попадание, возврат и заключительную паузу.</summary>
        public async UniTask PlayAttackAsync(BattlefieldUnit target, Func<CancellationToken, UniTask> impact, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            rectTransform.SetAsLastSibling();
            Vector2 origin = GetFormationPosition();
            try
            {
                await animations.PlayAsync(
                    DOTween.To(() => attackScaleValue, value => attackScaleValue = value, 1.3f, ANIMATION_ATTACK_TIME_STAGE_1)
                        .SetEase(Ease.Linear), token);

                Vector2 destination = target.GetFormationPosition();
                float distance = Vector2.Distance(origin, destination);
                if (distance > width)
                {
                    // Аналитическое время прежнего порога попадания: 1 - (1 - 1.2t)^6.
                    float remaining = width / distance;
                    float impactProgress = (1f - Mathf.Pow(remaining, 1f / 6f)) / 1.2f;
                    float travelled = 1f - remaining;
                    var contact = Vector2.LerpUnclamped(origin, destination, travelled);
                    await animations.PlayAsync(
                        DOTween.To(() => animationPositionValue, value => animationPositionValue = value,
                            contact, ANIMATION_ATTACK_TIME_STAGE_2 * impactProgress)
                            .SetEase((time, duration, overshoot, period) =>
                                (1f - Mathf.Pow(1f - (1.2f * impactProgress * time / duration), 6f)) / travelled), token);
                }

                await impact(token);
                token.ThrowIfCancellationRequested();
                Sequence returning = DOTween.Sequence()
                    .Join(DOTween.To(() => animationPositionValue, value => animationPositionValue = value,
                        origin, ANIMATION_ATTACK_TIME_STAGE_3).SetEase(Ease.Linear))
                    .Join(DOTween.To(() => attackScaleValue, value => attackScaleValue = value,
                        1f, ANIMATION_ATTACK_TIME_STAGE_3).SetEase(Ease.Linear));
                await animations.PlayAsync(returning, token);
                await animations.DelayAsync(ANIMATION_ATTACK_TIME_STAGE_4, token);
            }
            finally
            {
                if (rectTransform != null)
                {
                    animationPositionValue = origin;
                    attackScaleValue = 1f;
                }
            }
        }
    }
}
