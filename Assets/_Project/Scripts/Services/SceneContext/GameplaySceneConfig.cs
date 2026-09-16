namespace _Project.Scripts.Services.SceneContext
{
    public readonly struct GameplaySceneConfig
    {
        public readonly string LevelSceneName;

        public GameplaySceneConfig(string levelSceneName)
        {
            LevelSceneName = levelSceneName;
        }
    }
}
