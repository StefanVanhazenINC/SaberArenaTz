namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class ObjectIsEnabledCheck : ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
            => ctx.Weapon.isActiveAndEnabled ? ChainCheckResult.Pass() : ChainCheckResult.Fail("Object is disabled");
    }
}