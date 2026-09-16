using System;
using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Damageable.HealthSystem
{
    [Serializable]
    public class HealthData : IDisposable 
    {
        [SerializeField] private string _descriptionTarget;
        [SerializeField] private int _maxHealth;
        
       
        [SerializeField][ReadOnly]private int _health;
        
        public event Action OnHealthChanged = delegate { };
        public event Action OnDeath = delegate { };
        public event Action OnDeathOnePact = delegate { };
        public event  Action OnInitHealth  = delegate { };
        public int MaxHealth => _maxHealth; 
        public int Health
        {   
            get => _health; 
            set
            {
                bool wasAlive = _health > 0;

                _health = value;

                if (_health < 0)
                    _health = 0;

                if (_health > _maxHealth)
                    _health = _maxHealth;

                OnHealthChanged?.Invoke();

                if (wasAlive && IsDeath)
                {
                    OnDeath?.Invoke();

                    OnDeathOnePact?.Invoke();
                    OnDeathOnePact = null;
                }
                
            }

        }

        public void Death()
        {
            Health = 0;
        }

        public bool IsCanDamage => !IsDeath;
        public bool IsDeath  => _health <= 0;
        public bool HealthIsMax => _health >= _maxHealth;
        public string DescriptionTarget  => _descriptionTarget;

        public HealthData(int maxHealth, string descriptionTarget)
        {
            _maxHealth = maxHealth;
            _descriptionTarget = descriptionTarget;
        }

        public void ResetHealth()
        {
            Health = _maxHealth;
            OnInitHealth?.Invoke();
        }

        public void SetParametrs( int maxHealth)
        {
            _maxHealth = maxHealth;
            if (_health > _maxHealth)
                _health = _maxHealth;

            OnHealthChanged?.Invoke();
        }

        public void Reset()
        {
            OnHealthChanged = delegate { };
            OnDeath = delegate { };
            OnDeathOnePact = delegate { };
            OnInitHealth = delegate { };
        }
        public HealthData Clone()
        {
            return new HealthData(_maxHealth, _descriptionTarget);
        }
        public void Dispose()
        {
            Reset();
        }
    }
}