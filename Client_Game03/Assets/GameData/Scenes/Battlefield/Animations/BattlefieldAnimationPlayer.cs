using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.GameData.Scenes.Battlefield.Animations
{
    /// <summary>Ожидает завершения DOTween через обратные вызовы и управляет анимациями одного боя.</summary>
    public sealed class BattlefieldAnimationPlayer : IDisposable
    {
        /// <summary>Активные корневые анимации, включая последовательности.</summary>
        private readonly Dictionary<Tween, UniTaskCompletionSource> tweens = new();

        /// <summary>Множитель скорости только этого боя.</summary>
        private float speed;

        /// <summary>Признак освобождения проигрывателя.</summary>
        private bool disposed;

        /// <summary>Создаёт проигрыватель с заданной скоростью.</summary>
        public BattlefieldAnimationPlayer(float speed)
        {
            SetSpeed(speed);
        }

        /// <summary>Меняет скорость текущих и последующих анимаций без изменения глобального времени Unity.</summary>
        public void SetSpeed(float value)
        {
            if (value <= 0 || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            speed = value;
            foreach (Tween tween in tweens.Keys)
            {
                tween.timeScale = speed;
            }
        }

        /// <summary>Запускает новую анимацию и ожидает её завершения без покадрового опроса; отмена уничтожает tween.</summary>
        public async UniTask PlayAsync(Tween tween, CancellationToken cancellationToken)
        {
            if (tween == null)
            {
                throw new ArgumentNullException(nameof(tween));
            }

            _ = tween.SetId(tween);
            if (disposed || cancellationToken.IsCancellationRequested)
            {
                _ = DOTween.Kill(tween, false);
                cancellationToken.ThrowIfCancellationRequested();
                throw new ObjectDisposedException(nameof(BattlefieldAnimationPlayer));
            }

            UniTaskCompletionSource completion = new();
            bool completed = false;
            _ = tween.SetAutoKill(true).SetRecyclable(false).SetUpdate(true);
            tween.timeScale = speed;
            tween.onComplete += () => completed = true;
            tween.onKill += () =>
            {
                _ = tweens.Remove(tween);
                if (cancellationToken.IsCancellationRequested || disposed)
                {
                    _ = completion.TrySetCanceled(cancellationToken);
                }
                else if (completed)
                {
                    _ = completion.TrySetResult();
                }
                else
                {
                    _ = completion.TrySetException(new InvalidOperationException("Анимация боя прервана до завершения."));
                }
            };
            tweens.Add(tween, completion);

            // Токен сцены отменяется в главном потоке Unity, где допустимо уничтожать tween.
            using (cancellationToken.Register(() =>
            {
                _ = DOTween.Kill(tween, false);
                // DOTween может отложить OnKill до своего следующего обновления.
                _ = completion.TrySetCanceled(cancellationToken);
            }))
            {
                try
                {
                    _ = tween.Play();
                    await completion.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                finally
                {
                    _ = tweens.Remove(tween);
                }
            }
        }

        /// <summary>Ожидает паузу, которая подчиняется скорости боя и отмене сцены.</summary>
        public UniTask DelayAsync(float seconds, CancellationToken cancellationToken)
        {
            return PlayAsync(DOTween.Sequence().AppendInterval(seconds), cancellationToken);
        }

        /// <summary>Останавливает принадлежащие этому бою анимации и освобождает их ожидания.</summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            foreach (KeyValuePair<Tween, UniTaskCompletionSource> pair in new List<KeyValuePair<Tween, UniTaskCompletionSource>>(tweens))
            {
                _ = DOTween.Kill(pair.Key, false);
                _ = pair.Value.TrySetCanceled();
            }
            tweens.Clear();
        }
    }
}
