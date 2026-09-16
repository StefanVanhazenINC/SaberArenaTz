using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using UnityEngine;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Base.Configurator
{
    public class WeaponFactory : IWeaponFactory
    {
        private readonly DiContainer _container;
        private BaseWeapon _prefab;
        private IPoolProjectile _pool;
        private IWeaponConfigurator _weaponConfigurator;
        private IProviderCollection _providerCollection;
        public WeaponFactory ( DiContainer container, BaseWeapon prefab, IPoolProjectile pool, IWeaponConfigurator configurator,IProviderCollection providerCollection=null )
        {
            _container = container;
            _prefab = prefab;
            _pool = pool;
            _weaponConfigurator = configurator;
            _providerCollection = providerCollection;
        }
        public BaseWeapon Create(WeaponConfig param)
        {
            var weapon = _container.InstantiatePrefabForComponent<BaseWeapon>(_prefab);
            weapon = _weaponConfigurator.Install( param, _pool, weapon,_providerCollection);
            return weapon; 
        }

        public BaseWeapon CompleteExisting(WeaponConfig config, BaseWeapon weapon)
        {
            return _weaponConfigurator.CompleteExisting(config, _pool, weapon, _providerCollection);
        }

        public BaseWeapon CreatePrebaked(WeaponConfig config, BaseWeapon prefab, Transform parent)
        {
            BaseWeapon weapon = _container.InstantiatePrefabForComponent<BaseWeapon>(prefab, parent);
            return CompleteExisting(config, weapon);
        }
    }
}
