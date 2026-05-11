using Assets.GameData.Scripts;
using Game03Client.Collection;
using General.DTO.Battlefield;
using General.DTO.Entities.Collection;
using General.DTO.Entities.GameData;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.Battlefield
{
    public class BattlefieldUnit
    {
        private static readonly float width = 153f;
        private static readonly float height = 220f;
        private static readonly float[] yShiftArray = new float[] { -height / 2, height / 2, -height * 3 / 2, -height * 3 / 2 };
        private static readonly float xShift = 200f;
        private static readonly Vector2 vector2_05 = new(0.5f, 0.5f);
        private static readonly float textHealthFontSize = 25f;

        private readonly SpawnedHero spawnedHeroes;
        private readonly RectTransform rectTransform;
        private readonly TextMeshProUGUI textHealth_TextMeshProUGUI;
        private readonly bool isMyUnit;
        private int Position { get; set; }

        public BattlefieldUnit(SpawnedHero spawnedHeroes, int position, bool isMyUnit)
        {
            this.spawnedHeroes = spawnedHeroes;
            Position = position;
            this.isMyUnit = isMyUnit;

            GameObject canvasUnits = GameObjectFinder.FindByName("CanvasUnits");
            GameObject gameObject = AddressableCache.BattleFieldUnit.SafeInstant(canvasUnits.transform);
            BaseHero dtoBaseHero = Game03Client.GameData.Container.BaseHeroes.First(a=>a.Id == spawnedHeroes.BaseHeroId);

            gameObject.name = $"Unit{(isMyUnit ? "Player" : "Enemy")}_{dtoBaseHero.Name}";

            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = vector2_05;
            rectTransform.anchorMax = vector2_05;
            rectTransform.pivot = vector2_05;
            rectTransform.localScale = new Vector3(0.8f, 0.8f, 1);

            {
                Image rarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity", gameObject.transform);
                rarity_Image.sprite = AddressableCache.GetRarity(dtoBaseHero.Rarity);
                rarity_Image.preserveAspect = true;
                rarity_Image.type = Image.Type.Simple;
            }
            {
                Image hero_Image = GameObjectFinder.FindByName<Image>("ImageHero", gameObject.transform);
                hero_Image.sprite = AddressableCache.Heroes[$"{dtoBaseHero.Name}_face"];
                hero_Image.preserveAspect = true;
                hero_Image.type = Image.Type.Simple;
            }
            {
                textHealth_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("TextHealth", gameObject.transform);
                textHealth_TextMeshProUGUI.SetText(spawnedHeroes.Health.ToStr());
            }

            OnResize();
        }


        public void OnResize()
        {
            float x = xShift * ((Position / 4) + 1);
            if (isMyUnit)
            {
                x = -x;
            }

            float y = yShiftArray[Position % 4];
            float coefHeight = G.GetCoefHeight();
            rectTransform.anchoredPosition = new Vector2(x * coefHeight, y * coefHeight);
            rectTransform.sizeDelta = new Vector2(width * coefHeight, height * coefHeight);

            textHealth_TextMeshProUGUI.fontSize = textHealthFontSize * coefHeight;
        }

    }
}
