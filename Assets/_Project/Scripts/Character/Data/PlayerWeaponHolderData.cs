using System;
using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Aiming;
using _Project.Scripts._Common.Weapon.Base.Data;
using UnityEngine;
using Alchemy.Inspector;

namespace _Project.Scripts.Character.Data
{
    [Serializable]
    public class PlayerWeaponHolderData
    {
        [SerializeField] private WeaponHolder.Data _weaponData;
        [SerializeField] private WeaponAimSettings _aimSettings = new();
        [SerializeField] private AmmoHolder[] _ammoHolders;

        public WeaponHolder.Data WeaponData => _weaponData;
        public WeaponAimSettings AimSettings => _aimSettings;

        public AmmoHolder[] AmmoHolders => _ammoHolders;

        public WeaponHolder.Data CreateRuntimeWeaponData(Transform weaponParent, Transform lookAt)
        {
            WeaponHolder.Data source = _weaponData;
            return new WeaponHolder.Data
            {
                MaxWeapon = source.MaxWeapon,
                Weapons = source.Weapons == null ? Array.Empty<WeaponConfig>() : (WeaponConfig[])source.Weapons.Clone(),
                WeaponParent = weaponParent,
                LookAt = lookAt
            };
        }

        public AmmoHolder[] CreateRuntimeAmmoHolders()
        {
            if (_ammoHolders == null)
                return Array.Empty<AmmoHolder>();

            AmmoHolder[] runtimeHolders = new AmmoHolder[_ammoHolders.Length];
            for (int i = 0; i < _ammoHolders.Length; i++)
            {
                AmmoHolder source = _ammoHolders[i];
                if (source == null)
                    continue;

                runtimeHolders[i] = new AmmoHolder
                {
                    AmmoType = source.AmmoType,
                    AmmoAmountMax = source.AmmoAmountMax,
                    AmmoStartCount = source.AmmoStartCount
                };
            }

            return runtimeHolders;
        }
        
        public void Reset()
        {
            for (int i = 0; i < _ammoHolders.Length; i++)
            {
                _ammoHolders[i].Reset();
            }
        }
    }
}
