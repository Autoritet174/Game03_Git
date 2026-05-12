using Assets.GameData.Scripts;
using General.DTO.Battlefield;
using General.DTO.Entities.GameData;
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

        private static readonly float _Width = 150;
        private static readonly float _Height = 200;
        private static readonly float[] yShiftArray = new float[] { -_Height / 2/0.8f, _Height / 2 / 0.8f, -_Height * 3 / 2 / 0.8f, _Height * 3 / 2 / 0.8f };
        private static readonly float xShift = 200 / 0.8f;

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

        private readonly SpawnedHero _SpawnedHeroes;


        private readonly bool _IsMyUnit;

        private int Position { get; set; }

        public BattlefieldUnit(SpawnedHero spawnedHeroes, int position, bool isMyUnit, Transform canvasUnits__Transform)
        {
            _SpawnedHeroes = spawnedHeroes;
            Position = position;
            _IsMyUnit = isMyUnit;

            GameObject gameObject = AddressableCache.BattleFieldUnit.SafeInstant(canvasUnits__Transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.BaseHeroes.First(a => a.Id == spawnedHeroes.BaseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.Name}";

            _RectTransform = gameObject.GetComponent<RectTransform>();
            _RectTransform.anchorMin = new(0.5f, 0.5f);
            _RectTransform.anchorMax = new(0.5f, 0.5f);
            _RectTransform.pivot = new(0.5f, 0.5f);
            //_RectTransform.localScale = new Vector3(1f, 1f, 1);


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
            _HealthText_TextMeshProUGUI.SetText(spawnedHeroes.Health.ToStr());
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

            OnResize();
        }


        public void OnResize()
        {
            float x = xShift * ((Position / 4) + 1);
            if (_IsMyUnit)
            {
                x = -x;
            }

            float y = yShiftArray[Position % 4];
            float coefHeight = G.GetCoefHeight();
            _RectTransform.anchoredPosition = new Vector2(x * coefHeight, y * coefHeight);
            _RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Height * coefHeight);


            _ImageHeroMask__RectTransform.offsetMin = new Vector2(_ImageHeroMask_Padding, _ImageHeroMask_Padding);
            _ImageHeroMask__RectTransform.offsetMax = new Vector2(-_ImageHeroMask_Padding, -_ImageHeroMask_Padding);


            _Health__RectTransform.sizeDelta = new Vector2(0, _Health_Height * coefHeight);
            _Health__RectTransform.anchoredPosition = new Vector2(0, _HealthText_Y * coefHeight);
            RefreshHealthPercent();
            _HealthImageGreenBar__RectTransform.sizeDelta = new Vector2(_Width * coefHeight, _Health_Height * coefHeight);
            _HealthImageStat__RectTransform.sizeDelta = new Vector2(_HealthImageStat_Size * coefHeight, _HealthImageStat_Size * coefHeight);
            _HealthImageStat__RectTransform.anchoredPosition = new Vector2(_HealthImageStat_X * coefHeight, 0);


            _HealthText__RectTransform.sizeDelta = new Vector2(_HealthText_Width * coefHeight, _Health_Height * coefHeight);
            _HealthText_TextMeshProUGUI.fontSize = _HealthText_FontSize * coefHeight;


            _Level_RectTransform.sizeDelta = new Vector2(_Level_Width * coefHeight, _Level_Height * coefHeight);
            _LevelText_TextMeshProUGUI.fontSize = _Level_FontSize * coefHeight;
        }

        public void RefreshHealthPercent()
        {
            float coefHeight = G.GetCoefHeight();
            float width = (_Width - (_HealthImagePercent_Right * 2)) * _SpawnedHeroes.HealthPercent;
            _HealthImagePercent__RectTransform.sizeDelta = new Vector2(width, _Health_Height * coefHeight);
            Debug.Log(_SpawnedHeroes.HealthPercent);
        }
    }
}
