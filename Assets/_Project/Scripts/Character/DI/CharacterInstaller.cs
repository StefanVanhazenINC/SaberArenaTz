using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Aiming;
using _Project.Scripts.Character.Data;
using Common.BaseComponent;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace _Project.Scripts.Character.DI
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField] private CharacterConfig _characterConfig;
        [Header("Camera")]
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _cameraTransform;
        [Header("Camera-Lean")]
        [SerializeField] private Transform _cameraLeanReference;
        [SerializeField] private Transform _cameraLeanTransform;
   
        
        [Header("Weapon ")]
        [SerializeField] private Transform _weaponParent;
        [SerializeField] private Transform _lookAt;
        
        [Header("Component")]
        [SerializeField] private TeamComponent _teamComponent;

        public override void InstallBindings()
        {
            RegisterComponents();
            InstallCharacter();
            InstallCamera();
            InstallWeapon();
            Container.Bind<PlayerDeathHandler>().AsSingle().NonLazy();
        }

        private void RegisterComponents()
        {
            Container.BindInstance(_teamComponent).AsSingle().NonLazy();
        }

        private void InstallWeapon()
        {
            PlayerWeaponHolderData weaponConfig = _characterConfig.WeaponData;
            WeaponHolder.Data runtimeWeaponData = weaponConfig.CreateRuntimeWeaponData(_weaponParent, _lookAt);
            AmmoHolder[] runtimeAmmoHolders = weaponConfig.CreateRuntimeAmmoHolders();

            Container.BindInstance(runtimeWeaponData).AsSingle().NonLazy();
            Container.BindInstance(runtimeAmmoHolders).AsSingle().NonLazy();
            
            WeaponAimSettings aimSettings = _characterConfig.WeaponData.AimSettings;
            Transform aimTransform = _cameraTransform != null ? _cameraTransform : _lookAt;
            IAimProvider aimProvider = new TransformAimProvider(
                aimTransform,
                aimSettings.MaxDistance,
                aimSettings.AimMask,
                aimSettings.TriggerInteraction);

            Container.Bind<IAimProvider>().FromInstance(aimProvider).AsSingle().NonLazy();
            Container.Bind<WeaponHolder>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerWeaponHolder>().AsSingle().NonLazy();
        }

        private void InstallCharacter()
        {
           CharacterData newData = _characterConfig.CharacterData.Clone() ;
           Container.BindInstance(newData).AsSingle().NonLazy();
            
        }
        
     
        private void InstallCamera()
        {
            Container.BindInterfacesAndSelfTo<PlayerCamera>().AsSingle().WithArguments(_characterConfig.CameraData,_cameraTarget,_cameraTransform);
            Container.BindInterfacesAndSelfTo<CameraLean>().AsSingle().WithArguments(_characterConfig.LeanData,_cameraLeanReference,_cameraLeanTransform);
            
        }

    

      
    }
}
