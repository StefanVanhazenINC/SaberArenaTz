namespace _Project.Scripts.GameFlow
{
    public readonly struct MatchStatsSnapshot
    {
        public MatchStatsSnapshot(int damageDealt, int damageTaken, float elapsedTime)
        {
            DamageDealt = damageDealt;
            DamageTaken = damageTaken;
            ElapsedTime = elapsedTime;
        }

        public int DamageDealt { get; }
        public int DamageTaken { get; }
        public float ElapsedTime { get; }
    }
}
