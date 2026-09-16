namespace Common.Weapon.Damageable
{
    using System.Collections;
    using System.Collections.Generic;


    public interface IDamageable
    {
        public bool DontDestroyBullet { get; }
        public bool IsCanDamage { get;  }
        public void TakeDamage(DamageInfo info);
    }
}
