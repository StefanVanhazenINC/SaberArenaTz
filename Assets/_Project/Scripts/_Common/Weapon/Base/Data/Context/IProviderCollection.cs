using _Project.Scripts._Common.Weapon.Base.Data.Context;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base
{
    public interface IProviderCollection
    {
        void Bind(CompositeProvider composite);

        void PrepareShot(BaseWeapon weapon, Transform shotOrigin, Vector3 shotDirection);

    }
}
