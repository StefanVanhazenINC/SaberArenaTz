using System;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base
{
    [System.Serializable]
    public class AmmoHolder
    {
        public WeaponAmmoType AmmoType;
        public int AmmoAmountMax = 12;
        public int AmmoStartCount = 12;
        private int _ammoAmountCurrent = 0;
        public int AmmoAmountCurrent => _ammoAmountCurrent;

        public event Action OnChangeAmmoAmount = delegate { };
        public bool TryAddAmmo(int ammoAmount)
        {
            if (_ammoAmountCurrent>= AmmoAmountMax)
            {
                return false;
            }
            _ammoAmountCurrent += ammoAmount;
            if (_ammoAmountCurrent >= AmmoAmountMax)
            {
                _ammoAmountCurrent = AmmoAmountMax;
            }
            OnChangeAmmoAmount?.Invoke();
            return true;
        }
        public void SetCurrentAmount(int amount)
        {
            _ammoAmountCurrent = Mathf.Clamp(amount, 0, AmmoAmountMax);
            OnChangeAmmoAmount?.Invoke();
        }
        public bool TryRemove(int amount)
        {
            if (_ammoAmountCurrent < amount || _ammoAmountCurrent<=0)
            {
                return false;
            }
            _ammoAmountCurrent -= amount;
            OnChangeAmmoAmount?.Invoke();
            return true;
        }

        public void Reset()
        {
            OnChangeAmmoAmount = delegate { };
            _ammoAmountCurrent = AmmoStartCount;
        }

    }
}
