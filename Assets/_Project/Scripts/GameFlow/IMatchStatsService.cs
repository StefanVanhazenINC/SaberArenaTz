using _Project.Scripts.Character;
using _Project.Scripts.Enemies;

namespace _Project.Scripts.GameFlow
{
    public interface IMatchStatsService
    {
        int DamageDealt { get; }
        int DamageTaken { get; }

        void Reset();
        void ClearRegistrations();
        void RegisterPlayer(Player player);
        void RegisterEnemy(SlimeEnemy enemy);
        MatchStatsSnapshot GetSnapshot();
    }
}
