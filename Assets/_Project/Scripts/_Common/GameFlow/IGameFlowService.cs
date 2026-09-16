using System;

namespace _Project.Scripts._Common.GameFlow
{
    public interface IGameFlowService
    {
        public event Action<GameResult> GameFinished;
        public event Action RestartRequested;
        public bool IsFinished { get; }
        
        public void Victory();
        public void Defeat();
        public void Restart();
    }
}
