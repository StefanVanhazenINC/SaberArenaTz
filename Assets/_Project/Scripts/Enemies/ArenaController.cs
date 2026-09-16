using System;
using System.Collections.Generic;
using _Project.Scripts._Common.GameSignals;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Enemies
{
    public sealed class ArenaController : MonoBehaviour
    {
        [Serializable]
        private sealed class SpawnerSetup
        {
            [SerializeField] private EnemySpawner _spawner;
            [SerializeField] private int _enemyCount = 3;

            public EnemySpawner Spawner => _spawner;
            public int EnemyCount => Mathf.Max(0, _enemyCount);
        }

        [SerializeField] private SpawnerSetup[] _spawners;
        [SerializeField] private GameEvent _enemyKilledSignal;

        [Header("Events")]
        [SerializeField] private UnityEvent _onArenaStarted = new UnityEvent();
        [SerializeField] private UnityEvent _onArenaCompleted = new UnityEvent();
        [SerializeField] private UnityEvent<int> _onTotalEnemiesChanged = new UnityEvent<int>();
        [SerializeField] private UnityEvent<int> _onEnemyKilled = new UnityEvent<int>();
        [SerializeField] private UnityEvent<int> _onAliveEnemiesChanged = new UnityEvent<int>();

        private readonly HashSet<SlimeEnemy> _deadEnemies = new HashSet<SlimeEnemy>();
        private Transform _target;
        private int _totalEnemies;
        private int _spawnedEnemies;
        private int _killedEnemies;
        private bool _isStarted;
        private bool _isCompleted;

        public int TotalEnemies => _totalEnemies;
        public int SpawnedEnemies => _spawnedEnemies;
        public int KilledEnemies => _killedEnemies;
        public int AliveEnemies => Mathf.Max(0, _spawnedEnemies - _killedEnemies);
        public Transform Target => _target;
        
        
        public event Action Started = delegate { };
        public event Action<SlimeEnemy> EnemySpawned = delegate { };
        public event Action Completed = delegate { };
        private void Awake()
        {
            RecalculateTotalEnemies();
        }

        private void OnEnable()
        {
            SubscribeSpawners();
        }

        private void OnDisable()
        {
            UnsubscribeSpawners();
        }

        public void BeginArena(Transform target)
        {
            if (_isStarted)
                return;

            _target = target;
            _isStarted = true;
            _isCompleted = false;
            _spawnedEnemies = 0;
            _killedEnemies = 0;
            _deadEnemies.Clear();

            RecalculateTotalEnemies();
            _onArenaStarted.Invoke();
            Started.Invoke();
            _onTotalEnemiesChanged.Invoke(_totalEnemies);
            _onAliveEnemiesChanged.Invoke(AliveEnemies);

            for (int i = 0; i < _spawners.Length; i++)
            {
                SpawnerSetup setup = _spawners[i];
                if (setup?.Spawner == null)
                    continue;

                setup.Spawner.SpawnEnemies(setup.EnemyCount, _target);
            }

            if (_totalEnemies == 0)
                CompleteArena();
        }

        public void SetTarget(Transform target)
        {
            _target = target;

            for (int i = 0; i < _spawners.Length; i++)
            {
                SpawnerSetup setup = _spawners[i];
                if (setup?.Spawner != null)
                    setup.Spawner.SetTarget(_target);
            }
        }

        private void SubscribeSpawners()
        {
            for (int i = 0; i < _spawners.Length; i++)
            {
                EnemySpawner spawner = _spawners[i]?.Spawner;
                if (spawner == null)
                    continue;

                spawner.EnemySpawned += HandleEnemySpawned;
                spawner.EnemyDied += HandleEnemyDied;
            }
        }

        private void UnsubscribeSpawners()
        {
            for (int i = 0; i < _spawners.Length; i++)
            {
                EnemySpawner spawner = _spawners[i]?.Spawner;
                if (spawner == null)
                    continue;

                spawner.EnemySpawned -= HandleEnemySpawned;
                spawner.EnemyDied -= HandleEnemyDied;
            }
        }

        private void HandleEnemySpawned(EnemySpawner spawner, SlimeEnemy enemy)
        {
            _spawnedEnemies++;
            EnemySpawned.Invoke(enemy);
            _onAliveEnemiesChanged.Invoke(AliveEnemies);
        }

        private void HandleEnemyDied(EnemySpawner spawner, SlimeEnemy enemy)
        {
            if (enemy != null && !_deadEnemies.Add(enemy))
                return;

            _killedEnemies++;
            _enemyKilledSignal?.Invoke();
            _onEnemyKilled.Invoke(_killedEnemies);
            _onAliveEnemiesChanged.Invoke(AliveEnemies);

            if (_killedEnemies >= _totalEnemies)
                CompleteArena();
        }

        private void CompleteArena()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;
            _onArenaCompleted.Invoke();
            Completed.Invoke();
        }

        private void RecalculateTotalEnemies()
        {
            _totalEnemies = 0;

            if (_spawners == null)
                return;

            for (int i = 0; i < _spawners.Length; i++)
                _totalEnemies += _spawners[i]?.EnemyCount ?? 0;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RecalculateTotalEnemies();
        }
#endif
    }
}
