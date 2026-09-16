using System;
using _Project.Scripts._Common.GameFlow;
using UnityEngine;

namespace _Project.Scripts.GameFlow
{
    public class GameFlowService : IGameFlowService
    {
        public event Action<GameResult> GameFinished = delegate { };
        public event Action RestartRequested = delegate { };
        public bool IsFinished { get; private set; }

        public void Victory()
        {
            Finish(GameResult.Victory);
        }

        public void Defeat()
        {
            Finish(GameResult.Defeat);
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            IsFinished = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            RestartRequested.Invoke();
        }

        private void Finish(GameResult result)
        {
            if (IsFinished)
                return;

            IsFinished = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameFinished.Invoke(result);
        }
    }
}
