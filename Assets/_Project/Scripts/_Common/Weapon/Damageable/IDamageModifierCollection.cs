namespace Common.Weapon.Damageable
{
    public interface IDamageModifierCollection
    {
        void AddModifier(IDamageModifier modifier);
        void RemoveModifier(IDamageModifier modifier);
    }
}
