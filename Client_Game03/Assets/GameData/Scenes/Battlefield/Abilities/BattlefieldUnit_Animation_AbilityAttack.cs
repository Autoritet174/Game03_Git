using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using static Assets.GameData.Scenes.Battlefield.BattlefieldAbilityAnimationConstants;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {
        /// <summary>Последовательно ожидает увеличение, рывок, попадание, возврат и заключительную паузу.</summary>
        public async UniTask PlayAttackAsync(BattlefieldUnit target, Func<CancellationToken, UniTask> impact, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            rectTransform.SetAsLastSibling();
            Vector2 origin = GetFormationPosition();
            try
            {
                await animations.PlayAsync(
                    DOTween.To(() => attackScaleValue, value => attackScaleValue = value, RAISED_CARD_SCALE, CARD_RAISE_DURATION)
                        .SetEase(Ease.Linear), token);

                Vector2 destination = target.GetFormationPosition();
                float distance = Vector2.Distance(origin, destination);
                if (distance > width)
                {
                    // Аналитическое время прежнего порога попадания: 1 - (1 - 1.2t)^6.
                    float remaining = width / distance;
                    float impactProgress = (1f - Mathf.Pow(remaining, 1f / 6f)) / 1.2f;
                    float travelled = 1f - remaining;
                    Vector2 contact = Vector2.LerpUnclamped(origin, destination, travelled);
                    await animations.PlayAsync(
                        DOTween.To(() => animationPositionValue, value => animationPositionValue = value,
                            contact, ATTACK_LUNGE_DURATION * impactProgress)
                            .SetEase((time, duration, overshoot, period) =>
                                (1f - Mathf.Pow(1f - (1.2f * impactProgress * time / duration), 6f)) / travelled), token);
                }

                await impact(token);
                token.ThrowIfCancellationRequested();
                Sequence returning = DOTween.Sequence()
                    .Join(DOTween.To(() => animationPositionValue, value => animationPositionValue = value,
                        origin, CARD_LOWER_DURATION).SetEase(Ease.Linear))
                    .Join(DOTween.To(() => attackScaleValue, value => attackScaleValue = value,
                        NORMAL_CARD_SCALE, CARD_LOWER_DURATION).SetEase(Ease.Linear));
                await animations.PlayAsync(returning, token);
                await animations.DelayAsync(POST_ATTACK_DELAY, token);
            }
            finally
            {
                if (rectTransform != null)
                {
                    animationPositionValue = origin;
                    attackScaleValue = NORMAL_CARD_SCALE;
                }
            }
        }
    }
}
