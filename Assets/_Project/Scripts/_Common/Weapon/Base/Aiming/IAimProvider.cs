using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Aiming
{
    public interface IAimProvider
    {
        Vector3 GetAimPoint(Transform shotOrigin);
    }
}
