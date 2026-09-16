namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.BaseChainCondition
{
    public class ExampleChainCondition: ChainCheckBase
    {
        
        /*
         public bool CanPass(BaseChainContext ctx, out string reason)
        {
            // Проверка требует расширенный контекст:
            if (ctx is not ChainWithVisualContainerContext c)
            {
                reason = null;
                return true; // не применимо — пропускаем
            }

            // допустим, спринт лежит в Visual/Owner/etc
            bool isSprinting = /* достать откуда-то #1#;
            if (c.BlockWhileSprinting && isSprinting)
            {
                reason = "Sprinting";
                return false;
            }

            reason = null;
            return true;
        }
        */
        protected override ChainCheckResult Evaluate(BaseChainContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}