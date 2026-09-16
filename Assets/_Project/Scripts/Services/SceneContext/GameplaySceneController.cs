using System;
using _Project.Scripts._Common.GameFlow;
using _Project.Scripts._Common.SceneLoader;
using _Project.Scripts.Enemies;
using _Project.Scripts.GameFlow;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Services.SceneContext
{
    public sealed class GameplaySceneController : IInitializable, IDisposable
    {
        private readonly GameplaySceneConfig _config;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingCurtain _loadingCurtain;
        private readonly GameplayBootstrap _gameplayBootstrap;
        private readonly ArenaGameMode _arenaGameMode;
        private readonly IMatchStatsService _matchStats;
        private readonly IGameTimerService _gameTimer;
        private readonly IGameFlowService _gameFlow;

        private bool _isRestarting;

        public GameplaySceneController(
            GameplaySceneConfig config,
            ISceneLoader sceneLoader,
            GameplayBootstrap gameplayBootstrap,
            ArenaGameMode arenaGameMode,
            IMatchStatsService matchStats,
            IGameTimerService gameTimer,
            IGameFlowService gameFlow,
            [InjectOptional] ILoadingCurtain loadingCurtain = null)
        {
            _config = config;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameplayBootstrap = gameplayBootstrap;
            _arenaGameMode = arenaGameMode;
            _matchStats = matchStats;
            _gameTimer = gameTimer;
            _gameFlow = gameFlow;
        }

        public void Initialize()
        {
            _gameFlow.RestartRequested += Restart;
            LoadLevelAsync().Forget();
        }

        public void Dispose()
        {
            _gameFlow.RestartRequested -= Restart;
            _gameplayBootstrap.DespawnCharacter();
            _arenaGameMode.Unbind();
        }

        private void Restart()
        {
            RestartLevelAsync().Forget();
        }

        private async UniTask LoadLevelAsync()
        {
            if (string.IsNullOrWhiteSpace(_config.LevelSceneName))
            {
                Debug.LogError("Level scene name is empty.");
                return;
            }

            if (!IsSceneLoaded(_config.LevelSceneName))
                await _sceneLoader.LoadSceneAsync(_config.LevelSceneName, LoadSceneMode.Additive);

            SetupLoadedLevel();
        }

        private async UniTask RestartLevelAsync()
        {
            if (_isRestarting)
                return;

            if (string.IsNullOrWhiteSpace(_config.LevelSceneName))
            {
                Debug.LogError("Level scene name is empty.");
                return;
            }

            _isRestarting = true;

            try
            {
                if (_loadingCurtain != null)
                    await _loadingCurtain.ShowAsync();

                TeardownLoadedLevel();

                if (IsSceneLoaded(_config.LevelSceneName))
                    await _sceneLoader.UnloadSceneAsync(_config.LevelSceneName, useCurtain: false);

                await _sceneLoader.LoadSceneAsync(_config.LevelSceneName, LoadSceneMode.Additive, useCurtain: false);
                SetupLoadedLevel();
            }
            finally
            {
                if (_loadingCurtain != null)
                    await _loadingCurtain.HideAsync();

                _isRestarting = false;
            }
        }

        private void SetupLoadedLevel()
        {
            ArenaController arenaController = UnityEngine.Object.FindFirstObjectByType<ArenaController>();
            if (arenaController == null)
                Debug.LogError($"ArenaController is not found after loading level scene: {_config.LevelSceneName}");

            _arenaGameMode.Bind(arenaController);
            _gameplayBootstrap.SpawnCharacter();
        }

        private void TeardownLoadedLevel()
        {
            _gameplayBootstrap.DespawnCharacter();
            _arenaGameMode.Unbind();
            _gameTimer.StopTimer();
            _gameTimer.ResetTimer();
            _matchStats.ClearRegistrations();
            _matchStats.Reset();
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
