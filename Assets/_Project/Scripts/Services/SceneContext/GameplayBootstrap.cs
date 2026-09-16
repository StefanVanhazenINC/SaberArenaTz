using _Project.Scripts.Character;
using _Project.Scripts.Character.DI;
using _Project.Scripts.Character.SpawnSystem;
using _Project.Scripts.GameFlow;
using UnityEngine;

namespace _Project.Scripts.Services.SceneContext
{
    public class GameplayBootstrap
    {
        private readonly PlayerSpawnPointProvider _spawnPointProvider;
        private readonly CharacterFactory _characterFactory;
        private readonly IMatchStatsService _matchStats;

        private PlayerMarker _playerMarker;
        private Player _playerFacade;

        public GameplayBootstrap(
            PlayerSpawnPointProvider spawnPointProvider,
            CharacterFactory characterFactory,
            IMatchStatsService matchStats)
        {
            _spawnPointProvider = spawnPointProvider;
            _characterFactory = characterFactory;
            _matchStats = matchStats;
        }

        public Player Facade => _playerFacade;

        public Player SpawnCharacter()
        {
            DespawnCharacter();

            Transform spawnPoint = _spawnPointProvider.GetSpawnPoint();

            _playerMarker = _characterFactory.Create();
            _playerMarker.RunContext();

            _playerFacade = _playerMarker.GetComponentInChildren<Player>();
            _playerFacade.OnAfterInitialize += RegisterPlayerStats;

            if (spawnPoint != null)
                _playerFacade.SpawnAt(spawnPoint);

            return _playerFacade;
        }

        public void DespawnCharacter()
        {
            if (_playerFacade != null)
                _playerFacade.OnAfterInitialize -= RegisterPlayerStats;

            if (_playerMarker != null)
                Object.Destroy(_playerMarker.gameObject);

            _playerMarker = null;
            _playerFacade = null;
        }

        private void RegisterPlayerStats()
        {
            if (_playerFacade == null)
                return;

            _playerFacade.OnAfterInitialize -= RegisterPlayerStats;
            _matchStats.RegisterPlayer(_playerFacade);
        }
    }
}
