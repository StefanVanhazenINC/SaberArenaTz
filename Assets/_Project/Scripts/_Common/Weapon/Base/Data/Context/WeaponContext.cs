using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Data.Context
{
    public sealed class ShotDirectionContext
    {
        public readonly Transform ShotOrigin;
        public Vector3 Direction;

        public ShotDirectionContext(Transform shotOrigin, Vector3 direction)
        {
            ShotOrigin = shotOrigin;
            Direction = direction;
        }
    }

    public readonly struct WeaponContext 
    {
        public readonly Transform ShotOrigin;
        public readonly Vector3 ShotDirection;
        public readonly Transform AdditionLineCheck;
        public readonly IWeaponContextProvider? Provider;

        public Transform ShotDir => ShotOrigin;
        
            
        public WeaponContext(
            Transform shotOrigin,
            Vector3 shotDirection,
            Transform additionLineCheck,
            IWeaponContextProvider? provider = null)
        {
            ShotOrigin = shotOrigin;
            ShotDirection = shotDirection.sqrMagnitude > Mathf.Epsilon
                ? shotDirection.normalized
                : Vector3.forward;
            Provider = provider;
            AdditionLineCheck = additionLineCheck;
        }
        
    }
}
