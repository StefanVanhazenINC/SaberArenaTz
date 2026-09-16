namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class ReloadingCheck : ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
            => ctx.Weapon.ReloadingReady ? ChainCheckResult.Pass() : ChainCheckResult.Fail("ReloadingNotEnd");

    }
}