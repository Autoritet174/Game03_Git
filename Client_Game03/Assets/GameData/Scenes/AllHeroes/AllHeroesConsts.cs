using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameData.Scenes.AllHeroes
{
    internal static class AllHeroesConsts
    {
        internal const string HERO_IMAGE_NULL = "hero-image-null";

        private static readonly Color[] colorsAnimation = new Color[9];

        static AllHeroesConsts()
        {
            int div = colorsAnimation.Length + 1;
            for (int i = 0; i < colorsAnimation.Length; i++)
            {
                colorsAnimation[i] = new Color(1f, 1f, 1f, 1f * (i + 1) / div);
            }
            colorsAnimation[^1] = Color.white;
        }

        internal static async UniTask RunAnimationImage(Image image, float milliseconds = 1000)
        {
            try
            {
                int delay_ms = (int)(milliseconds / (colorsAnimation.Length + 1));

                for (int i = 0; i < colorsAnimation.Length; i++)
                {
                    image.color = colorsAnimation[i];
                    await UniTask.Delay(delay_ms);
                }
                image.color = Color.white;
            }
            catch
            {
                try
                {
                    image.color = Color.white;
                }
                catch { }
            }
        }
    }
}
