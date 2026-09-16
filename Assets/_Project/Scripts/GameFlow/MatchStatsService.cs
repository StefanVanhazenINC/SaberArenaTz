using System;
using System.Collections.Generic;
using _Project.Scripts._Common.GameFlow;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;
using _Project.Scripts.Character;
using _Project.Scripts.Enemies;
using Common.BaseComponent;
using Common.Weapon.Damageable;

namespace _Project.Scripts.GameFlow
{
    public sealed class MatchStatsService : IMatchStatsService, IDisposable
    {
        private readonly IGameTimerService _gameTimer;
        private readonly Dictionary<HealthSystem, Action<DamageInfo>> _subscriptions =
            new Dictionary<HealthSystem, Action<DamageInfo>>();

        public int DamageDealt { get; private set; }
        public int DamageTaken { get; private set; }

        public MatchStatsService(IGameTimerService gameTimer)
        {
            _gameTimer = gameTimer;
        }

        public void Reset()
        {
            DamageDealt = 0;
            DamageTaken = 0;
        }

        public void ClearRegistrations()
        {
            foreach (var subscription in _subscriptions)
                subscription.Key.OnSendDamageInfo -= subscription.Value;

            _subscriptions.Clear();
        }

        public void RegisterPlayer(Player player)
        {
            if (player == null)
                return;

            Subscribe(player.HealthSystem, AddTakenDamage);
            Subscribe(player.ArmorSystem, AddTakenDamage);
        }

        public void RegisterEnemy(SlimeEnemy enemy)
        {
            if (enemy == null)
                return;

            Subscribe(enemy.HealthSystem, AddDealtDamage);
        }

        public MatchStatsSnapshot GetSnapshot()
        {
            return new MatchStatsSnapshot(DamageDealt, DamageTaken, _gameTimer.ElapsedTime);
        }

        public void Dispose()
        {
            ClearRegistrations();
        }

        private void Subscribe(HealthSystem healthSystem, Action<DamageInfo> handler)
        {
            if (healthSystem == null || _subscriptions.ContainsKey(healthSystem))
                return;

            _subscriptions.Add(healthSystem, handler);
            healthSystem.OnSendDamageInfo += handler;
        }

        private void AddTakenDamage(DamageInfo info)
        {
            if (info?.SourceTeam == null || info.SourceTeam.TeamType != TypeTeam.Enemy)
                return;

            DamageTaken += info.Damage;
        }

        private void AddDealtDamage(DamageInfo info)
        {
            if (info?.SourceTeam == null || info.SourceTeam.TeamType != TypeTeam.Player)
                return;

            DamageDealt += info.Damage;
        }
    }
}
