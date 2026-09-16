using System;
using Common.Weapon.Damageable;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace _Project.Scripts._Common.Weapon.Damageable.HealthSystem
{
    public class HealthSystem : IDamageable, IDamageModifierCollection, IDisposable
    {
        private readonly HealthData _data;
        private readonly DamagePipeline _damagePipeline = new DamagePipeline();

        public DamageInfo LastDamageInfo;
        public bool LastDamageWasTaken { get; private set; }
        public UnityEvent<DamageInfo> OnSendDamageInfoUnity;

        public event Action<DamageInfo> OnSendDamageInfo = delegate { };
        public event Action OnTakeDamage = delegate { };
        public event Action OnTakeHeal = delegate { };
        public event Action OnDeathDamaged = delegate { };
        public event Action OnDamagedNotTaken = delegate { };
        public event Action OnDeath = delegate { };

        public int Health
        {
            get => _data.Health;
            set => _data.Health = value;
        }

        public int MaxHealth => _data.MaxHealth;
        public bool IsDeath => _data.IsDeath;
        public bool DontDestroyBullet => true;
        public bool IsCanDamage => _data.IsCanDamage;

        [Inject]
        public HealthSystem(HealthData data)
        {
            _data = data;
            ResetHealth();
            _data.OnDeath += Death;
        }

        public void ResetHealth()
        {
            _data.ResetHealth();
        }

        public bool TakeHealth(int amount, bool isDeathReset = false)
        {
            if (!isDeathReset && IsDeath)
                return false;

            if (Health >= MaxHealth)
                return false;

            Health += amount;
            OnTakeHeal.Invoke();
            return true;
        }

        public void ChangeMaxHealth(int amount)
        {
            _data.SetParametrs(amount);
        }

        public void AddModifier(IDamageModifier modifier)
        {
            _damagePipeline.AddModifier(modifier);
        }

        public void RemoveModifier(IDamageModifier modifier)
        {
            _damagePipeline.RemoveModifier(modifier);
        }

        public void TakeDamage(DamageInfo info)
        {
            LastDamageWasTaken = false;

            if (info == null || IsDeath || info.Damage <= 0)
                return;

            DamageContext context = new DamageContext(info, this)
            {
                SourceTeam = info.SourceTeam
            };
            ApplyInfoModifiers(context);
            context = _damagePipeline.Process(context);

            if (context.IsRejected || context.Damage <= 0)
            {
                OnDamagedNotTaken?.Invoke();
                return;
            }

            OnTakeDamage?.Invoke();
            OnSendDamageInfo?.Invoke(context.Info);
            OnSendDamageInfoUnity?.Invoke(context.Info);

            if (Health - context.Damage <= 0)
                OnDeathDamaged?.Invoke();

            Health -= context.Damage;
            LastDamageInfo = context.Info;
            LastDamageWasTaken = true;
        }

        public void Dispose()
        {
            _data.OnDeath -= Death;
            OnDeath = null;
            OnSendDamageInfo = null;
            OnTakeDamage = null;
            OnTakeHeal = null;
            OnDeathDamaged = null;
            OnDamagedNotTaken = null;
            _damagePipeline.Clear();
            _data?.Dispose();
        }

        private void Death()
        {
            OnDeath?.Invoke();
        }

        private void ApplyInfoModifiers(DamageContext context)
        {
            for (int i = 0; i < context.Info.Modifiers.Count; i++)
            {
                if (context.IsRejected)
                    break;

                context.Info.Modifiers[i]?.Modify(context);
            }
        }

    }
}
