using System;
using _Project.Scripts._Common.Weapon.Base.TypeReloading;

namespace _Project.Scripts.Character.Data
{
    [Serializable]
    public class AmmoSnapshotData
    {
        public WeaponAmmoType AmmoType;
        public int CurrentAmount;
    }
}
