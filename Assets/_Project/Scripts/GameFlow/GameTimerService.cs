using System;
using _Project.Scripts._Common.GameFlow;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.GameFlow
{
    public sealed class GameTimerService : IGameTimerService, ITickable, IDisposable
    {
        private readonly IGameFlowService _gameFlow;

        public event Action<float> TimeChanged = delegate { };

        public float ElapsedTime { get; private set; }
        public bool IsRunning { get; private set; }

        public GameTimerService(IGameFlowService gameFlow)
        {
            _gameFlow = gameFlow;
            _gameFlow.GameFinished += OnGameFinished;
        }

        public void StartTimer()
        {
            ResetTimer();
            IsRunning = true;
        }

        public void StopTimer()
        {
            IsRunning = false;
        }

        public void ResetTimer()
        {
            ElapsedTime = 0f;
            TimeChanged.Invoke(ElapsedTime);
        }

        public void Tick()
        {
            if (!IsRunning || _gameFlow.IsFinished)
                return;

            ElapsedTime += Time.deltaTime;
            TimeChanged.Invoke(ElapsedTime);
        }

        public void Dispose()
        {
            _gameFlow.GameFinished -= OnGameFinished;
        }

        private void OnGameFinished(GameResult result)
        {
            StopTimer();
        }
    }
}
