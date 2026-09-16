using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts._Common.Pool
{
    public class PrefabPool<T> where T : class
    {
        private readonly int _maxSizePool;

        private readonly Dictionary<GameObject, ObjectPool<T>> _pools = new();
        private readonly Dictionary<T, GameObject> _objectToPrefabMap = new();

        private readonly Func<GameObject, T> _createInstance;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;
        private readonly Action<T> _actionOnDestroy;

        public PrefabPool(
            Func<GameObject, T> createInstance,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            int maxSizePool = 100)
        {
            _createInstance = createInstance;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _maxSizePool = maxSizePool;
        }

        public T Get(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreateNewPool(prefab);
                _pools[prefab] = pool;
            }

            return pool.Get();
        }

        public void Release(T obj)
        {
            if (_objectToPrefabMap.TryGetValue(obj, out var prefab) &&
                _pools.TryGetValue(prefab, out var pool))
            {
                pool.Release(obj);
            }
        }

        private ObjectPool<T> CreateNewPool(GameObject prefab)
        {
            return new ObjectPool<T>(
                () => CreatePooledObject(prefab),
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyObject,
                true,
                20,
                _maxSizePool
            );
        }

        private T CreatePooledObject(GameObject prefab)
        {
            var obj = _createInstance(prefab);
            _objectToPrefabMap[obj] = prefab;
            return obj;
        }

        private void OnTakeFromPool(T obj)
        {
            _actionOnGet?.Invoke(obj);
        }

        private void OnReturnToPool(T obj)
        {
            _actionOnRelease?.Invoke(obj);
        }

        private void OnDestroyObject(T obj)
        {
            _objectToPrefabMap.Remove(obj);
            _actionOnDestroy?.Invoke(obj);
        }
    }
}