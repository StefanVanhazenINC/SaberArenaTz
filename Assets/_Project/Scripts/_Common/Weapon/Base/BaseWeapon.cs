using System;
using System.Collections.Generic;
using _Project.Scripts._Common.Weapon.Base.AltWeaponUse;
using _Project.Scripts._Common.Weapon.Base.Aiming;
using _Project.Scripts._Common.Weapon.Base.ChainCondition;
using _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition;
using _Project.Scripts._Common.Weapon.Base.ChainCondition.Tools;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using _Project.Scripts._Common.Weapon.Base.TypeShooter;
using _Project.Scripts._Common.Weapon.Base.Visual;
using Common.Weapon.Damageable;
using Alchemy.Inspector;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace _Project.Scripts._Common.Weapon.Base
{
    [HideScriptField]
    public class BaseWeapon : MonoBehaviour
    {

       
       //прямо тут будет выбираться chain для условий , прям на префабе 
       [SerializeField] private Transform[] _shotDir;
       [SerializeField] private Transform _positionLineCheck;
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public BaseChainContext _chainContext;
       
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _getReadyChainList= new List<IChainCheck>();
       private IChainCheck _getReadyChain;
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _setAndReturnReadyChainList= new List<IChainCheck>();
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _setAndReturnReadyAdditionChainList= new List<IChainCheck>();
       private IChainCheck _setAndReturnReadyChain;
       private IChainCheck _setAndReturnReadyAdditionChain;

       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _cancelChainList= new List<IChainCheck>();
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _cancelChainAdditionList= new List<IChainCheck>();
    
       private IChainCheck _cancelChain;
       private IChainCheck _cancelAdditionChain;
       [FoldoutGroup("Chain condition")]
       [SerializeReference] public List<IChainCheck> _tryReloadingChainList= new List<IChainCheck>();
       private IChainCheck _tryReloadingChain;
      

       [SerializeReference] public IShooter _shooter;
       [SerializeReference] private IReloading _reloading;
       [SerializeReference] private IAltWeaponUse _altWeaponUse;
       [SerializeField] private WeaponConfig _config;
       
       [SerializeReference] public List<IWeaponDataModule> WeaponDataModules = new List<IWeaponDataModule>();

       private bool _isReadyShoot = true;
       private float _fireRate = 0.2f;
       private int _valueShot = 1;
       private float _lastShootTime = -100f;
       private bool _semiShotUsed;
      
       
       private bool _isReadyAfterDraw = false;
       private bool _isReadyHide = false;
       
       private BaseWeaponVisualContainer _weaponVisual;
       
       private BaseWeaponData WeaponData => _config.WeaponData;
       private readonly CompositeProvider _providerContext = new CompositeProvider(capacity: 8);
       private IProviderCollection _providerCollection;
       private IAimProvider _aimProvider;
       public WeaponConfig Config => _config;
       public IReloading Reloading => _reloading; 
       public IShooter ShotSpawner => _shooter;
       public IAltWeaponUse AltWeaponUse => _altWeaponUse;
       public BaseWeaponVisualContainer WeaponVisual => _weaponVisual;

       public bool IsTacticalReloading()
       {
           if (_reloading != null)
           {
               return _reloading.IsTacticalReloading;
           }
           return false;
       }
       public BaseWeaponVisualContainer WeaponVisualContainer { get; set; }
       public ShootingMode ShootingMode => WeaponData.ShootingMode;
       public Transform[] ShootDir { get => _shotDir; set => _shotDir = value; }
       public Transform PositionLineCheck { get =>  _positionLineCheck; set =>  _positionLineCheck= value; }

       public float LastTimeShot => _lastShootTime;
       public float FireRate => _fireRate;
       
       public bool IsReadyAfterDraw { get => _isReadyAfterDraw;  }
       public bool IsReadyHide { get => _isReadyHide; }
       public bool ReloadingReady 
       {
           get
           {
             
               if (_reloading!=null)
               {
                   return _reloading.GetReady; 
               }
               return true;
           }
       }
       public bool ShooterReadyShot
       {
           get
           {
               if (_shooter!=null)
               {
                   return _shooter.IsAnimationReadyShot; 
               }
               return false;
           }  
       }
       public bool IsReadyShoot
       {
           get => _isReadyShoot;
           set => _isReadyShoot = value;
       }
       public bool CanUse => GetReady();
       public BaseChainContext ChainContext => _chainContext;
       public event Action OnUseWeapon;
       public event Action OnHideWeapon;
       public event Action OnDrawWeapon;
       public event Action<Transform> OnUseShotDir;
       public event Action<Transform,IProviderCollection> OnUseShotDirAndProvider;
       public event Action<ShotDirectionContext,IProviderCollection> OnPrepareShotDirection;
       public event Action<IDamageable> OnHitTarget;
       public event Action<float> OnUpdateModule;

      

       
       private void OnEnable()
       {
           if (_shooter != null)
           {
               _shooter.OnHitTarget += OnHitTarget;
           }
       }
       private void OnDisable()
       {
           if (_shooter != null)
           {
               _shooter.OnHitTarget -= OnHitTarget;
           }
       }
       public void Update()
       {
           OnUpdateModule?.Invoke(Time.deltaTime);
       }
       public void Set(IShooter shooter, IReloading reloading, IAltWeaponUse altWeaponUse,WeaponConfig config)
       {
           _shooter  = shooter;
           _reloading = reloading;
           _altWeaponUse = altWeaponUse;
           IsReadyShoot = true;
           _semiShotUsed = false;
           
           _config = config;
           _fireRate =  WeaponData.FireRate;
           _valueShot =  WeaponData.ValueShot;

           if ( _altWeaponUse!=null)
           {
               _altWeaponUse.Setup(this);
           }

           
           BuildingChainCondition();
           _chainContext ??= new DefaultChainContext();
           _chainContext.Bind(this);

           
       }

       [Button]
       public void ResetWeapon()
       {
           IsReadyShoot = true;
           _shooter.IsAnimationReadyShot = true;
       }

       public void SetVisual( BaseWeaponVisualContainer weaponVisual)
       {
           _weaponVisual = weaponVisual;
       }

       public void SetProviderCollection(IProviderCollection providerCollection)
       {
           if (providerCollection!=null)
           {
               _providerCollection = providerCollection;
               _providerCollection?.Bind(_providerContext);
           }
       }

       public void SetAimProvider(IAimProvider aimProvider)
       {
           _aimProvider = aimProvider;
       }

       public void SetNewContext(BaseChainContext context)
       {
           _chainContext = context;
       }
#if UNITY_EDITOR
        [FoldoutGroup("Chain condition")]
        [Button]
        public void SetDefaultGetReadyChain()
        {
            _getReadyChainList.Clear(); 
            _getReadyChainList.Add(new CooldownCheck());
            _getReadyChainList.Add(new ReadyShotCheck());
            _getReadyChainList.Add(new ReadyShotInShooterCheck());
            EditorUtility.SetDirty(this);
        }
        [FoldoutGroup("Chain condition")]
        [Button]
        public void SetDefaultSetAndReturnReadyChain()
        {
            _setAndReturnReadyChainList.Clear();
            //пока без проверки на SingleActionRealidng
            _setAndReturnReadyChainList.Add(new CooldownCheck());
            _setAndReturnReadyChainList.Add(new ReadyShotCheck());
            _setAndReturnReadyChainList.Add(new ReadyShotInShooterCheck());
            _setAndReturnReadyChainList.Add(new ReloadingCheck());
            EditorUtility.SetDirty(this);

        }
        [FoldoutGroup("Chain condition")]
        [Button]
        public void SetDefaultCancelChain()
        {
            _cancelChainList.Clear();
            _cancelChainList.Add(new ReloadingCheck());
            EditorUtility.SetDirty(this);
        }
        [FoldoutGroup("Chain condition")]
        [Button]
        public void SetDefaultTryReloadingChain()
        {
            _tryReloadingChainList.Clear();
            _tryReloadingChainList.Add(new ReadyShotInShooterCheck());
            EditorUtility.SetDirty(this);
        }
    #endif
        
       private void BuildingChainCondition()
       {
           _getReadyChain = BuildChainTools.BuildChain(_getReadyChainList);
           _setAndReturnReadyChain = BuildChainTools.BuildChain(_setAndReturnReadyChainList);
           _cancelChain =  BuildChainTools.BuildChain(_cancelChainList);
           _tryReloadingChain = BuildChainTools.BuildChain(_tryReloadingChainList);
           _setAndReturnReadyAdditionChain = BuildChainTools.BuildChain(_setAndReturnReadyAdditionChainList);
           _cancelAdditionChain = BuildChainTools.BuildChain(_cancelChainAdditionList);
       }
       public void SetReady(bool value)
       {
           _isReadyAfterDraw = value;
           
       }
       public void SetReadyHide(bool value)
       {
           _isReadyHide = value;
       }
       public void Draw() 
       {
           
           if (_shooter!=null && _shooter.ShootAfterAnimation)
           {
              // _weaponVisual.SetReloadingTime(WeaponData.FireRate);//WeaponData.FireRate == 100% 
           }
           OnDrawWeapon?.Invoke();
       }
       public void Hide()
       {
           if (_reloading != null)
           {
               _reloading.CancelReloading();
           }
           AltCancelWeapon();
           _shooter.IsAnimationReadyShot = true;
           OnHideWeapon?.Invoke();
       }
    
       [Button]
       public void UseWeapon()
       {
           TryUseWeapon();
           // if (SetAndReturnReady())
           // {
           //     _lastShootTime = Time.time;
           //     if (_reloading != null)
           //     {
           //         _reloading.RemoveAmmo();
           //     }
           //     Fire();
           // }
           //
       }
       
       public bool TryUseWeapon()
       {
           if (!ShootingModeReady())
               return false;

           if (!SetAndReturnReady())
               return false;

           _lastShootTime = Time.time;

           if (_reloading != null)
               _reloading.RemoveAmmo();

           Fire();
           RegisterShootingModeUse();
           return true;
       }
       
       
       [Button]
       public void AltUseWeapon()
       {
           if (_altWeaponUse!=null)
           {
               WeaponContext ctx = CreateWeaponContext(null);
               _altWeaponUse.Use(ctx);
           }
       }
       public void AltCancelWeapon()
       {
           if (_altWeaponUse!=null)
           {
               _altWeaponUse.Cancel();
           }
       }


       private void Fire() 
       {
           if (!_shooter.ShootAfterAnimation)
           {
               for (int i = 0; i < _valueShot; i++)
               {
                   for (int j = 0; j < _shotDir.Length; j++)
                   {
                       FireWithContext(_shotDir[j]);
                   }
               }
               if (_shooter.IsAnimationReadyShot)
               {
                   _shooter.IsAnimationReadyShot = false;
                   OnUseWeapon?.Invoke();
               }
           }
           else
           {
               if (_shooter.IsAnimationReadyShot)
               {
                  
                   _shooter.IsAnimationReadyShot = false;
                   OnUseWeapon?.Invoke();
               }
           }
       }

       private WeaponContext CreateWeaponContext(Transform shotDir)
       {
           Vector3 shotDirection = CalculateShotDirection(shotDir);
           ShotDirectionContext shotDirectionContext = new ShotDirectionContext(shotDir, shotDirection);
           OnPrepareShotDirection?.Invoke(shotDirectionContext, _providerCollection);
           shotDirection = shotDirectionContext.Direction.sqrMagnitude > Mathf.Epsilon
               ? shotDirectionContext.Direction.normalized
               : shotDirection;
           _providerCollection?.PrepareShot(this, shotDir, shotDirection);
           WeaponContext ctx = new WeaponContext(shotDir, shotDirection,_positionLineCheck,_providerContext);
           return ctx;
       }

       private Vector3 CalculateShotDirection(Transform shotOrigin)
       {
           if (shotOrigin == null)
               return transform.forward;

           if (_aimProvider == null)
               return shotOrigin.forward;

           Vector3 aimPoint = _aimProvider.GetAimPoint(shotOrigin);
           Vector3 direction = aimPoint - shotOrigin.position;

           if (direction.sqrMagnitude <= Mathf.Epsilon)
               return shotOrigin.forward;

           return direction.normalized;
       }

       private void FireWithContext(Transform shotDir)
       {
           OnUseShotDir?.Invoke(shotDir);
           OnUseShotDirAndProvider?.Invoke(shotDir,_providerCollection);
          // _providerCollection?.PrepareShot(this, shotDir);
           //WeaponContext ctx = new WeaponContext(shotDir,_positionLineCheck,_providerContext);
           WeaponContext ctx = CreateWeaponContext(shotDir);
           if (_positionLineCheck != null)
           {
               _shooter.CheckLineBeforeShot(ctx);
           }
           _shooter.Shot(ctx);
       }

       public void FireAfterAnimation()
       {
               FireWithContext(_shotDir[0]);
       }

       public bool GetReady()
       {
           if (_getReadyChain != null)
           {
               ChainCheckResult result = _getReadyChain.Check(_chainContext);
               if (!result.Ok) return false;
               return true;
           }
           return false;
       }
       public bool SetAndReturnReady() 
       {
           if (_setAndReturnReadyChain!=null)
           {
               ChainCheckResult result = _setAndReturnReadyChain.Check(_chainContext);
               ChainCheckResult result1 = _setAndReturnReadyAdditionChain?.Check(_chainContext) ?? ChainCheckResult.Pass();
               if (!result.Ok) return false;
               if (!result1.Ok) return false;
               return true;
           }

           return false;
       }
       private bool ShootingModeReady()
       {
           switch (ShootingMode)
           {
               case ShootingMode.Semi:
                   return !_semiShotUsed;
               case ShootingMode.Auto:
               case ShootingMode.Burst:
               default:
                   return true;
           }
       }

       private void RegisterShootingModeUse()
       {
           if (ShootingMode == ShootingMode.Semi)
           {
               _semiShotUsed = true;
           }
       }

       public void CancelUseWeapon() 
       {
           _semiShotUsed = false;

           if (_cancelChain!=null)
           {
               ChainCheckResult result = _cancelChain.Check(_chainContext);
               if (result.Ok)
               {
                   _isReadyShoot = true;
               }
               ChainCheckResult result1 = _cancelAdditionChain?.Check(_chainContext) ?? ChainCheckResult.Fail("Cancel addition chain is empty");
               if (result1.Ok)
               {
                   _isReadyShoot = true;
               }
           }

       }
       public void TryReloading() 
       {
           if (_reloading!=null) 
           {
               ChainCheckResult result = _tryReloadingChain.Check(_chainContext);
               if (result.Ok)
               { 
                   _reloading.Reloading();
               }
           }
       }
     
    }
}
