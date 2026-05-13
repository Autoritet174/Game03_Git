using Assets.GameData.Scripts;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Battlefield
{
    public class BattlefieldUnit
    {
        private static readonly Color colorStrength = new(224f / 255f, 0, 0);
        private static readonly Color colorAgility = new(0, 239f / 255f, 17f / 255f);
        private static readonly Color colorIntelligence = new(0, 160f / 255f, 255f / 255f);
        private static readonly Color colorUniversal = Color.white;

        private static readonly float _Scale = 0.9f;
        private static readonly float _Width = 150;
        private static readonly float _Height = 200;
        private static readonly float[] yShiftArray = new float[] {
            -_Height / 2/0.8f* _Scale,
            _Height / 2 / 0.8f* _Scale,
            -_Height * 3 / 2 / 0.8f* _Scale,
            _Height * 3 / 2 / 0.8f* _Scale
        };
        private static readonly float xShift = 200 / 0.8f* _Scale;

        private readonly RectTransform _RectTransform;


        private readonly RectTransform _Health__RectTransform;
        private readonly RectTransform _HealthImagePercent__RectTransform;
        private static readonly float _HealthImagePercent_Right = 1;
        private readonly RectTransform _HealthImageGreenBar__RectTransform;

        private static readonly float _Health_Height = 30;
        private static readonly float _HealthText_Width = 130;
        private static readonly float _HealthText_Y = -3;
        private static readonly float _HealthText_FontSize = 22;
        private readonly TextMeshProUGUI _HealthText_TextMeshProUGUI;
        private readonly RectTransform _HealthText__RectTransform;


        private static readonly float _ImageHeroMask_Padding = 5;
        private readonly RectTransform _ImageHeroMask__RectTransform;


        private static readonly float _HealthImageStat_X = -2;
        private static readonly float _HealthImageStat_Size = 17;
        private readonly RectTransform _HealthImageStat__RectTransform;


        private static readonly float _Level_Width = 50;
        private static readonly float _Level_Height = 25;
        private readonly RectTransform _Level_RectTransform;
        private static readonly float _Level_FontSize = 22;
        private readonly TextMeshProUGUI _LevelText_TextMeshProUGUI;

        private static readonly float _HealthChange_Height = 25;
        private static readonly float _HealthChange_FontSize = 60;
        private readonly RectTransform _HealthChange_RectTransform;
        private readonly TextMeshProUGUI _HealthChange_TextMeshProUGUI;


        private readonly SpawnedHero _SpawnedHeroes;

        private readonly bool _IsMyUnit;
        private readonly int _Position;
        

        public BattlefieldUnit(SpawnedHero spawnedHeroes, int position, bool isMyUnit, Transform canvasUnits__Transform)
        {
            _SpawnedHeroes = spawnedHeroes;
            _Position = position;
            _IsMyUnit = isMyUnit;

            GameObject gameObject = AddressableCache.BattleFieldUnit.SafeInstant(canvasUnits__Transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.BaseHeroes.First(a => a.Id == spawnedHeroes.BaseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.Name}";

            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchorMin = new(0.5f, 0.5f);
            _RectTransform.anchorMax = new(0.5f, 0.5f);
            _RectTransform.pivot = new(0.5f, 0.5f);
            _RectTransform.localScale = new Vector3(_Scale, _Scale, 1);


            Image _ImageRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", gameObject.transform);
            _ImageRarity_Image.sprite = AddressableCache.GetRarity(dtoBaseHero.Rarity);
            _ImageRarity_Image.preserveAspect = true;
            _ImageRarity_Image.type = Image.Type.Simple;


            _ImageHeroMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageHeroMask", gameObject.transform);

            Image _ImageHero_Image = GameObjectFinder.FindByName<Image>("ImageHero", gameObject.transform);
            _ImageHero_Image.sprite = AddressableCache.Heroes[$"{dtoBaseHero.Name}_face"];
            _ImageHero_Image.preserveAspect = true;
            _ImageHero_Image.type = Image.Type.Simple;


            _Health__RectTransform = GameObjectFinder.FindByName<RectTransform>("Health", gameObject.transform);
            _HealthImagePercent__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImagePercent", gameObject.transform);
            _HealthImageGreenBar__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageGreenBar", gameObject.transform);
            _HealthText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("HealthText", gameObject.transform);
            _HealthText__RectTransform = _HealthText_TextMeshProUGUI.GetComponent<RectTransform>();


            _HealthImageStat__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageStat", gameObject.transform);


            _Level_RectTransform = GameObjectFinder.FindByName<RectTransform>("Level", gameObject.transform);
            _LevelText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LevelText", gameObject.transform);
            _LevelText_TextMeshProUGUI.SetText(spawnedHeroes.Level.ToString());
            Image _Level_Image = _Level_RectTransform.GetComponent<Image>();
            _Level_Image.color = dtoBaseHero.MainStat switch
            {
                General.EMainStat.Strength => colorStrength,
                General.EMainStat.Agility => colorAgility,
                General.EMainStat.Intelligence => colorIntelligence,
                General.EMainStat.Universal => colorUniversal,
                _ => throw new System.NotImplementedException()
            };


            _HealthChange_RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthChange", gameObject.transform);
            _HealthChange_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("HealthChange", gameObject.transform);
            _HealthChange_RectTransform.gameObject.SetActive(false);
            OnResize();

            RefreshHealth(_SpawnedHeroes.Health);
        }

        public void RefreshHealth(float newHealthValue)
        {
            _SpawnedHeroes.Health = newHealthValue;
            _HealthText_TextMeshProUGUI.SetText(_SpawnedHeroes.Health.ToStr());

            float coefHeight = G.GetCoefHeight();
            float width = (_Width - (_HealthImagePercent_Right * 2)) * _SpawnedHeroes.HealthPercent;
            _HealthImagePercent__RectTransform.sizeDelta = new Vector2(width, _Health_Height * coefHeight);
        }

        public Vector2 GetCoords()
        {
            float coefHeight = G.GetCoefHeight();
            float x = xShift * ((_Position / 4) + 1);
            if (_IsMyUnit)
            {
                x = -x;
            }
            float y = yShiftArray[_Position % 4];
            return new Vector2(x * coefHeight, y * coefHeight);
        }

        public void OnResize()
        {
            float coefHeight = G.GetCoefHeight();
            _RectTransform.anchoredPosition = GetCoords();
            _RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Height * coefHeight);


            _ImageHeroMask__RectTransform.offsetMin = new Vector2(_ImageHeroMask_Padding, _ImageHeroMask_Padding);
            _ImageHeroMask__RectTransform.offsetMax = new Vector2(-_ImageHeroMask_Padding, -_ImageHeroMask_Padding);


            _Health__RectTransform.sizeDelta = new Vector2(0, _Health_Height * coefHeight);
            _Health__RectTransform.anchoredPosition = new Vector2(0, _HealthText_Y * coefHeight);
            _HealthImageGreenBar__RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Health_Height * coefHeight);
            _HealthImageStat__RectTransform.sizeDelta = new Vector2(_HealthImageStat_Size * coefHeight, _HealthImageStat_Size * coefHeight);
            _HealthImageStat__RectTransform.anchoredPosition = new Vector2(_HealthImageStat_X * coefHeight, 0);


            _HealthText__RectTransform.sizeDelta = new Vector2(_HealthText_Width * coefHeight, _Health_Height * coefHeight);
            _HealthText_TextMeshProUGUI.fontSize = _HealthText_FontSize * coefHeight;


            _Level_RectTransform.sizeDelta = new Vector2(_Level_Width * coefHeight, _Level_Height * coefHeight);
            _LevelText_TextMeshProUGUI.fontSize = _Level_FontSize * coefHeight;


            RefreshHealth(_SpawnedHeroes.Health);

            _HealthChange_TextMeshProUGUI.fontSize = _HealthChange_FontSize * coefHeight;
            _HealthChange_RectTransform.anchoredPosition = new Vector2(0, -_HealthChange_Height * coefHeight);
            _HealthChange_RectTransform.sizeDelta = new Vector2(0, _HealthChange_Height * coefHeight);
        }



        private static readonly double AnimationSpeed = 1;
        private static readonly double AnimationAttackTimeStage1 = 0.2;
        private static readonly double AnimationAttackTimeStage2 = 0.3;
        private static readonly double AnimationAttackTimeStage3 = 0.3;
        public int AnimationAttackStage { get; private set; } = 0;
        private DateTime AtimationAttackStart = DateTime.Now;
        private DateTime AtimationAttackEnd = DateTime.Now;
        private BattlefieldUnit AtimationAttackUnitTarget;
        private Vector2 AtimationAttackPosEnd = Vector2.zero;
        public static float ShiftPower(float x, float p = 6f)
        {
            x = Math.Clamp(x, 0f, 1f);
            return 1f - MathF.Pow(1f - x, p);
        }
        public void AnimationStartAttackUnit(BattlefieldUnit unitTarget)
        {
            AtimationAttackUnitTarget = unitTarget;
            AnimationAttackStage = 1;
            AtimationAttackStart = DateTime.Now;
            AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage1 / AnimationSpeed);
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
                float coef = (1f + (0.3f * animationPercent))* _Scale;
                _RectTransform.localScale = new(coef, coef, 1);
                if (animationPercent == 1)
                {
                    AnimationAttackStage = 2;
                    AtimationAttackStart = DateTime.Now;
                    AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage2 / AnimationSpeed);
                }
            }
            else if (AnimationAttackStage == 2) // движение от базовой точки до цели
            {
                float animationPercentForPos = ShiftPower(animationPercent*1.2f);


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
                    AtimationAttackEnd = AtimationAttackStart.AddSeconds(AnimationAttackTimeStage3 / AnimationSpeed);
                    AtimationAttackUnitTarget.AnimationStartHealthChange(-RandomShared.NextInclusive(10, 99));
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

                float coef = (1f + (0.3f * (1-animationPercent)))* _Scale;
                _RectTransform.localScale = new(coef, coef, 1);
                if (animationPercent == 1)
                {
                    AnimationAttackStage = 0;
                }
            }

        }

        private static readonly double AnimationHealthChangeTime = 2;
        private int AnimationHealthChangeStage = 0;
        private DateTime AtimationHealthChangeStart = DateTime.Now;
        private DateTime AtimationHealthChangeEnd = DateTime.Now;
        public void AnimationStartHealthChange(float v)
        {
            if (v == 0)
            {
                return;
            }
            if (v < 0)
            {
                _HealthChange_TextMeshProUGUI.text = v.ToStr();
                _HealthChange_TextMeshProUGUI.color = Color.red;
            }
            else
            {
                _HealthChange_TextMeshProUGUI.text = "+"+v.ToStr();
                _HealthChange_TextMeshProUGUI.color = Color.green;
            }

            _HealthChange_RectTransform.anchoredPosition = new Vector2(0, -_HealthChange_Height * G.GetCoefHeight());
            AnimationHealthChangeStage = 1;
            AtimationHealthChangeStart = DateTime.Now;
            AtimationHealthChangeEnd = AtimationHealthChangeStart.AddSeconds(AnimationHealthChangeTime / AnimationSpeed);
            _HealthChange_RectTransform.gameObject.SetActive(true);

            RefreshHealth(_SpawnedHeroes.Health + v);
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
