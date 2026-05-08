namespace Assets.GameData.Scripts
{
    public static class GameSceneManager
    {
        public enum SceneName { MainMenu, SelectBattlefield, AllHeroes, Collection, Battlefield, Auth }
        public static void Load(SceneName sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"{sceneName}Scene");
        }
    }
}
