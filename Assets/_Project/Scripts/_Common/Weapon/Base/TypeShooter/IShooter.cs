using System;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using Common.BaseComponent;
using Common.Weapon.Damageable;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.TypeShooter
{
    public interface IShooter
    {
        public bool IsAnimationReadyShot { get; set; }
        public bool ShootAfterAnimation { get; }
        public event Action OnStartShoot;
        public event Action<IDamageable> OnHitTarget;   
        public TeamComponent Team { get; }
        public IShooter Clone(IPoolProjectile pool);
        public void Shot(WeaponContext ctx);
        public void SetTeam(TeamComponent team);
        public void CheckLineBeforeShot(WeaponContext ctx);

       
    }
}
