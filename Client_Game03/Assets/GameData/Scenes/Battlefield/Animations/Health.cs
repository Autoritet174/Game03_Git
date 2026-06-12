using Assets.GameData.Scripts;
using General;
using System;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield.Animations
{
    public class Health
    {
        public Health(GameObject gameObject)
        {
            this.gameObject = gameObject;
            rectTransform = gameObject.GetComponent<RectTransform>();
            textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.fontSize = HealthHub.FontSize * G.GetCoefHeight();
        }

        private readonly GameObject gameObject;
        private readonly RectTransform rectTransform;
        private readonly TextMeshProUGUI textMeshProUGUI;
        private DateTime dtStart;
        private DateTime dtEnd;
        private bool active = true;
        private RectTransform posParent;
        private Vector2 posEnd = Vector2.zero;

        public void Start(float value, bool isCrit, RectTransform posParent)
        {
            dtStart = DateTime.Now;
            dtEnd = dtStart.AddSeconds(HealthHub.AnimationHealthChangeTime / BattlefieldSceneInitializator.AnimationSpeed);

            string text;
            if (value < 0)
            {
                text = value.ToStr();
                textMeshProUGUI.color = Color.red;
            }
            else
            {
                text = $"+{value.ToStr()}";
                textMeshProUGUI.color = Color.green;
            }

            if (isCrit)
            {
                text = $"{text} CRIT";
            }
            textMeshProUGUI.text = text;

            this.posParent = posParent;
            //float angle = RandomShared.NextBool() ? RandomShared.NextSingle(-180, 180) : RandomShared.NextSingle(15, 90);
            posEnd = HealthHub.GetPointFromAngle(HealthHub.Distance * G.GetCoefHeight(), RandomShared.NextSingle(-180, 180));

            Active = true;
        }

        public bool Active
        {
            get => active;
            private set
            {
                active = value;
                gameObject.SetActive(value);
            }
        }

        public void Update()
        {
            if (!Active)
            {
                return;
            }
            try
            {
                float animationPercent = Math.Clamp((float)((DateTime.Now - dtStart).TotalSeconds / (dtEnd - dtStart).TotalSeconds), 0, 1);
                Vector2 posEndShift = posEnd + posParent.anchoredPosition;

                float xDist = posEndShift.x - posParent.anchoredPosition.x;
                float yDist = posEndShift.y - posParent.anchoredPosition.y;
                float x = posParent.anchoredPosition.x + (xDist * animationPercent);
                float y = posParent.anchoredPosition.y + (yDist * animationPercent);
                rectTransform.anchoredPosition = new Vector2(x * G.GetCoefHeight(), y * G.GetCoefHeight());

                if (animationPercent == 1)
                {
                    Active = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
