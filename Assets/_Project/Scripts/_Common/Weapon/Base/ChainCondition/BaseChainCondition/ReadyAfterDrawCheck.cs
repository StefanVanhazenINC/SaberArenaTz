namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class ReadyAfterDrawCheck: ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
            => ctx.Weapon.IsReadyAfterDraw? ChainCheckResult.Pass() : ChainCheckResult.Fail("Weapon not ready after Draw");
    }
}