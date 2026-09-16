using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts._Common.Pool
{
    public class GenericPool<T> where T : Component, IReturnToPool<T>
    {
        private readonly Transform _parent;
        private readonly int _maxSize;
        private readonly int _defaultCapacity;

        private readonly Dictionary<T, ObjectPool<T>> _pools = new();
        private readonly Dictionary<T, T> _instanceToPrefab = new();

        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;
        private readonly Action<T> _onCreated;

        public GenericPool(
            Transform parent,
            int defaultCapacity = 20,
            int maxSize = 100,
            Action<T> onCreated = null,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null)
        {
            _parent = parent;
            _defaultCapacity = defaultCapacity;
            _maxSize = maxSize;
            _onCreated = onCreated;
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
        }

        public T Get(T prefab)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab);
                _pools[prefab] = pool;
            }

            return pool.Get();
        }

        public void Release(T instance)
        {
            if (_instanceToPrefab.TryGetValue(instance, out var prefab) &&
                _pools.TryGetValue(prefab, out var pool))
            {
                pool.Release(instance);
            }
            else
            {
                Debug.LogWarning($"Pool for instance {instance.name} not found.");
            }
        }

        private ObjectPool<T> CreatePool(T prefab)
        {
            return new ObjectPool<T>(
                createFunc: () => CreateInstance(prefab),
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnToPool,
                actionOnDestroy: OnDestroyObject,
                collectionCheck: true,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }

        private T CreateInstance(T prefab)
        {
            var instance = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, _parent);
            instance.gameObject.SetActive(false);

            instance.ReturnRequested += Release;

            _instanceToPrefab[instance] = prefab;
            _onCreated?.Invoke(instance);

            return instance;
        }

        private void OnTakeFromPool(T instance)
        {
            instance.gameObject.SetActive(true);
            _onGet?.Invoke(instance);
        }

        private void OnReturnToPool(T instance)
        {
            _onRelease?.Invoke(instance);
            instance.gameObject.SetActive(false);
        }

        private void OnDestroyObject(T instance)
        {
            instance.ReturnRequested -= Release;
            _instanceToPrefab.Remove(instance);
            _onDestroy?.Invoke(instance);
            UnityEngine.Object.Destroy(instance.gameObject);
        }
    }
}