using System.Collections.Generic;
using Common.BaseComponent;
using Common.Weapon.Damageable;
using UnityEngine;

namespace _Project.Scripts.Enemies
{
    public sealed class SlimeDashDamageHitbox : MonoBehaviour
    {
        [SerializeField] private Collider _damageCollider;
        [SerializeField] private int _damage = 10;
        [SerializeField] private bool _damageTargetOncePerDash = true;

        private readonly HashSet<IDamageable> _damagedTargets = new HashSet<IDamageable>();
        private TeamComponent _sourceTeam;
        private bool _isDamageEnabled;

        private void Awake()
        {
            if (_damageCollider == null)
                _damageCollider = GetComponent<Collider>();

            SetDamageEnabled(false);
        }

        public void Construct(TeamComponent sourceTeam)
        {
            _sourceTeam = sourceTeam;
        }

        public void SetDamageEnabled(bool value)
        {
            _isDamageEnabled = value;

            if (_damageCollider != null)
                _damageCollider.enabled = value;

            if (value)
                _damagedTargets.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryDamage(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryDamage(other);
        }

        private void TryDamage(Collider other)
        {
            if (!_isDamageEnabled)
                return;

            if (!DamageTargetResolver.TryResolve(other, _sourceTeam, out IDamageable target, out _))
                return;

            if (_damageTargetOncePerDash && !_damagedTargets.Add(target))
                return;

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 direction = other.bounds.center - transform.position;
            if (direction.sqrMagnitude > 0.0001f)
                direction.Normalize();

            DamageInfo damageInfo = new DamageInfo(_damage, direction, hitPoint)
            {
                SourceTeam = _sourceTeam
            };

            target.TakeDamage(damageInfo);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_damageCollider == null)
                _damageCollider = GetComponent<Collider>();

            if (_damageCollider != null)
                _damageCollider.isTrigger = true;
        }
#endif
    }
}
