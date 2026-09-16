using System;
using _Project.Scripts._Common.Weapon.Base.Data;
using Common.Weapon.Damageable;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Projectile
{
    public interface IProjectile
    {
        public string Id { get; }
        public Action<IProjectile> DisableCallback { get; set; }
        public event Action<IDamageable> OnHitTarget;
        public GameObject GetGameObject();
        public void Disable();
        public void Active();
        public void SetProjectile(BulletInfo info);
        public void SetPositionAndRotation(Vector3 position,Quaternion rotation);

      
    }
}