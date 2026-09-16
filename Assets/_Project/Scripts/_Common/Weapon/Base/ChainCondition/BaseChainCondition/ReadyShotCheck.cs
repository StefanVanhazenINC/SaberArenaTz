namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class ReadyShotCheck: ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
            => ctx.Weapon.IsReadyShoot ? ChainCheckResult.Pass() : ChainCheckResult.Fail("Weapon not ready");
    }
}
   
