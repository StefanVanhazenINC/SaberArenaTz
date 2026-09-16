using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;

namespace Common.Weapon.Damageable.Modifiers
{
    public sealed class ArmorDamageModifier : IDamageModifier
    {
        private readonly HealthSystem _armor;

        public ArmorDamageModifier(HealthSystem armor)
        {
            _armor = armor;
        }

        public int Order => DamageModifierPriority.Armor;

        public void Modify(DamageContext context)
        {
            if (_armor == null || !_armor.IsCanDamage || context.Damage <= 0)
                return;

            int armorBefore = _armor.Health;
            var armorDamage = new DamageInfo(context.Damage, context.Info.Direction, context.Info.Position)
            {
                SourceTeam = context.SourceTeam
            };

            _armor.TakeDamage(armorDamage);

            int absorbedDamage = armorBefore - _armor.Health;
            context.Damage -= absorbedDamage;
        }
    }
}
