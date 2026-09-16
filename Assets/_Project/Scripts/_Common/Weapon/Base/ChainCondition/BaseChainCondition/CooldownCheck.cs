using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class CooldownCheck: ChainCheckBase
    {
        //Убрать кулдаун , если выстрел идет в анимации ?
        //доп параметр для кулдауна ? (минусовое значение использовать  ctx.Weapon.FireRate = -1)


        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
            => Time.time < ctx.Weapon.LastTimeShot + ctx.Weapon.FireRate
                ? ChainCheckResult.Fail("Cooldown")
                : ChainCheckResult.Pass();
    }
}