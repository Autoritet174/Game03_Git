using Assets.GameData.Scripts;
using General.DTO.Battlefield;
using General.DTO.Entities.Collection;
using General.DTO.Entities.GameData;
using System.Linq;
using UnityEngine;

namespace Assets.GameData.Scenes.Battlefield
{
    public class BattlefieldUnit
    {
        private static readonly float[] yShiftArray = new float[] { -89f, 92f, -265f, 269f };
        private static readonly float xShift = 200f;
        private static readonly Vector2 vector2_05 = new(0.5f, 0.5f);

        private readonly SpawnedHero spawnedHeroes;
        private readonly GameObject gameObject;
        private readonly RectTransform rectTransform;
        private readonly bool isMyUnit;
        private int Position { get; set; }

        public BattlefieldUnit(SpawnedHero spawnedHeroes, int position, bool isMyUnit)
        {
            this.spawnedHeroes = spawnedHeroes;
            Position = position;
            this.isMyUnit = isMyUnit;

            GameObject canvasUnits = GameObjectFinder.FindByName("CanvasUnits");
            gameObject = AddressableCache.BattleFieldUnit.SafeInstant(canvasUnits.transform);
            DtoHero dtoHero = Game03Client.Collection.CollectionProvider.GetCollectionHeroesFromCache().FirstOrDefault(a => a.Id == spawnedHeroes.HeroId);
            DtoBaseHero dtoBaseHero = dtoHero.BaseHero;

            gameObject.name = $"UnitPlayer_{dtoBaseHero.Name}";

            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = vector2_05;
            rectTransform.anchorMax = vector2_05;
            rectTransform.pivot = vector2_05;

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
        }

    }
}
