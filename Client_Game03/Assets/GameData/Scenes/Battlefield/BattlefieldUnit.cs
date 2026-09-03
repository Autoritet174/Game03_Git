using Assets.GameData.Scripts;
using General;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;
using LM = Game03Client.LocalizationManager;

namespace Assets.GameData.Scenes.Battlefield
{
    public partial class BattlefieldUnit
    {
        private static readonly Color colorStrength = new(224f / 255f, 0, 0);
        private static readonly Color colorAgility = new(0, 239f / 255f, 17f / 255f);
        private static readonly Color colorIntelligence = new(0, 160f / 255f, 255f / 255f);
        private static readonly Color colorUniversal = Color.white;

        private static readonly float _ScaleAlive = 0.85f;
        private static readonly float _ScaleDead = _ScaleAlive * 0.65f;
        private static readonly float _Width = 150;
        private static readonly float _Height = 200;

        private static readonly float yShift1 = _Height * 0.6f * _ScaleAlive;
        private static readonly float yShift = 40;
        private static readonly float yShift2 = yShift1 * 3;
        private static readonly float[] yShiftArray = new float[] {
            -yShift2 + yShift,//1
            -yShift1 + yShift,//2
            yShift1 + yShift,//3
            yShift2 + yShift,//4
        };
        private static readonly float xShift = 200f * _ScaleAlive;

        private readonly RectTransform _RectTransform;


        //private readonly RectTransform _Health__RectTransform;
        //private readonly RectTransform _HealthImagePercent__RectTransform;
        //private readonly RectTransform _HealthImageGreenBar__RectTransform;

        private static readonly float _Health_Height = 30;
        //private readonly TextMeshProUGUI _HealthText_TextMeshProUGUI;
        //private readonly RectTransform _HealthText__RectTransform;

        private readonly RectTransform _ImageHeroMask__RectTransform;

        private readonly RectTransform _HealthImageStat__RectTransform;

        private readonly RectTransform _Level_RectTransform;

        private readonly TextMeshProUGUI _LevelText_TextMeshProUGUI;

        //private static readonly float _HealthChange_Height = 25;
        //private readonly RectTransform _HealthChange_RectTransform;
        //private readonly TextMeshProUGUI _HealthChange_TextMeshProUGUI;

        private readonly RectTransform _ActionPoints_RectTransform;
        private readonly RectTransform _ActionPointsImage_RectTransform;
        private readonly RectTransform _ActionPointsText_RectTransform;
        private readonly TextMeshProUGUI _ActionPointsText_TextMeshProUGUI;

        private readonly GameObject _ImageDead_GameObject;
        private readonly RectTransform _ImageDead_RectTransform;

        private readonly ProgressBar__prefab__script progressBar;

        public SpawnedHero SpawnedHero { get; }

        private readonly bool _IsMyUnit;
        private readonly int _Position;
        private readonly string textDead = "Dead";

        private readonly PanelDamage__script panelDamage__script;

