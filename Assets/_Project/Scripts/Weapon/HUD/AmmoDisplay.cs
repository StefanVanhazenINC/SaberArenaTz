
using _Project.Scripts._Common.Weapon.Base;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Weapon.HUD
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField] protected BaseWeapon _baseWeapon;
        [SerializeField] protected TMP_Text  _ammoText;

        private bool _lowAmmoShow = false;

        protected virtual void Awake()
        {
            _baseWeapon = GetComponentInParent<BaseWeapon>();
        }
    
        protected virtual void OnEnable()
        {
            if (!TryResolveWeapon())
                return;

            _baseWeapon.Reloading.OnChangeAmmo += RedrawAmmo;
            RedrawAmmo();
        }

        protected virtual void OnDisable()
        {
            if (_baseWeapon?.Reloading != null)
            {
                _baseWeapon.Reloading.OnChangeAmmo -= RedrawAmmo;
            }
        }

        public virtual void RedrawAmmo()
        {
            if (!TryResolveWeapon() || _ammoText == null)
                return;

            _ammoText.text = _baseWeapon.Reloading.AmmoMagCurrentValue.ToString();
            if ( _baseWeapon.Reloading.AmmoMagCurrentValue<=0)
            {
                _ammoText.color = Color.red;
            }
            else
            {
                _ammoText.color = Color.white;  
            }
        }

        protected bool TryResolveWeapon()
        {
            if (_baseWeapon == null)
            {
                _baseWeapon = GetComponentInParent<BaseWeapon>();
            }

            return _baseWeapon != null && _baseWeapon.Reloading != null;
        }
    }
}
