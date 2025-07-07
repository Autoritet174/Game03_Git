using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class GameObjectFinder
    {

        /// <summary>
        /// Возвращает игровой объект из текущей сцены по тегу, находящийся в корне сцены. 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static GameObject FindByTag(string tag) {
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                if (obj.CompareTag(tag))
                {
                    return obj;
                }
            }
            return null;
        }

    }
}
