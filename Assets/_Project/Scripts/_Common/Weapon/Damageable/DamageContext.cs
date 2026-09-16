using Common.BaseComponent;
using UnityEngine;

namespace Common.Weapon.Damageable
{
    public sealed class DamageContext
    {
        public DamageContext(DamageInfo info, IDamageable target)
        {
            Info = info;
            Target = target;
        }

        public DamageInfo Info { get; }
        public IDamageable Target { get; }
        public TeamComponent SourceTeam { get; set; }
        public TeamComponent TargetTeam { get; set; }
        public bool IsRejected { get; private set; }

        public int Damage
        {
            get => Info.Damage;
            set => Info.Damage = Mathf.Max(0, value);
        }

        public void Reject()
        {
            IsRejected = true;
        }
    }
}
