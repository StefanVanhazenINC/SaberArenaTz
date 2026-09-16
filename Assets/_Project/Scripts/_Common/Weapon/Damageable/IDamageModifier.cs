namespace Common.Weapon.Damageable
{
    public interface IDamageModifier
    {
        int Order { get; }
        void Modify(DamageContext context);
    }
}
