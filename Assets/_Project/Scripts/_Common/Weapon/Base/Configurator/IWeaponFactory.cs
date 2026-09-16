using _Project.Scripts._Common.Weapon.Base.Data;
using UnityEngine;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Base.Configurator
{
    public interface IWeaponFactory : IFactory<WeaponConfig, BaseWeapon>
    {
        BaseWeapon CreatePrebaked(WeaponConfig config, BaseWeapon prefab, Transform parent);
        BaseWeapon CompleteExisting(WeaponConfig config, BaseWeapon weapon);
    }
}