        public BattlefieldUnit(SpawnedHero spawnedHeroes,
            int position, bool isMyUnit, Transform canvasUnits__Transform,
            Animations.HealthHub healthHub, PanelDamage__script panelDamage__script)
        {
            SpawnedHero = spawnedHeroes;
            _Position = position;
            _IsMyUnit = isMyUnit;
            this.healthHub = healthHub;
            this.panelDamage__script = panelDamage__script;

            GameObject gameObject = AddressablePrefabProvider.BattlefieldUnit.SafeInstant(canvasUnits__Transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.baseHeroes.First(a => a.id == spawnedHeroes.baseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.name}";

            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchorMin = new(0.5f, 0.5f);
            _RectTransform.anchorMax = new(0.5f, 0.5f);
            _RectTransform.pivot = new(0.5f, 0.5f);
            _RectTransform.localScale = new Vector3(_ScaleAlive, _ScaleAlive, 1);


            Image _ImageRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", gameObject.transform);
            _ImageRarity_Image.sprite = AddressablePrefabProvider.GetRarity(dtoBaseHero.rarity);
            _ImageRarity_Image.preserveAspect = true;
            _ImageRarity_Image.type = Image.Type.Simple;


            _ImageHeroMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageHeroMask", gameObject.transform);

            Image _ImageHero_Image = GameObjectFinder.FindByName<Image>("ImageHero", gameObject.transform);
            _ImageHero_Image.sprite = AddressablePrefabProvider.Heroes[$"{dtoBaseHero.name}_face"];
            _ImageHero_Image.preserveAspect = true;
            _ImageHero_Image.type = Image.Type.Simple;


            progressBar = GameObjectFinder.FindByName<ProgressBar__prefab__script>("ProgressBar__prefab", gameObject.transform);
            progressBar.SetTextRightOffsetRight(20);
            progressBar.Initialize();
            textDead = LM.GetValue(L.UI.Label.Dead).ToUpperInvariant();
            //_Health__RectTransform = GameObjectFinder.FindByName<RectTransform>("Health", gameObject.transform);
            //_HealthImagePercent__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImagePercent", gameObject.transform);
            //_HealthImageGreenBar__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageGreenBar", gameObject.transform);
            //_HealthText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("HealthText", gameObject.transform);
            //_HealthText__RectTransform = _HealthText_TextMeshProUGUI.GetComponent<RectTransform>();


            _HealthImageStat__RectTransform = GameObjectFinder.FindByName<RectTransform>("HealthImageStat", gameObject.transform);


            _Level_RectTransform = GameObjectFinder.FindByName<RectTransform>("Level", gameObject.transform);
            _LevelText_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LevelText", gameObject.transform);
            _LevelText_TextMeshProUGUI.SetText(spawnedHeroes.level.ToString());
            Image _Level_Image = _Level_RectTransform.GetComponent<Image>();
            _Level_Image.color = dtoBaseHero.mainStat switch
            {
                EMainStat.strength => colorStrength,
                EMainStat.agility => colorAgility,
                EMainStat.intelligence => colorIntelligence,
                EMainStat.universal => colorUniversal,
                _ => throw new NotImplementedException()
            };

            _ActionPoints_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPoints", gameObject.transform);
            _ActionPointsImage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsImage", gameObject.transform);
            _ActionPointsText_RectTransform = GameObjectFinder.FindByName<RectTransform>("ActionPointsText", gameObject.transform);
            _ActionPointsText_TextMeshProUGUI = _ActionPointsText_RectTransform.GetComponent<TextMeshProUGUI>();

            _ImageDead_GameObject = GameObjectFinder.FindByName("ImageDead", gameObject.transform);
            _ImageDead_RectTransform = _ImageDead_GameObject.GetComponent<RectTransform>();

            OnResize();

            RefreshHealth();
            RefreshActionPoints(SpawnedHero.actionPoints);
        }


        public void OnResize()
        {
            float coefHeight = G.GetCoefHeight();

            float text_Width = 130 * coefHeight;
            float text_Height = _Health_Height * coefHeight;
            float miniIconStat_X = -2 * coefHeight;
            float miniIconStat_Size = 17 * coefHeight;


            _RectTransform.anchoredPosition = GetCoords();
            _RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Height * coefHeight);

            float imageHeroMask_Padding = 10 * coefHeight;
            _ImageHeroMask__RectTransform.offsetMin = new(imageHeroMask_Padding, imageHeroMask_Padding);
            _ImageHeroMask__RectTransform.offsetMax = new(-imageHeroMask_Padding, -imageHeroMask_Padding);


            //_Health__RectTransform.sizeDelta = new Vector2(0, text_Height);
            //_Health__RectTransform.anchoredPosition = new Vector2(0, -3 * coefHeight);
            //_HealthImageGreenBar__RectTransform.sizeDelta = new Vector2(_Width * coefHeight, text_Height);

            Vector2 miniIconStat_Size_Vector2 = new(miniIconStat_Size, miniIconStat_Size);
            Vector2 miniIconStat_X_Vector2 = new(miniIconStat_X, 0);

            _HealthImageStat__RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            _HealthImageStat__RectTransform.anchoredPosition = miniIconStat_X_Vector2;

            //_HealthText__RectTransform.sizeDelta = new Vector2(text_Width, text_Height);
            //_HealthText_TextMeshProUGUI.fontSize = 22 * coefHeight;


            _Level_RectTransform.sizeDelta = new Vector2(50 * coefHeight, 25 * coefHeight);
            _LevelText_TextMeshProUGUI.fontSize = 22 * coefHeight;


            RefreshHealth();


            _ActionPoints_RectTransform.anchoredPosition = new Vector2(0, text_Height);
            _ActionPoints_RectTransform.sizeDelta = new Vector2(0, text_Height);
            _ActionPointsImage_RectTransform.sizeDelta = miniIconStat_Size_Vector2;
            _ActionPointsImage_RectTransform.anchoredPosition = miniIconStat_X_Vector2;
            _ActionPointsText_RectTransform.sizeDelta = new Vector2(text_Width, text_Height);
            _ActionPointsText_TextMeshProUGUI.fontSize = 22 * coefHeight;

            float imageDead = 115 * coefHeight;
            _ImageDead_RectTransform.sizeDelta = new Vector2(imageDead, imageDead);
        }

        /// <summary> Изменение текста и полоски здоровья. </summary>
        public void RefreshHealth()
        {
            if (SpawnedHero.health > 0)
            {
                progressBar.SetTextRight(SpawnedHero.health.ToStr());
                _ImageDead_GameObject.SetActive(false);
                _RectTransform.localScale = new Vector3(_ScaleAlive, _ScaleAlive, _ScaleAlive);
            }
            else
            {
                progressBar.SetTextRight(textDead);
                _ImageDead_GameObject.SetActive(true);
                _RectTransform.localScale = new Vector3(_ScaleDead, _ScaleDead, _ScaleDead);
            }
            progressBar.healthBar = SpawnedHero.health;
            progressBar.valueMax = SpawnedHero.healthMax;
            progressBar.Refresh(ProgressBar__prefab__script.DisplayMode.HealthBar);
        }

        public void RefreshActionPoints(int ap)
        {
            SpawnedHero.actionPoints = ap;
            _ActionPointsText_TextMeshProUGUI.text = ap.ToString();
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

        public static float AnimationShiftPower(float x, float p = 6f)
        {
            x = Math.Clamp(x, 0f, 1f);
            return 1f - MathF.Pow(1f - x, p);
        }


    }
}
