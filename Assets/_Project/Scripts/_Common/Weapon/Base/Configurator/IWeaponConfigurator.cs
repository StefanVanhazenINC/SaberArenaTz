using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Base.Configurator
{
    public interface IWeaponConfigurator
    {
        BaseWeapon Install(WeaponConfig config, IPoolProjectile pool, BaseWeapon weapon, IProviderCollection providerCollection = null);
        BaseWeapon CompleteExisting(WeaponConfig config, IPoolProjectile pool, BaseWeapon weapon, IProviderCollection providerCollection = null);
    }
}
