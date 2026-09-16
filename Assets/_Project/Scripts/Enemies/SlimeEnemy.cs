using System.Collections;
using System;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;
using Common.BaseComponent;
using Common.Proxy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace _Project.Scripts.Enemies
{
    public sealed class SlimeEnemy : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TeamComponent _teamComponent;
        [SerializeField] private DamageableProxy _damageableProxy;
        [SerializeField] private SlimeDashDamageHitbox _dashDamageHitbox;
        [SerializeField] private SlimeVisual _visual;
        [SerializeField] private NavMeshAgent _agent;

        [Header("Health")]
        [SerializeField] private HealthData _healthData = new HealthData(30, "Slime");

        [Header("Target")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _stopDistance = 1.1f;

        [Header("Dash")]
        [SerializeField] private float _prepareDuration = 0.25f;
        [SerializeField] private float _dashDuration = 0.35f;
        [SerializeField] private float _dashSpeed = 8f;
        [SerializeField] private float _recoveryDuration = 0.8f;
        [SerializeField] private float _rotationSpeed = 720f;

        [Header("Events")]
        [SerializeField] private UnityEvent _onDamageTaken = new UnityEvent();
        [SerializeField] private UnityEvent _onDeath = new UnityEvent();
        [SerializeField] private UnityEvent _onDashStarted = new UnityEvent();
        [SerializeField] private UnityEvent _onDashFinished = new UnityEvent();

        private HealthSystem _healthSystem;
        private Coroutine _behaviourRoutine;
        private bool _isDead;

        public event Action<SlimeEnemy> Died = delegate { };

        public HealthData HealthData => _healthData;
        public HealthSystem HealthSystem => _healthSystem;
        public Transform Target
        {
            get => _target;
            set => SetTarget(value);
        }

        private void Awake()
        {
            if (_agent != null)
            {
                _agent.updateRotation = false;
                _agent.stoppingDistance = _stopDistance;
                _agent.speed = _dashSpeed;
                _agent.isStopped = true;
            }

            _healthSystem = new HealthSystem(_healthData);

            if (_damageableProxy != null)
                _damageableProxy.Constuct(_teamComponent, _healthSystem);

            if (_dashDamageHitbox != null)
            {
                _dashDamageHitbox.Construct(_teamComponent);
                _dashDamageHitbox.SetDamageEnabled(false);
            }

            _healthSystem.OnTakeDamage += HandleDamageTaken;
            _healthData.OnDeath += HandleDeath;
        }

        private void OnEnable()
        {
            _behaviourRoutine = StartCoroutine(BehaviourRoutine());
        }

        private void OnDisable()
        {
            StopActiveCoroutines();
            StopMovement();

            if (_dashDamageHitbox != null)
                _dashDamageHitbox.SetDamageEnabled(false);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void Spawn(Transform target)
        {
            _isDead = false;
            SetTarget(target);
            _healthSystem?.ResetHealth();
        }

        private IEnumerator BehaviourRoutine()
        {
            while (!_isDead)
            {
                if (!HasTargetToDash())
                {
                    yield return null;
                    continue;
                }

                yield return PrepareDash();
                yield return Dash();

                if (_recoveryDuration > 0f)
                    yield return new WaitForSeconds(_recoveryDuration);
            }
        }

        private IEnumerator PrepareDash()
        {
            _visual?.PrepareDash();

            float timer = 0f;

            while (timer < _prepareDuration)
            {
                FaceTarget(Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator Dash()
        {
            if (_target == null)
                yield break;

            _visual?.DashProcess();
            _onDashStarted.Invoke();

            if (_dashDamageHitbox != null)
                _dashDamageHitbox.SetDamageEnabled(true);

            StartDashMovement();

            float timer = 0f;
            while (timer < _dashDuration)
            {
                UpdateDashDestination();
                FaceTarget(Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            StopMovement();
            _visual?.DashEnd();

            if (_dashDamageHitbox != null)
                _dashDamageHitbox.SetDamageEnabled(false);

            _onDashFinished.Invoke();
        }

        private bool HasTargetToDash()
        {
            if (_target == null)
                return false;

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;

            return toTarget.sqrMagnitude > _stopDistance * _stopDistance;
        }

        private Vector3 GetDirectionToTarget()
        {
            if (_target == null)
                return Vector3.zero;

            Vector3 direction = _target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return Vector3.zero;

            return direction.normalized;
        }

        private void FaceTarget(float deltaTime)
        {
            Vector3 direction = GetDirectionToTarget();
            if (direction == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * deltaTime);
        }

        private void Move(Vector3 direction, float distance)
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.Move(direction * distance);
                return;
            }

            transform.position += direction * distance;
        }

        private void StartDashMovement()
        {
            if (_agent == null || !_agent.isOnNavMesh)
                return;

            _agent.speed = _dashSpeed;
            _agent.stoppingDistance = _stopDistance;
            _agent.isStopped = false;
            UpdateDashDestination();
        }

        private void UpdateDashDestination()
        {
            if (_target == null || _agent == null || !_agent.isOnNavMesh)
                return;

            _agent.SetDestination(_target.position);
        }

        private void StopMovement()
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }

        private void HandleDamageTaken()
        {
            _onDamageTaken.Invoke();
        }

        private void HandleDeath()
        {
            if (_isDead)
                return;

            _isDead = true;
            StopActiveCoroutines();
            StopMovement();

            if (_dashDamageHitbox != null)
                _dashDamageHitbox.SetDamageEnabled(false);

            _onDeath.Invoke();
            Died.Invoke(this);
            gameObject.SetActive(false);
        }

        private void StopActiveCoroutines()
        {
            if (_behaviourRoutine != null)
            {
                StopCoroutine(_behaviourRoutine);
                _behaviourRoutine = null;
            }
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnTakeDamage -= HandleDamageTaken;
                _healthSystem.Dispose();
            }

            if (_healthData != null)
            {
                _healthData.OnDeath -= HandleDeath;
                _healthData.Dispose();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_teamComponent == null)
                _teamComponent = GetComponent<TeamComponent>();

            if (_damageableProxy == null)
                _damageableProxy = GetComponentInChildren<DamageableProxy>();

            if (_dashDamageHitbox == null)
                _dashDamageHitbox = GetComponentInChildren<SlimeDashDamageHitbox>();

            if (_visual == null)
                _visual = GetComponentInChildren<SlimeVisual>();

            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();
        }
#endif
    }
}
