using _Project.Scripts._Common.GameFlow;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Character
{
    public sealed class PlayerDeathHandler
    {
        private readonly IGameFlowService _gameFlow;
        private bool _deathHandled;

        public PlayerDeathHandler(IGameFlowService gameFlow)
        {
            _gameFlow =  gameFlow;
        }

        public void HandleDeath()
        {
            if (_deathHandled)
                return;

            _deathHandled = true;
            _gameFlow.Defeat();
        }
    }
}
