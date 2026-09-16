using TMPro;
using UnityEngine;

namespace _Project.Scripts.Weapon.HUD
{
    public class AmmoAndStockDisplay :  AmmoDisplay
    {
        [SerializeField] private TMP_Text  _stockText;
        
        protected override void Awake()
        {
            base.Awake();
            RedrawStock();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!TryResolveWeapon())
                return;

            _baseWeapon.Reloading.OnChangeAmmo += RedrawStock;
            RedrawStock();
        }

        protected override void OnDisable()
        {
            if (_baseWeapon?.Reloading != null)
            {
                _baseWeapon.Reloading.OnChangeAmmo -=  RedrawStock;
            }

            base.OnDisable();
        }
        public void RedrawStock()
        {
            if (!TryResolveWeapon() || _stockText == null)
                return;

            _stockText.text = _baseWeapon.Reloading.AmmoStockCurrentValue .ToString();
            if ( _baseWeapon.Reloading.AmmoStockCurrentValue <=0)
            {
                _stockText.color = Color.red;  
            }
            else
            {
                _stockText.color = Color.white;  
            }   
        }
    }
}
