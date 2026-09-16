using System;

namespace _Project.Scripts._Common.Weapon.Base.TypeReloading
{
    public interface IReloading
    {
        public event Action OnStartReloading;
        public event Action OnEndReloading;
        public event Action OnProcessReloading;
        public event Action OnChangeAmmo;
        public event Action<float> OnSendReloadingTime;
        
        public WeaponAmmoType AmmoType { get; }
        public int AmmoMagCurrentValue { get; set; }
        public int AmmoMagSize { get;  set; } 
        public int AmmoStockSize { get;  set; }
        public int AmmoStockCurrentValue { get; set; }
        public bool IsFullAmmo{ get;  }
        
        public float TimeToReloading { get; set; }
        public float TimeTacticalReloading{ get ; set ; }
        public bool IsReloadingProcess { get; set; }
        public bool IsTacticalReloading { get; set; }
        public bool GetReady { get; }
        
        public void Reloading();
        public void CancelReloading();
        
        public bool RestoreAmmo(WeaponAmmoType ammoType = null);
        public int AddedAmmo(int value,WeaponAmmoType ammoType = null);
        public bool RemoveAmmo();
        public int RemoveAmmo(int ammo);
        public void RemovePrecent(int precent = 1);
        
        
        public IReloading Clone();
    }
}