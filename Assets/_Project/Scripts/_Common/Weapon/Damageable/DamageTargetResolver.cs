using Common.BaseComponent;
using UnityEngine;

namespace Common.Weapon.Damageable
{
    public static class DamageTargetResolver
    {
        public static bool TryResolve(
            Collider collider,
            TeamComponent sourceTeam,
            out IDamageable target,
            out bool destroyBullet)
        {
            target = null;
            destroyBullet = true;

            if (collider == null)
                return false;

            target = collider.GetComponentInParent<IDamageable>();
            if (target == null)
                return false;

            if (target is ITeamDamageable teamDamageable && sourceTeam != null)
            {
                TeamComponent targetTeam = teamDamageable.TeamComponent;
                if (targetTeam != null &&
                    (!targetTeam.CheckWrongTeam(sourceTeam.TeamType) || targetTeam.Self(sourceTeam)))
                {
                    destroyBullet = false;
                    target = null;
                    return false;
                }
            }

            destroyBullet = !target.DontDestroyBullet;
            return target.IsCanDamage;
        }
    }
}
