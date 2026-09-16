using System.Collections.Generic;
using Common.BaseComponent;

namespace Common.Weapon.Damageable
{
    using UnityEngine;

    public class DamageInfo
    {
        public int Damage;
        public Vector3? Position;
        public Vector3 Direction;
        public TeamComponent SourceTeam;
        public readonly List<IDamageModifier> Modifiers = new List<IDamageModifier>();

        public DamageInfo()
        {
            
        }

        public DamageInfo(int damage, Vector3 direction, Vector3? point = null,
            IDamageModifier modifier = null)
        {
            Damage = damage;
            Direction = direction;
            Position = point;
            AddModifier(modifier);
        }

        public DamageInfo AddModifier(IDamageModifier modifier)
        {
            if (modifier != null)
                Modifiers.Add(modifier);

            return this;
        }
    }
}
