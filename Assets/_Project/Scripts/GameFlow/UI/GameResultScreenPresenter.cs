using System;
using _Project.Scripts._Common.GameFlow;
using _Project.Scripts.GameFlow;
using Zenject;

namespace _Project.Scripts.GameFlow.UI
{
    public class GameResultScreenPresenter: IInitializable, IDisposable
    {
        private readonly IGameFlowService _gameFlow;
        private readonly GameResultScreenView _view;
        private readonly IMatchStatsService _matchStats;

        public GameResultScreenPresenter(
            IGameFlowService gameFlow,
            GameResultScreenView view,
            IMatchStatsService matchStats)
        {
            _gameFlow = gameFlow;
            _view = view;
            _matchStats = matchStats;
        }

        public void Initialize()
        {
            _view.Hide();
            _gameFlow.GameFinished += ShowResult;
            _view.RestartClicked += Restart;
            _view.ExitClicked += Exit;
        }

        private void ShowResult(GameResult result)
        {
            string text = result == GameResult.Victory ? "Victory" : "Defeat";
            _view.Show(text, _matchStats.GetSnapshot());
        }

        private void Restart()
        {
            _view.Hide();
            _gameFlow.Restart();
        }

        private void Exit()
        {
            UnityEngine.Application.Quit();
        }

        public void Dispose()
        {
            _gameFlow.GameFinished -= ShowResult;
            _view.RestartClicked -= Restart;
            _view.ExitClicked -= Exit;
        }
    }
}
