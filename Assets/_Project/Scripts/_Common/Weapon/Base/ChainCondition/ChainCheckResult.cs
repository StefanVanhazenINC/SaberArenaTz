namespace _Project.Scripts._Common.Weapon.Base.ChainCondition
{
    public readonly struct ChainCheckResult
    {
        public readonly bool Ok;
        public readonly string Reason;
        
        private ChainCheckResult(bool ok, string reason)
        {
            Ok = ok;
            Reason = reason;
        }
        public static ChainCheckResult Pass() => new(true, "");
        public static ChainCheckResult Fail(string reason) => new(false, reason);
        
    }
}