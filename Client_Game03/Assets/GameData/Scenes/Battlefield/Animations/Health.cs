using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield.Animations
{
    /// <summary>Одно число изменения здоровья, возвращаемое в пул после завершения DOTween-анимации.</summary>
    public class Health
    {
        /// <summary>Настройки показа чисел.</summary>
        private readonly HealthHub healthHub;

        /// <summary>Проигрыватель времени текущего боя.</summary>
        private readonly BattlefieldAnimationPlayer animations;

        /// <summary>Переиспользуемый объект числа.</summary>
        private readonly GameObject gameObject;

        /// <summary>Область числа на Canvas.</summary>
        private readonly RectTransform rectTransform;

        /// <summary>Текст и цвет числа.</summary>
        private readonly TextMeshProUGUI text;

        /// <summary>Карточка, за которой следует число.</summary>
        private RectTransform parent;

        /// <summary>Текущее смещение от карточки в координатах базового разрешения.</summary>
        private Vector2 offset;

        /// <summary>Признак занятости объекта анимацией.</summary>
        public bool Active { get; private set; }

        /// <summary>Сохраняет ссылки на компоненты переиспользуемого числа.</summary>
        public Health(GameObject gameObject, HealthHub healthHub, BattlefieldAnimationPlayer animations)
        {
            this.gameObject = gameObject;
            this.healthHub = healthHub;
            this.animations = animations;
            rectTransform = gameObject.GetComponent<RectTransform>();
            text = gameObject.GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        /// <summary>Показывает значение, ожидает линейный разлёт и освобождает объект даже при отмене.</summary>
        public async UniTask PlayAsync(float value, bool isCrit, RectTransform parent, Vector2 destination, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            this.parent = parent;
            offset = Vector2.zero;
            text.color = value < 0 ? Color.red : Color.green;
            text.text = (value < 0 ? value.ToStr() : $"+{value.ToStr()}") + (isCrit ? " CRIT" : "");
            Active = true;
            gameObject.SetActive(true);
            OnResize();
            try
            {
                await animations.PlayAsync(
                    DOTween.To(() => offset, value => offset = value, destination, healthHub.AnimationHealthChangeTime)
                        .SetEase(Ease.Linear).OnUpdate(RefreshPosition), token);
            }
            finally
            {
                Active = false;
                this.parent = null;
                if (gameObject != null)
                    gameObject.SetActive(false);
            }
        }

        /// <summary>Обновляет размер шрифта и позицию при изменении разрешения.</summary>
        public void OnResize()
        {
            text.fontSize = healthHub.FontSize * G.GetCoefHeight();
            RefreshPosition();
        }

        /// <summary>Привязывает рассчитанное DOTween смещение к движущейся карточке без собственного расчёта времени.</summary>
        private void RefreshPosition()
        {
            if (parent != null && rectTransform != null)
            {
                Vector3 origin = rectTransform.parent.InverseTransformPoint(parent.position);
                rectTransform.localPosition = origin + (Vector3)(offset * G.GetCoefHeight());
            }
        }
    }
}
