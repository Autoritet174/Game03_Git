using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using static Assets.GameData.Scenes.Battlefield.BattlefieldAbilityAnimationConstants;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {
        /// <summary>Увеличивает карточку целителя, показывает исцеление, выдерживает паузу и возвращает исходный масштаб.</summary>
        public async UniTask PlayHealingAsync(Func<CancellationToken, UniTask> impact, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rectTransform.SetAsLastSibling();
            try
            {
                await animations.PlayAsync(
                    DOTween.To(() => attackScaleValue, value => attackScaleValue = value,
                        RAISED_CARD_SCALE, CARD_RAISE_DURATION).SetEase(Ease.Linear), cancellationToken);

                await impact(cancellationToken);
                await animations.DelayAsync(HEALING_HOLD_DURATION, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                await animations.PlayAsync(
                    DOTween.To(() => attackScaleValue, value => attackScaleValue = value,
                        NORMAL_CARD_SCALE, CARD_LOWER_DURATION).SetEase(Ease.Linear), cancellationToken);
            }
            finally
            {
                if (rectTransform != null)
                {
                    attackScaleValue = NORMAL_CARD_SCALE;
                }
            }
        }
    }
}
