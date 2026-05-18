using Assets.GameData.Scripts;
using System;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {

        private static readonly double AnimationHealthChangeTime = 4;
        private int AnimationHealthChangeStage = 0;
        private DateTime AtimationHealthChangeStart = DateTime.Now;
        private DateTime AtimationHealthChangeEnd = DateTime.Now;
        public void AnimationStartHealthChange(float v, bool isCrit)
        {
            if (v == 0)
            {
                return;
            }
            string text;
            if (v < 0)
            {
                text = v.ToStr();
                _HealthChange_TextMeshProUGUI.color = Color.red;
            }
            else
            {
                text = "+" + v.ToStr();
                _HealthChange_TextMeshProUGUI.color = Color.green;
            }

            if (isCrit)
            {
                text = $"! {text} !";
            }

            _HealthChange_TextMeshProUGUI.text = text;

            _HealthChange_RectTransform.anchoredPosition = new Vector2(0, -_HealthChange_Height * G.GetCoefHeight());
            AnimationHealthChangeStage = 1;
            AtimationHealthChangeStart = DateTime.Now;
            AtimationHealthChangeEnd = AtimationHealthChangeStart.AddSeconds(AnimationHealthChangeTime / AnimationSpeed);
            _HealthChange_RectTransform.gameObject.SetActive(true);

            RefreshHealth();
        }

        public void UpdateAnimationChangeHealth()
        {
            if (AnimationHealthChangeStage == 0)
            {
                return;
            }

            float animationPercent = Math.Clamp((float)((DateTime.Now - AtimationHealthChangeStart).TotalSeconds / (AtimationHealthChangeEnd - AtimationHealthChangeStart).TotalSeconds), 0, 1);

            float yStart = -_HealthChange_Height;
            float yEnd = _HealthChange_Height;
            float yDist = yEnd - yStart;
            float y = yStart + (yDist * animationPercent);
            _HealthChange_RectTransform.anchoredPosition = new Vector2(0, y * G.GetCoefHeight());

            if (animationPercent == 1)
            {
                AnimationHealthChangeStage = 0;
                _HealthChange_RectTransform.gameObject.SetActive(false);
            }
        }
    }
}
