using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.TypeReloading.BaseTypeReloading
{
    [System.Serializable]
    public sealed class MagReloading : IReloading
    {
        [Header("Mag Size")]
        [SerializeField] private int _magSize = 7;
        [SerializeField] private int _stockSize = 49;
        [SerializeField] private float _delayAfterShot = 0.1f;
        
        [Header("Time Reloading")]
        [SerializeField] private float _timeReloading = 1;
        [SerializeField] private float _timeTacticalReloading = 1;
        [SerializeField] private  float _delayReloading = 0;

        [Header("Ammo Infinity")] 
        [SerializeField] private bool _infinityStock;
        [SerializeField] private bool _infinityMag;
        
        private int _ammoMagCurrentValue;
        private CancellationTokenSource _reloadCts;

        public event Action OnStartReloading;
        public event Action OnEndReloading;
        public event Action OnProcessReloading;
        public event Action OnChangeAmmo;
        public event Action<float> OnSendReloadingTime;

        public WeaponAmmoType AmmoType => null;

        public int AmmoMagCurrentValue   
        {
            get => _ammoMagCurrentValue;
            set 
            {
                _ammoMagCurrentValue = value;
                OnChangeAmmo?.Invoke();
            }
        
        }
        public int AmmoMagSize { get; set; }
        public int AmmoStockSize { get; set; }
        public int AmmoStockCurrentValue { get; set; }
        public bool IsFullAmmo => AmmoMagCurrentValue == AmmoMagSize && AmmoStockSize == AmmoStockSize;
        public float TimeToReloading { get; set; }
        public float TimeTacticalReloading { get; set; }
        public bool IsReloadingProcess { get; set; }
        public bool IsTacticalReloading { get; set; }
        public bool GetReady => (AmmoMagCurrentValue != 0) && (IsReloadingProcess != true );

        public MagReloading()
        {
        }

        public MagReloading(MagReloading other)
        {
            _magSize = other._magSize;
            _stockSize = other._stockSize;

            _timeReloading = other._timeReloading;
            _timeTacticalReloading = other._timeTacticalReloading;
            _delayAfterShot = other._delayAfterShot;
            _delayReloading = other._delayReloading;
            
            AmmoMagSize = _magSize;
            AmmoStockSize = _stockSize;

            AmmoMagCurrentValue = AmmoMagSize;
            AmmoStockCurrentValue = AmmoStockSize;

            _infinityMag = other._infinityMag;
            _infinityStock = other._infinityStock;
            IsReloadingProcess = false;
            TimeToReloading = _timeReloading;
            TimeTacticalReloading = _timeTacticalReloading;
            
        }
        public IReloading Clone()
        {
            return new MagReloading(this);
        }
        public void Reloading()
        {
            if (AmmoMagCurrentValue >= AmmoMagSize || AmmoStockCurrentValue <= 0) return;
            if (!IsReloadingProcess ) 
            {
                IsReloadingProcess = true;
                OnStartReloading?.Invoke();
                _reloadCts?.Cancel();
                _reloadCts = new CancellationTokenSource();
                ReloadingProcessAsync(_reloadCts.Token).Forget();
            }
        }
        private async UniTask ReloadingProcessAsync(CancellationToken ct = default)
        {
            try
            {
                await UniTask.WaitForSeconds(_delayReloading, cancellationToken: ct);

                float currentTime = AmmoMagCurrentValue <= 0 ? TimeToReloading : TimeTacticalReloading;
                OnSendReloadingTime?.Invoke(currentTime);

                await UniTask.WaitForSeconds(currentTime, cancellationToken: ct);

                // на всякий случай (хотя UniTask уже кинет отмену, но это безопасно)
                if (ct.IsCancellationRequested) return;

                ReloadingMag();
            }
            catch (OperationCanceledException)
            {
                // отменили — просто выходим
            }
            finally
            {
                // если нужно сбрасывать флаг по завершению/отмене
                IsReloadingProcess = false;
            }

        }
        private void ReloadingMag() 
        {
            int mag;
            int needAmmo =  AmmoMagSize - AmmoMagCurrentValue; 
            if (needAmmo <= AmmoStockCurrentValue) 
            {
                if (!_infinityStock)
                {
                    AmmoStockCurrentValue -= needAmmo;
                }
                mag = AmmoMagCurrentValue +  needAmmo ; 
            }
            else  
            {
                mag = AmmoMagCurrentValue +   AmmoStockCurrentValue  ;   
                AmmoStockCurrentValue = 0;      
            }
            

            AmmoMagCurrentValue = mag;
            IsReloadingProcess = false;
            OnEndReloading?.Invoke();
        }
        public void CancelReloading()
        {
            if (_reloadCts != null)
            {
                _reloadCts.Cancel();
                _reloadCts.Dispose();
                _reloadCts = null;
            }
        }

        public bool RestoreAmmo(WeaponAmmoType ammoType = null)
        {
            AmmoMagCurrentValue = AmmoMagSize;
            AmmoStockCurrentValue = AmmoStockSize;
            OnChangeAmmo?.Invoke();
            return true;
        }

        public int AddedAmmo(int value,WeaponAmmoType ammoType = null)
        {
            if (value==0)
            {
                return 0;
            }
            int tempStock = AmmoStockSize - AmmoStockCurrentValue;//сколько не хватате в стоке
            if (value > tempStock)
            {
                AmmoStockCurrentValue = AmmoStockSize;
            }
            else
            {
                AmmoStockCurrentValue = AmmoStockCurrentValue + value;
            }
            OnChangeAmmo?.Invoke();
            return 0;
        }

        public bool RemoveAmmo()
        {
            if (GetReady)
            {
                if (!_infinityMag)
                {
                    AmmoMagCurrentValue--;
                }

                return true;
            }
            return false;
        }

        public int RemoveAmmo(int ammo)
        {
            int temp = 0;
            if ( ammo <= _ammoMagCurrentValue)
            {
                temp = ammo;
                if (!_infinityMag)
                {
                    AmmoMagCurrentValue = AmmoMagCurrentValue  - temp;
                }
            }
            else
            {
                temp = ammo - AmmoMagCurrentValue;
                ammo -= temp;
                if (!_infinityMag)
                {
                    AmmoMagCurrentValue = 0;
                }

                if ( ammo > 0)
                {
                    if ( ammo <= AmmoStockCurrentValue)
                    {
                        temp += ammo;
                
                        if (!_infinityStock)
                        {
                            AmmoStockCurrentValue = AmmoStockCurrentValue - ammo;
                        }
                    }
                    else
                    {
                        temp += (ammo - AmmoStockCurrentValue);
                        ammo -= temp;
                        if (!_infinityStock)
                        {
                            AmmoStockCurrentValue = 0;
                        }
                    }
                }


            }

            return temp;
        }

        public void RemovePrecent(int precent = 1)
        {
            AmmoMagCurrentValue =Mathf.CeilToInt( AmmoMagSize * (precent/100f)); 
            AmmoStockCurrentValue = Mathf.CeilToInt( AmmoMagSize * (precent/100f)); 
        }

      
    }
}
