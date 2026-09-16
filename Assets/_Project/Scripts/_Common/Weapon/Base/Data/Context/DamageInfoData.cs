using Common.Weapon.Damageable;

namespace _Project.Scripts._Common.Weapon.Base.Data.Context
{
    public struct DamageInfoData
    {
       public readonly DamageInfo Value;
       public DamageInfoData(DamageInfo value) => Value = value;
    }
}