using _Project.Scripts._Common.Weapon.Base.Configurator;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using Alchemy.Inspector;
using UnityEngine;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Base.Installer
{
    [HideScriptField]
    public class WeaponInstaller : MonoInstaller
    {
        [SerializeField] private Transform _parentPool;
       
        [SerializeField] private WeaponInstallerSetting _setting;
        
        public override void InstallBindings()
        {
            InstallPool();
            InstallWeapon();
        }

        private void InstallPool()
        {
            Container.BindInterfacesAndSelfTo<PoolProjectile>().AsSingle().WithArguments( _parentPool,  _setting.MaxSizePool).NonLazy();
        }
        private void InstallWeapon()
        {
            Container.Bind<BaseWeapon>().FromInstance(_setting.BaseWeapon ).AsSingle();
            Container.BindInterfacesAndSelfTo<BaseWeaponConfigurator>().AsSingle().NonLazy();
            Container.Bind<IWeaponFactory>().To<WeaponFactory>().AsSingle().WithArguments(_setting.ProviderCollection).NonLazy();

        }
    }
}