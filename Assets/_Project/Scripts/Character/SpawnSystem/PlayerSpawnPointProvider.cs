using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Character.SpawnSystem
{
    public sealed class PlayerSpawnPointProvider
    {
        private PlayerSpawnPoint[] _spawnPoints;

        public void Refresh()
        {
            _spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        }

        public Transform GetSpawnPoint()
        {
            Refresh();

            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogWarning("Player spawn point is not found on scene");
                return null;
            }

            return _spawnPoints
                .OrderBy(spawnPoint => spawnPoint.Priority)
                .First()
                .Transform;
        }
    }
}
