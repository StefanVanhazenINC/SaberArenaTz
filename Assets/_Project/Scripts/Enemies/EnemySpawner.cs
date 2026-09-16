using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace _Project.Scripts.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private SlimeEnemy _slimePrefab;

        [Header("Spawn Area")]
        [SerializeField] private Vector3 _spawnAreaOffset;
        [SerializeField] private Vector3 _spawnAreaSize = new Vector3(6f, 0f, 6f);
        [SerializeField] private bool _sampleNavMesh = true;
        [SerializeField] private float _navMeshSampleRadius = 2f;
        [SerializeField] private int _positionAttempts = 12;

        [Header("Spawn Settings")]
        [SerializeField] private int _spawnCount = 3;
        [SerializeField] private float _spawnDelay = 0.15f;
        [SerializeField] private bool _spawnOnTargetSet = true;

        [Header("Events")]
        [SerializeField] private UnityEvent _onSpawnStarted = new UnityEvent();
        [SerializeField] private UnityEvent _onSpawnFinished = new UnityEvent();

        private readonly List<SlimeEnemy> _spawnedEnemies = new List<SlimeEnemy>();
        private Transform _target;
        private Coroutine _spawnRoutine;
        private bool _hasSpawned;

        public event Action<EnemySpawner, SlimeEnemy> EnemySpawned = delegate { };
        public event Action<EnemySpawner, SlimeEnemy> EnemyDied = delegate { };

        public IReadOnlyList<SlimeEnemy> SpawnedEnemies => _spawnedEnemies;
        public int SpawnCount => _spawnCount;
        public Transform Target => _target;

        public void SetSpawnCount(int value)
        {
            _spawnCount = Mathf.Max(0, value);
        }

        public void SetTarget(Transform target)
        {
            _target = target;

            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                if (_spawnedEnemies[i] != null)
                    _spawnedEnemies[i].SetTarget(_target);
            }

            if (_spawnOnTargetSet && !_hasSpawned)
                SpawnEnemies();
        }

        public void SpawnEnemies()
        {
            if (_spawnRoutine != null)
                StopCoroutine(_spawnRoutine);

            _spawnRoutine = StartCoroutine(SpawnRoutine());
        }

        public void SpawnEnemies(int spawnCount, Transform target)
        {
            SetSpawnCount(spawnCount);
            SetTarget(target);

            if (!_spawnOnTargetSet)
                SpawnEnemies();
        }

        private IEnumerator SpawnRoutine()
        {
            if (_slimePrefab == null)
            {
                Debug.LogWarning("Slime prefab is not assigned", this);
                yield break;
            }

            _hasSpawned = true;
            _onSpawnStarted.Invoke();

            for (int i = 0; i < _spawnCount; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                SlimeEnemy slime = Instantiate(
                    _slimePrefab,
                    spawnPosition,
                    transform.rotation,
                    transform);

                slime.Spawn(_target);
                slime.Died += HandleEnemyDied;
                _spawnedEnemies.Add(slime);
                EnemySpawned.Invoke(this, slime);

                if (_spawnDelay > 0f && i < _spawnCount - 1)
                    yield return new WaitForSeconds(_spawnDelay);
            }

            _spawnRoutine = null;
            _onSpawnFinished.Invoke();
        }

        private Vector3 GetSpawnPosition()
        {
            int attempts = Mathf.Max(1, _positionAttempts);

            for (int i = 0; i < attempts; i++)
            {
                Vector3 point = GetRandomPointInArea();
                if (!_sampleNavMesh)
                    return point;

                if (NavMesh.SamplePosition(point, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas))
                    return hit.position;
            }

            return transform.TransformPoint(_spawnAreaOffset);
        }

        private Vector3 GetRandomPointInArea()
        {
            Vector3 halfSize = _spawnAreaSize * 0.5f;
            Vector3 localPoint = _spawnAreaOffset + new Vector3(
                UnityEngine.Random.Range(-halfSize.x, halfSize.x),
                UnityEngine.Random.Range(-halfSize.y, halfSize.y),
                UnityEngine.Random.Range(-halfSize.z, halfSize.z));

            return transform.TransformPoint(localPoint);
        }

        private void HandleEnemyDied(SlimeEnemy enemy)
        {
            if (enemy != null)
                enemy.Died -= HandleEnemyDied;

            EnemyDied.Invoke(this, enemy);
        }

        private void OnDestroy()
        {
            if (_spawnRoutine != null)
                StopCoroutine(_spawnRoutine);

            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                if (_spawnedEnemies[i] != null)
                    _spawnedEnemies[i].Died -= HandleEnemyDied;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_spawnCount < 0)
                _spawnCount = 0;

            if (_spawnDelay < 0f)
                _spawnDelay = 0f;

            if (_navMeshSampleRadius < 0f)
                _navMeshSampleRadius = 0f;

            if (_positionAttempts < 1)
                _positionAttempts = 1;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.25f);
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(_spawnAreaOffset, _spawnAreaSize);
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            Gizmos.DrawWireCube(_spawnAreaOffset, _spawnAreaSize);
            Gizmos.matrix = previousMatrix;
        }
#endif
    }
}
