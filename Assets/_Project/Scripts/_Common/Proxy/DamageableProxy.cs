using System;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;
using Alchemy.Inspector;
using Common.BaseComponent;
using Common.Weapon.Damageable;
using UnityEngine;

namespace Common.Proxy
{
    [HideScriptField]
    public class DamageableProxy : MonoBehaviour, ITeamDamageable, IDamageModifierCollection
    {
        [SerializeField] private int _damageMultiplayer = 1;
        [SerializeField] private bool _dontDestroyBullet = false;

        private readonly DamagePipeline _damagePipeline = new DamagePipeline();
        private TeamComponent _teamComponent;
        private HealthSystem _healthComponent;
        private int _defaultDamageMultiplayer;

        public Func<DamageInfo, DamageInfo> OnDamageResist;
        public event Action OnTakeDamage = delegate { };
        public event Action OnDeath = delegate { };

        public TeamComponent TeamComponent
        {
            get => _teamComponent;
            set => _teamComponent = value;
        }

        public HealthSystem HealthComponent
        {
            get => _healthComponent;
            set => _healthComponent = value;
        }

        public bool DontDestroyBullet
        {
            get => _dontDestroyBullet;
            set => _dontDestroyBullet = value;
        }

        public bool IsCanDamage => _healthComponent != null && _healthComponent.IsCanDamage;

        public void Constuct(TeamComponent teamComponent, HealthSystem healthComponent)
        {
            _teamComponent = teamComponent;
            _healthComponent = healthComponent;
            _healthComponent.OnDeath += Death;
            _defaultDamageMultiplayer = _damageMultiplayer;
        }

        public void TakeDamage(DamageInfo info)
        {
            if (info == null || _healthComponent == null)
                return;

            DamageInfo damageInfo = OnDamageResist?.Invoke(info) ?? info;
            DamageContext context = new DamageContext(damageInfo, this)
            {
                SourceTeam = damageInfo.SourceTeam,
                TargetTeam = _teamComponent
            };

            _damagePipeline.Process(context);
            context.Damage *= _damageMultiplayer;

            if (context.IsRejected || context.Damage <= 0)
                return;

            _healthComponent.TakeDamage(context.Info);
            if (_healthComponent.LastDamageWasTaken)
                OnTakeDamage?.Invoke();
        }

        public void AddModifier(IDamageModifier modifier)
        {
            _damagePipeline.AddModifier(modifier);
        }

        public void RemoveModifier(IDamageModifier modifier)
        {
            _damagePipeline.RemoveModifier(modifier);
        }

        public void ResetProxy()
        {
            if (gameObject != _teamComponent.gameObject)
                gameObject.SetActive(true);
        }

        public void SetDamageMultiplayer(int value)
        {
            _damageMultiplayer = value;
        }

        public void DefaultDamageMultiplayer()
        {
            _damageMultiplayer = _defaultDamageMultiplayer;
        }

        [Button]
        public void TestDame()
        {
            TakeDamage(new DamageInfo(1, Vector3.zero));
        }

        private void Death()
        {
            OnDeath?.Invoke();
            if (gameObject != _teamComponent.gameObject)
                gameObject.SetActive(false);
        }
    }
}
