using System.Collections.Generic;

namespace Common.Weapon.Damageable
{
    public sealed class DamagePipeline
    {
        private readonly List<IDamageModifier> _modifiers = new List<IDamageModifier>();

        public void AddModifier(IDamageModifier modifier)
        {
            if (modifier == null || _modifiers.Contains(modifier))
                return;

            _modifiers.Add(modifier);
            _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public void RemoveModifier(IDamageModifier modifier)
        {
            if (modifier == null)
                return;

            _modifiers.Remove(modifier);
        }

        public DamageContext Process(DamageInfo info, IDamageable target)
        {
            return Process(new DamageContext(info, target));
        }

        public DamageContext Process(DamageContext context)
        {
            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (context.IsRejected)
                    break;

                _modifiers[i].Modify(context);
            }

            return context;
        }

        public void Clear()
        {
            _modifiers.Clear();
        }
    }
}
