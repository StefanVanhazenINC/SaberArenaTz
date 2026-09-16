namespace Common.Weapon.Damageable
{
    public static class DamageModifierPriority
    {
        public const int Early = -1000;
        public const int Resistance = -100;
        public const int Armor = 0;
        public const int Multiplier = 100;
        public const int Late = 1000;
    }
}
