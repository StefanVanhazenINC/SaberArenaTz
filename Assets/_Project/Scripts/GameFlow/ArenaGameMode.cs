using System;
using _Project.Scripts._Common.GameFlow;
using _Project.Scripts.Enemies;

namespace _Project.Scripts.GameFlow
{
    public sealed class ArenaGameMode : IDisposable
    {
        private readonly IGameFlowService _gameFlow;
        private readonly IGameTimerService _gameTimer;
        private readonly IMatchStatsService _matchStats;

        private ArenaController _arenaController;

        public ArenaGameMode(
            IGameFlowService gameFlow,
            IGameTimerService gameTimer,
            IMatchStatsService matchStats)
        {
            _gameFlow = gameFlow;
            _gameTimer = gameTimer;
            _matchStats = matchStats;
        }

        public void Bind(ArenaController arenaController)
        {
            Unbind();

            _arenaController = arenaController;

            if (_arenaController == null)
                return;

            _arenaController.Started += _matchStats.Reset;
            _arenaController.Started += _gameTimer.StartTimer;
            _arenaController.EnemySpawned += _matchStats.RegisterEnemy;
            _arenaController.Completed += _gameFlow.Victory;
        }

        public void Unbind()
        {
            if (_arenaController == null)
                return;

            _arenaController.Started -= _matchStats.Reset;
            _arenaController.Started -= _gameTimer.StartTimer;
            _arenaController.EnemySpawned -= _matchStats.RegisterEnemy;
            _arenaController.Completed -= _gameFlow.Victory;
            _arenaController = null;
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}
