using System;
using System.Threading;
using _Project.Scripts._Common.Weapon.Base.Aiming;
using _Project.Scripts._Common.Weapon.Base.Configurator;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using Common.BaseComponent;
using Common.Weapon.Damageable;
using Cysharp.Threading.Tasks;
using Alchemy.Inspector;
using UnityEngine;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Base
{

  

    public class WeaponHolder
    {
        [System.Serializable]
        public class Data 
        {
            public int MaxWeapon = 4;
            public WeaponConfig[] Weapons;
            public Transform LookAt;
            public Transform WeaponParent;

            public void ChangeParent(Transform parent)
            {
                WeaponParent = parent;
            }

            public void ChangeLookAt(Transform lookAt)
            {
                LookAt = lookAt;
            }
        }

        private int _selectWeapon = -1;
        private bool _isSwithcProcess;
        
        private Data _data;
        private TeamComponent _teamComponent;
        private BaseWeapon[] _weaponInventory;
        private IWeaponFactory _factory;
        private IAimProvider _aimProvider;
        
        private CancellationTokenSource _waitHideCts;
        
        
        public bool BlockUseWeapons { get; set; }
        public BaseWeapon GetCurrentWeapon =>_selectWeapon>=0 ? _weaponInventory[_selectWeapon] : null;
        public int GetCurrentWeaponIndex => _selectWeapon;

        public Transform GetWeaponParent => _data.WeaponParent;
        public BaseWeapon[] AllWeapon => _weaponInventory;
        public int GetLastWeaponIndex 
        {
            get
            {
                for (int i = AllWeapon.Length - 1; i >= 0; i--)
                {
                    if ( AllWeapon[i] !=null)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
        public event Action OnChangeWeapon = delegate { };
        public event Action<BaseWeapon> OnChangeBaseWeapon = delegate { };
        public event Action<IDamageable> OnHitTarget = delegate { };
        public event Action<BaseWeapon> OnWeaponInstall = delegate { };
        
        public WeaponHolder(Data data,IWeaponFactory factory,TeamComponent teamComponent)
            : this(data, factory, teamComponent, null)
        {
        }

        [Inject]
        public WeaponHolder(Data data,IWeaponFactory factory,TeamComponent teamComponent, IAimProvider aimProvider)
        {
            _data = data;
            _factory = factory;
            _teamComponent = teamComponent;
            _selectWeapon = -1;
            _weaponInventory = new BaseWeapon[_data.MaxWeapon];
            _aimProvider = aimProvider;

            OnChangeWeapon += UseDefaultImmediately;
        }
        public void Initialize()
        {
            for (int i = 0; i < _data.Weapons.Length; i++)
            {
                InstallWeapon(_data.Weapons[i]);
            }
            
            SwitchWeaponProcess(0);
        }
        
        public void InstallWeapon(WeaponConfig newWeapon)
        {
            int emptySlot = FindEmptySlot();
            if ( emptySlot ==-1 || newWeapon==null)
            {
                return;
            }

            if (WeaponIsAlreadyEquip( newWeapon))
            {
                return;
            }

            BaseWeapon weapon = _factory.Create(newWeapon);
        
            InstallWeapon(weapon);
        }

        private bool WeaponIsAlreadyEquip(WeaponConfig newWeapon)
        {
            bool temp = false;
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
                if (!_weaponInventory[i])
                {
                    continue;
                }

                WeaponConfig installedConfig = _weaponInventory[i].Config;
                if (installedConfig == newWeapon)
                    return true;

                string installedId = installedConfig.WeaponData.Id;
                string newId = newWeapon.WeaponData.Id;
                if (!string.IsNullOrEmpty(installedId) && installedId == newId)
                {
                    return true;
                }
            }
            
            
            return temp;
        }

        public void ChangeWeaponParent(BaseWeapon weapon)
        {
            weapon.transform.SetParent(_data.WeaponParent,false);
            weapon.transform.localPosition = Vector3.zero;
        }

        public void InstallWeapon(BaseWeapon weapon)
        {
            int emptySlot = FindEmptySlot();
            if (weapon == null || emptySlot == -1)
                return;
        
            weapon.CancelUseWeapon();
         
            ChangeWeaponParent(weapon);
            
            _weaponInventory[emptySlot] = weapon;
        
            weapon.SetAimProvider(_aimProvider);
            
            weapon.OnHitTarget += HitTarget;
            if (weapon.ShotSpawner!=null)
            {
                weapon.ShotSpawner.SetTeam(_teamComponent);
            }
            OnWeaponInstall?.Invoke(weapon);
        }

   

        public BaseWeapon RemoveWeapon( )
        {
            BaseWeapon tempWeapon = _weaponInventory[_selectWeapon];
            _weaponInventory[_selectWeapon] = null;
            SwitchImmediatelyProcess(FindFullSlot());
            return tempWeapon;
        }
        private void HitTarget(IDamageable target)
        {
            OnHitTarget?.Invoke(target);
        }
        private int FindFullSlot()
        {
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
                if (_weaponInventory[i] != null)
                {
                    return i;
                }
            }
            return -1; 
        }
        public void ResetAmmo() 
        {
            GetCurrentWeapon.Reloading.RestoreAmmo(); 
        }
        
        public void SwitchWeaponIndex(int i)
        {
            if ((_selectWeapon != -1 && !_weaponInventory[_selectWeapon].IsReadyAfterDraw) /*|| _weaponInventory.Length==1 */ ) 
            {
                return;
            }
            int temp = _selectWeapon + i;
            temp = (temp % _weaponInventory.Length + _weaponInventory.Length) % _weaponInventory.Length;
            if (_weaponInventory[temp]==null ||  temp==_selectWeapon)
            {
                return;
            }
            SwitchWeaponProcess(temp);
        }
        private void SwitchWeaponProcess(int i)
        {
            if (_weaponInventory.Length==0) 
            {
                return;
            }
            if (_selectWeapon != -1)
            {
                _weaponInventory[_selectWeapon].Hide();
              
            }
            else 
            {
                if ( _weaponInventory[i]!=null)
                {
                    SwitchWeapon(i);
                }

            }

            if (_selectWeapon<_weaponInventory.Length && _selectWeapon!= -1)
            {
                CancelWaitHide();
                _waitHideCts = new CancellationTokenSource();
                WaitHideWeaponAsync(() => _weaponInventory[_selectWeapon].IsReadyHide, i, _waitHideCts.Token).Forget();
            }

       
        }
        private int FindEmptySlot()
        {
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
                if (_weaponInventory[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public void SwitchImmediatelyIndex(int index)
        {
    
            //if (_selectWeapon == -1 || _weaponInventory.Length==1 ) 
            if (_selectWeapon != -1 || _weaponInventory.Length==1 ) 
            {
                return;
            }

            int temp = _selectWeapon + index;
            temp = (temp % _weaponInventory.Length + _weaponInventory.Length) % _weaponInventory.Length;
            
            if (_weaponInventory[temp]==null ||  temp==_selectWeapon)
            {
                return;
            }

            SwitchImmediatelyProcess(temp);
      
        }
        public void SwitchImmediatelyProcess(int index)
        {
            if (_weaponInventory.Length==0) 
            {
                return;
            }
            CancelWaitHide();
            SwitchWeapon(index);
        }
        private async UniTask WaitHideWeaponAsync(Func<bool> condition, int i, CancellationToken ct = default)
        {
            await UniTask.WaitUntil(condition, cancellationToken: ct);
            SwitchWeapon(i);
        }
        private void CancelWaitHide()
        {
            _waitHideCts?.Cancel();
            _waitHideCts?.Dispose();
            _waitHideCts = null;
        }
        private void SwitchWeapon(int i)
        {
            if (_selectWeapon != -1 && _weaponInventory[_selectWeapon]!=null)
            {
                _weaponInventory[_selectWeapon].gameObject.SetActive(false);
            }
            _selectWeapon = i;
            _weaponInventory[_selectWeapon].gameObject.SetActive(true);
            _weaponInventory[_selectWeapon].Draw();
            OnChangeWeapon?.Invoke();
            OnChangeBaseWeapon?.Invoke(_weaponInventory[_selectWeapon]);
        }
        public void ProcessWeapon(bool value) 
        {
            if (value)
            {
                UseWeapon();
            } 
            else
            {
                CancelWeapon();
            }
        }

        public void ProcessAltUseWeapon(bool value)
        {
            if (value)
            {
                AltUseWeapon();
            } 
            else
            {
                AltCancelWeapon();
            }
        }
        private void AltUseWeapon() 
        {
            if (GetAproveWeapon() )
                _weaponInventory[_selectWeapon].AltUseWeapon();
        }
        private void AltCancelWeapon() 
        {
            if (GetAproveWeapon())
                _weaponInventory[_selectWeapon].AltCancelWeapon();
        }
        private void UseWeapon() 
        {
            if (GetAproveWeapon() )
                _weaponInventory[_selectWeapon].UseWeapon();
        }
        private void CancelWeapon() 
        {
            if (GetAproveWeapon())
                _weaponInventory[_selectWeapon].CancelUseWeapon();
        }
        public void Reloading()
        {
            if (GetAproveWeaponWithNotBlock()&& _weaponInventory[_selectWeapon].IsReadyAfterDraw) //GetAproveWeaponWithNotBlock()
                _weaponInventory[_selectWeapon].TryReloading();
        }

        public bool RestoreAllAmmoType( WeaponAmmoType ammoType)
        {
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
                if (_weaponInventory[i]==null || _weaponInventory[i].Reloading==null)continue;
                if (_weaponInventory[i]?.Reloading.AmmoType == ammoType)
                {
                    _weaponInventory[i]?.Reloading.RestoreAmmo(ammoType);
                    return true;
                }
            }
            return false;

        }

        public bool RestoreAmmo(int value, WeaponAmmoType ammoType)
        {
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
                if (_weaponInventory[i]==null || _weaponInventory[i].Reloading==null)continue;
                
                if (_weaponInventory[i]?.Reloading.AmmoType == ammoType)
                {
                   
                    if ( _weaponInventory[i]?.Reloading.AddedAmmo(value,ammoType) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool FindWeapon(WeaponConfig configWeapon,out BaseWeapon findWeapon)
        {
            for (int i = 0; i < _weaponInventory.Length; i++)
            {
         
                if (_weaponInventory[i]!=null  )
                {
                    if (_weaponInventory[i].Config == configWeapon)
                    {
                        findWeapon = _weaponInventory[i];
                        return true;   
                    }
                }
            }
            findWeapon = null;
            return false;
        }
        private bool GetAproveWeapon()
        {
            if (_selectWeapon > _weaponInventory.Length || _selectWeapon == -1 || BlockUseWeapons || _weaponInventory[_selectWeapon] == null) return false;
            return true;
        }
        private bool GetAproveWeaponWithNotBlock()
        {
            if (_selectWeapon > _weaponInventory.Length || _selectWeapon == -1 || _weaponInventory[_selectWeapon] == null ) return false;
            if (!_weaponInventory[_selectWeapon].IsReadyAfterDraw) return false;
            return true;
        }
        private void UseDefaultImmediately()
        {
        }
    }
}
