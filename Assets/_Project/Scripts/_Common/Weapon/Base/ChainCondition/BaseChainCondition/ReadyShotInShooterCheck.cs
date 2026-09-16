namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    [System.Serializable]
    public class ReadyShotInShooterCheck : ChainCheckBase
    {
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
        => ctx.Weapon.ShooterReadyShot ? ChainCheckResult.Pass() : ChainCheckResult.Fail("Shooter not ready");
    }
}
