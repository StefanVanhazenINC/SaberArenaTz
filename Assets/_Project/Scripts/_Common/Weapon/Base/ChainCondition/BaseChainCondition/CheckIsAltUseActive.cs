namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]

    public class CheckIsAltUseActive: ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
        {
            if (ctx.Weapon.AltWeaponUse==null)
            {
                return ChainCheckResult.Pass();
            }
            else
            {
                if (ctx.Weapon.AltWeaponUse.Activate)
                {
                    return ChainCheckResult.Fail("Alt use Weapon Active");
                }
                else
                {
                    return  ChainCheckResult.Pass();
                }

            }
        }
    }
}