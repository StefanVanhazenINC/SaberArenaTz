using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts._Common.Weapon.Base.Projectile
{
    public class PoolProjectile: IPoolProjectile
    {
        
        private int _maxSizePool = 1000;
        private Transform _parentPool;
        
        private readonly Dictionary<IProjectile, ObjectPool<IProjectile>> _pools = new();
        private readonly Dictionary<IProjectile, IProjectile> _objectToPrefabMap = new();
        
        private readonly Func<IProjectile> _onCreateProjectile;
        private readonly Action<IProjectile,GameObject> _onAfterCreateProjectile;
        
        
        public PoolProjectile(Transform parentPool, int maxSizePool = 100)
        {
            _maxSizePool = maxSizePool;
            _parentPool = parentPool;
        }

        public PoolProjectile(Transform parentPool, Func<IProjectile> createFunc, Action<IProjectile,GameObject> onAfteCreateProjectile, int maxSizePool = 100)
        {
            _maxSizePool = maxSizePool;
            _parentPool = parentPool;
            _onCreateProjectile = createFunc;
            _onAfterCreateProjectile = onAfteCreateProjectile;
        }
        
        private ObjectPool<IProjectile> CreateNewPool(IProjectile prefab)
        {
            return new ObjectPool<IProjectile>(
                () => CreatePooledObject(prefab),
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyObject,
                true,
                20,
                _maxSizePool
            );
        }
        private IProjectile CreatePooledObject(IProjectile prefab)
        {
            IProjectile projectile;
            GameObject projectileGameObject;
            
            if (_onCreateProjectile == null)
            {
                projectileGameObject = GameObject.Instantiate(prefab.GetGameObject(), Vector3.zero, Quaternion.identity, _parentPool);
                projectile = projectileGameObject.GetComponent<IProjectile>();
            }
            else
            {
                projectile = _onCreateProjectile?.Invoke();
                projectileGameObject = projectile.GetGameObject();
            }
            
            if (_onAfterCreateProjectile!=null)
            {
                _onAfterCreateProjectile?.Invoke(projectile,projectileGameObject);
            }
    
            
            projectileGameObject.SetActive(false);
            projectile.DisableCallback += ActionToReturnInPool;

            _objectToPrefabMap[projectile] = prefab;
            return projectile;
        }

        
        public IProjectile GetObjectInPool(IProjectile prefab)
        {
            if (!_pools.ContainsKey(prefab))
            {
                _pools[prefab] = CreateNewPool(prefab);
            }
            return _pools[prefab].Get();
        }
        void ActionToReturnInPool(IProjectile obj)
        {
            if (_objectToPrefabMap.TryGetValue(obj, out var prefab) && _pools.TryGetValue(prefab, out var pool))
            {
                pool.Release(obj);
            }
        }
        void OnDestroyObject(IProjectile obj)
        {
            GameObject.Destroy(obj.GetGameObject());
        }
        void OnReturnToPool(IProjectile obj)
        {
            obj.Disable();
        }
        void OnTakeFromPool(IProjectile obj)
        {
        }

    }
}