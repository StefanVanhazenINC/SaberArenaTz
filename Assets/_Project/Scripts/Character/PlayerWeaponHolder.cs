using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts._Common.Weapon;
using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Configurator;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using _Project.Scripts.Character.Data;
using Alchemy.Inspector;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character
{
    public class PlayerWeaponHolder
    {
        private WeaponHolder _weaponHolder;

        private AmmoHolder[] _ammoHolders;

        private int _lastSwitchWeapon = 0;
        private WeaponCatalog _weaponCatalog;
        public BaseWeapon[] Weapons => _weaponHolder.AllWeapon;
        public AmmoHolder[] AmmoHolders => _ammoHolders;
        public int CurrentWeaponIndex => _weaponHolder.GetCurrentWeaponIndex ;
        public WeaponHolder WeaponHolder => _weaponHolder;
        
        public event Action OnTakeAmmo = delegate { }; 
        
        [Inject]
        public PlayerWeaponHolder(AmmoHolder[] ammoHolders, WeaponHolder weaponHolder , WeaponCatalog weaponCatalog)
        {
            for (int i = 0; i < ammoHolders.Length; i++)
            {
                ammoHolders[i].Reset();
            }

            _ammoHolders = ammoHolders;
            _weaponHolder = weaponHolder;
            _weaponCatalog = weaponCatalog;

        }

        public void InstallNewWeapon(WeaponConfig newWeapon)
        {
            BaseWeapon installedWeapon = null;
            void CacheInstalledWeapon(BaseWeapon weapon) => installedWeapon = weapon;

            _weaponHolder.OnWeaponInstall += CacheInstalledWeapon;
            _weaponHolder.InstallWeapon(newWeapon);
            _weaponHolder.OnWeaponInstall -= CacheInstalledWeapon;

            if (installedWeapon != null)
            {
                SwitchAfterAdd(installedWeapon);
            }
        }

        private void SwitchAfterAdd(BaseWeapon weapon)
        {
            int weaponIndex = Array.IndexOf(_weaponHolder.AllWeapon, weapon);

            if (weaponIndex < 0)
            {
                return;
            }

            _weaponHolder.SwitchImmediatelyProcess(weaponIndex);
        }

        public List<string>  GetOwnedWeaponId()
        {
            List<string> ids = new List<string>();
            foreach (var weapon in Weapons)
            {
                ids.Add(weapon.Config.WeaponData.Id);
            }

            return ids;
        }

        public void RestoreWeapon(List<string> ownedWeapon, int currentWeapon)
        {
            for (int i = 0; i < ownedWeapon.Count; i++)
            {

                _weaponHolder.InstallWeapon(_weaponCatalog.GetById( ownedWeapon[i]));
            }
            _weaponHolder.SwitchImmediatelyProcess(currentWeapon);
        }

        public List<AmmoSnapshotData> Capture()
        {
            var result = new List<AmmoSnapshotData>(_ammoHolders.Length);
            foreach (var holder in _ammoHolders)
            {
                result.Add(new AmmoSnapshotData
                {
                    AmmoType = holder.AmmoType,
                    CurrentAmount = holder.AmmoAmountCurrent
                });
            }

            return result;
        }
        public void Restore(List<AmmoSnapshotData> savedAmmo)
        {
            if (savedAmmo == null)
                return;
            for (int i = 0; i < _ammoHolders.Length; i++)
            {
                var holder = _ammoHolders[i];

              
                for (int j = 0; j < savedAmmo.Count; j++)
                {
                    if (savedAmmo[j].AmmoType != holder.AmmoType)
                        continue;

                    holder.SetCurrentAmount(savedAmmo[j].CurrentAmount);
                    break;
                }
            }
        }
        public void Initialize()
        {
            _weaponHolder.Initialize();
        }
        public void UpdateInput(WeaponHolderInput input)
        {
            _weaponHolder.ProcessWeapon(input.Attack);
            _weaponHolder.ProcessAltUseWeapon(input.AltAttack);
            if (input.Reloading)
            {
                _weaponHolder.Reloading();
            }

            if (input.SwitchWeapon != _lastSwitchWeapon)
            {
                _weaponHolder.SwitchWeaponIndex(input.SwitchWeapon);
            }
        }
       
    }
}
