using System;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Damageable.HealthSystem.UI
{
    public class HealthBarPresenter : IDisposable
    {
        private HealthBarView _healthBarView;
        private HealthData _healthSystem;

        public HealthBarPresenter(HealthBarView healthBarView, HealthData healthData)
        {
            _healthBarView = healthBarView;
            _healthSystem = healthData;

            _healthSystem.OnHealthChanged += ChangeHealth;
            ChangeHealth();
        }

        public void ResetHealth() 
        {
        }
        public void ChangeHealth() 
        {
            _healthBarView.ChangeMainSlider((float)_healthSystem.Health / (float)_healthSystem.MaxHealth);
            _healthBarView.SetTextHealth(_healthSystem.Health.ToString(), _healthSystem.MaxHealth.ToString());
        }
        public void Dispose()
        {
            GameObject.Destroy(_healthBarView);
            _healthBarView = null;
            _healthSystem.OnHealthChanged -= ChangeHealth;
        }
    }
}