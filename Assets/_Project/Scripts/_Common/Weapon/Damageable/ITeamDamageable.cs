using Common.BaseComponent;

namespace Common.Weapon.Damageable
{
    public interface ITeamDamageable : IDamageable
    {
        TeamComponent TeamComponent { get; }
    }
}
