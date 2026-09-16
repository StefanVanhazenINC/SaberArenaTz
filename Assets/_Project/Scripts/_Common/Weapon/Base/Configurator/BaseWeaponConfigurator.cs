using _Project.Scripts._Common.Weapon.Base.AltWeaponUse;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using _Project.Scripts._Common.Weapon.Base.TypeShooter;
using _Project.Scripts._Common.Weapon.Base.Visual;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Configurator
{
    public class BaseWeaponConfigurator : IWeaponConfigurator
    {
        public BaseWeapon Install(WeaponConfig config, IPoolProjectile pool, BaseWeapon weapon, IProviderCollection providerCollection = null)
        {
            return Configure(config, pool, weapon, providerCollection);
        }

        public BaseWeapon CompleteExisting(WeaponConfig config, IPoolProjectile pool, BaseWeapon weapon, IProviderCollection providerCollection = null)
        {
            if (weapon == null)
                throw new System.ArgumentNullException(nameof(weapon));

            return Configure(config, pool, weapon, providerCollection);
        }

        private BaseWeapon Configure(
            WeaponConfig config,
            IPoolProjectile pool,
            BaseWeapon weapon,
            IProviderCollection providerCollection)
        {
            if (config == null)
                throw new System.ArgumentNullException(nameof(config));
            if (weapon == null)
                throw new System.ArgumentNullException(nameof(weapon));

            ShooterInstaller(config, pool, out IShooter shooter);
            ReloadingInstaller(config, out IReloading reloading);
            AltUseWeaponInstaller(config, out IAltWeaponUse altWeaponUse);
            weapon.Set(shooter,reloading, altWeaponUse, config);
            ResolveWeaponVisual(config, weapon, out BaseWeaponVisualContainer visual);
            weapon.SetVisual(visual);
            ModuleInstaller(config, weapon);
            weapon.SetProviderCollection( providerCollection);
            return weapon;
        }

     

        private void ReloadingInstaller(WeaponConfig config, out IReloading reloading)
        {
            reloading = null;
            if (config.WeaponData.ReloadingPrototype!=null)
            {
                reloading = config.WeaponData.ReloadingPrototype.Clone();
            }
        }

        private void ShooterInstaller(WeaponConfig config, IPoolProjectile pool, out IShooter shot)
        {
            shot = null;
            if (config.WeaponData.ShooterPrototype!=null)
            {
                shot = config.WeaponData.ShooterPrototype.Clone(pool);
            }

        }

        private void AltUseWeaponInstaller(WeaponConfig config, out IAltWeaponUse altUse)
        {
            altUse = null;
            if (config.WeaponData.AltWeaponUse!=null)
            {
                altUse = config.WeaponData.AltWeaponUse.Clone();
            }
        }
        

        private void ModuleInstaller(WeaponConfig config, BaseWeapon weapon)
        {
            for (int i = 0; i < config.WeaponData.WeaponDataModules.Count; i++)
            {
                config.WeaponData.WeaponDataModules[i].InstallModule(config,weapon);
            }
        }

        private void ResolveWeaponVisual(
            WeaponConfig config,
            BaseWeapon weapon,
            out BaseWeaponVisualContainer weaponVisual)
        {
            weapon.gameObject.SetActive(false);

            weaponVisual = weapon.GetComponentInChildren<BaseWeaponVisualContainer>(true);
            if (weaponVisual == null)
                weaponVisual = GameObject.Instantiate(config.VisualContainer, weapon.transform);

            weapon.ShootDir = weaponVisual.ShotDir;

            if (weaponVisual.PositionLineCheck)
                weapon.PositionLineCheck = weaponVisual.PositionLineCheck;

            weapon.WeaponVisualContainer = weaponVisual ;
            WeaponAnimationController weaponAnimation = weaponVisual.WeaponAnimation != null
                ? weaponVisual.WeaponAnimation
                : weaponVisual.GetComponent<WeaponAnimationController>();
            
            if (weaponAnimation != null)
            {
                weaponAnimation.SetVisualContainer( weaponVisual);
                weaponAnimation.SetAnimation(weapon);
            }
        }
    }
}
