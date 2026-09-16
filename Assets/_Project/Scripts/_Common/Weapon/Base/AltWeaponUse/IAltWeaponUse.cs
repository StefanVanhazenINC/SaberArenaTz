using _Project.Scripts._Common.Weapon.Base.Data.Context;

namespace _Project.Scripts._Common.Weapon.Base.AltWeaponUse
{
    public interface IAltWeaponUse
    {
        public bool Activate { get; }
        public void Setup(BaseWeapon weapon);

        public void Use(WeaponContext ctx);
        public void Cancel();

        public IAltWeaponUse Clone();
    }
}