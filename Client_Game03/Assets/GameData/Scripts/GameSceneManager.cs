namespace Assets.GameData.Scripts
{
    /// <summary>Загружает игровые сцены по устойчивому соответствию имён ресурсов.</summary>
    public static class GameSceneManager
    {
        /// <summary>Перечисляет доступные игровые сцены.</summary>
        public enum ESceneName
        {
            mainMenu, selectBattlefield, allHeroes, collection, battlefield, auth
        }

        /// <summary>Возвращает имя ресурса сцены независимо от имени элемента перечисления.</summary>
        public static string GetSceneName(ESceneName sceneName)
        {
            // Имена ресурсов сцен сохраняются при изменении имён элементов перечисления.
            string scene = sceneName switch
            {
                ESceneName.mainMenu => "MainMenu",
                ESceneName.selectBattlefield => "SelectBattlefield",
                ESceneName.allHeroes => "AllHeroes",
                ESceneName.collection => "Collection",
                ESceneName.battlefield => "Battlefield",
                ESceneName.auth => "Auth",
                _ => ((int)sceneName).ToString()
            };
            return $"{scene}Scene";
        }

        /// <summary>Загружает сцену по имени её ресурса.</summary>
        public static void Load(ESceneName sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GetSceneName(sceneName));
        }
    }
}
