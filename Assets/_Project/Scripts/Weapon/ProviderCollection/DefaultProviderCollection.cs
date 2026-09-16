using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts.Weapon.ContextProvider;
using Common.Weapon.Damageable;
using UnityEngine;

namespace _Project.Scripts.Weapon.ProviderCollection
{
    [System.Serializable]
    public  sealed  class DefaultProviderCollection: IProviderCollection
    {
        private readonly WeaponBaseProvider _baseProvider = new();

        public void Bind(CompositeProvider composite)
        {
            composite.AddLayer(_baseProvider);
        }

        public void PrepareShot(BaseWeapon weapon, Transform shotOrigin, Vector3 shotDirection)
        {
            DamageInfo info = new DamageInfo(weapon.Config.WeaponData.Damage, shotDirection);
            _baseProvider.UpdateForShot(info);
        }
    }
}
