using System;
using System.Collections;
using _Project.Scripts._Common.Weapon.Base.Data;
using Common.Weapon.Damageable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts._Common.Weapon.Base.Projectile
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private TrailRenderer _trailRender;
        [SerializeField] private float _delayToDestoy = 0.2f;
        [SerializeField] private SphereCollider _colldier;

        [SerializeField] private LayerMask _collisionMask;

        [SerializeField] private Vector2 _bulletHoleScaleRange;
        [SerializeField] private string _id;
        private GameObject _visual;
        private float _defaultSize = 0 ;

        private Vector3 _hitPosition;
        private Vector3 _hitNormal;
        BulletInfo _info;
        private Coroutine _delayCoroutine;
        private Vector3 lastPosition;
        
        private bool _enabled = true;
        
        private Action<IProjectile> _disableCallback;
        
        public Vector3 HitPosition { get => _hitPosition;  }
        public Vector3 HitNormal { get => _hitNormal;  }
        public SphereCollider Colldier { get => _colldier;}
        public BulletInfo Info { get => _info; }

        public event Action<Collision> OnCollision;
        public event Action<Collider> OnCollider;
        public event Action OnCollisionNoArg;
        public event Action OnDisableAction;
        public event Action OnEnableAction;
        public event Action<IDamageable> OnHitTarget;
        public event Action<DamageInfo> OnAfterCreateDamageInfo;
        public string Id => _id;

        Action<IProjectile> IProjectile.DisableCallback
        {
            get => _disableCallback;
            set => _disableCallback = value;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetProjectile(BulletInfo info)
        {


            _info = info;
            if (_visual) 
            {
                _visual.SetActive(false);
            }
            if (_defaultSize <= 0)
            {
                _defaultSize = _colldier.radius;
            }
            else 
            {
                _colldier.radius = _defaultSize ;

            }

            _rb.linearVelocity = transform.forward * _info.speed;
           
            if (info.visual) 
            {
                _colldier.radius = info.size;
                if (!_visual)
                {
                    _visual = Instantiate(info.visual, transform);
                }
                else if (_visual) 
                {
                    if (_visual == info.visual)
                    {
                        _visual.SetActive(true);
                    }
                    else 
                    {
                        Destroy(_visual.gameObject);
                        _visual = Instantiate(info.visual, transform);
                    }
                }
            }

            lastPosition = transform.position;
        }
        private void OnEnable()
        {
            _enabled = true;
            OnEnableAction?.Invoke();
            _delayCoroutine = StartCoroutine(OverLifeTime());
        }
        private IEnumerator OverLifeTime() 
        {
            yield return new WaitForSeconds(_info.lifeTime);
            DestoryProjecTile();
        }
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_delayToDestoy);
            DestoryProjecTile();
        }
        private void FixedUpdate()
        {
            if (_enabled)
            {
                HandleCollision();
            }
        }
        public void HandleCollision()
        {
            Vector3 direction = _rb.linearVelocity.normalized;

            float distance = (lastPosition -transform.position  ).magnitude;
            if (Physics.SphereCast(lastPosition,_colldier.radius, direction, out RaycastHit hit, distance, _collisionMask))
            {
                ProcessHit(hit);
            }


            lastPosition = transform.position;

        }
        private void ProcessHit(RaycastHit hit)
        {
            Vector3 _hitPosition = hit.point;
            Vector3 _hitNormal = hit.normal;
            bool DestroyBullet = true;

            bool hitDamageable = DamageTargetResolver.TryResolve(hit.collider, _info.team, out IDamageable target, out DestroyBullet);
            if (hitDamageable)
            {
                DamageInfo info = new DamageInfo(_info.damage, transform.forward, hit.point)
                {
                    SourceTeam = _info.team
                };

                OnAfterCreateDamageInfo?.Invoke(info);
                info.Position = _hitPosition;
                target.TakeDamage(info);
                OnHitTarget?.Invoke(target);
            }
            else if (DestroyBullet)
            {

                Vector3 tangent = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
                Debug.DrawRay(_hitPosition, Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized * 2, Color.blue,5f);
                Debug.DrawRay(_hitPosition, hit.normal * 3, Color.blue,5f);
            }

            Debug.DrawRay(_hitPosition, _hitNormal * 2, Color.red,5f);

            if (DestroyBullet)
            {
                DestoryProjecTile();
            }

            OnCollisionNoArg?.Invoke();
            OnCollider?.Invoke(hit.collider); 
        }
        private void DestoryProjecTile()
        {
            if (gameObject.activeSelf)
            {
                StopCoroutine(_delayCoroutine);
                 _enabled = false;
                _rb.linearVelocity = Vector3.zero;
                StartCoroutine(DestroyAfterDelay());
                _disableCallback?.Invoke(this);
            }
        }

      
        public void Disable()
        {
            OnHitTarget = null;
            OnDisableAction?.Invoke();
            _trailRender.Clear();
            _enabled = false;
            gameObject.SetActive(false);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;  
        }

        public void Active()
        {
            gameObject.SetActive(true);
        }

      
    }
}
