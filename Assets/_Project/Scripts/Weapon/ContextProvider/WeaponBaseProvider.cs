using _Project.Scripts._Common.Weapon.Base.Data.Context;
using Common.Weapon.Damageable;

namespace _Project.Scripts.Weapon.ContextProvider
{
    public class WeaponBaseProvider : IWeaponContextProvider
    {
        private DamageInfo _baseDamage;

        public WeaponBaseProvider(DamageInfo baseDamage)
        {
            _baseDamage = baseDamage;
        }
        public  WeaponBaseProvider()
        {
        }

        public void UpdateForShot(DamageInfo baseDamage)
        {
            _baseDamage = baseDamage;
        }

        public bool TryGet<T>(out T value)
        {
            if (typeof(T) == typeof(DamageInfo))
            {
                value = (T)(object)_baseDamage;
                return true;
            }

            value = default!;
            return false;
        }
    }
}