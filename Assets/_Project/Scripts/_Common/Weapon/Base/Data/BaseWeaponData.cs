using System;
using System.Collections.Generic;
using _Project.Scripts._Common.Weapon.Base.AltWeaponUse;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;
using _Project.Scripts._Common.Weapon.Base.TypeShooter;
using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Data
{
    [Serializable]
    public class BaseWeaponData
    {
        [Header("BaseParam")] 
        public string Name;
        public Sprite Icon;
        public string Id;
        
        [Header("ShotParam")] 
        public float FireRate = 0.2f;
        public int ValueShot = 1;
        public int Damage = 1;
        public ShootingMode ShootingMode ;
        
        [Header("BattleParam")] 
        [SerializeReference]public IShooter ShooterPrototype; 
        [SerializeReference]public IReloading ReloadingPrototype;
        [SerializeReference]public IAltWeaponUse AltWeaponUse;
        
        [SerializeReference] public List<IWeaponDataModule> WeaponDataModules = new List<IWeaponDataModule>();


        public T GetModule<T>() where T : class, IWeaponDataModule
        {
            for (int i = 0; i < WeaponDataModules.Count; i++)
            {
                if (WeaponDataModules[i] is T module)
                    return module;
            }
            return null;
        }

        public bool TryGetModule<T>(out T module) where T : class, IWeaponDataModule
        {
            module = GetModule<T>();
            return module != null;
        }
    }
}
