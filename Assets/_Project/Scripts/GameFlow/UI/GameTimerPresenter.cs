using System;
using _Project.Scripts._Common.GameFlow;
using Zenject;

namespace _Project.Scripts.GameFlow.UI
{
    public sealed class GameTimerPresenter : IInitializable, IDisposable
    {
        private readonly IGameTimerService _gameTimer;
        private readonly GameTimerView _view;

        public GameTimerPresenter(IGameTimerService gameTimer, GameTimerView view)
        {
            _gameTimer = gameTimer;
            _view = view;
        }

        public void Initialize()
        {
            _view.SetTime(_gameTimer.ElapsedTime);
            _gameTimer.TimeChanged += _view.SetTime;
        }

        public void Dispose()
        {
            _gameTimer.TimeChanged -= _view.SetTime;
        }
    }
}
