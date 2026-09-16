using System;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using _Project.Scripts._Common.Weapon.Base.Projectile;
using Common.BaseComponent;
using Common.Weapon.Damageable;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts._Common.Weapon.Base.TypeShooter.BaseTypeShooter
{
    [System.Serializable]
    public sealed class ProjectileShooter : IShooter
    {
        [SerializeField] private Projectile.Projectile _projectilePrefab;

        [Header("Settings")] [SerializeField] private int _projectileInShoot = 1;
        [SerializeField] private float _lifeTimeProjectile = 15f;
        [SerializeField] private float _speedProjectile = 10;
        [Header("Physics")] [SerializeField] private LayerMask _lineChekcMask;
        [SerializeField] private float _distanceCheck = 1.5f;
        [SerializeField] private float _lineCheckRadius = 0.08f;
        [Header("Animation")] 
        [SerializeField] private bool _shootAfterAnimation = false;
        private bool _animationAnimationReady = true;

        private TeamComponent _team;
        private IPoolProjectile _pool;

        private int _currentShot = 0;

        public bool ShootAfterAnimation
        {
            get => _shootAfterAnimation;
        }

        public bool IsAnimationReadyShot
        {
            get => _animationAnimationReady;
            set
            {
                _currentShot = 0;
                _animationAnimationReady = value;
                if (value)
                {
                    OnChangeWeaponReady?.Invoke();
                }
            }
        }

        public event Action<IDamageable> OnHitTarget;
        public event Action OnChangeWeaponReady;

        public ProjectileShooter()
        {
        }

        public ProjectileShooter(ProjectileShooter other, IPoolProjectile pool)
        {
           

            _projectileInShoot = other._projectileInShoot;
            _lifeTimeProjectile = other._lifeTimeProjectile;
            _speedProjectile = other._speedProjectile;
            _lineChekcMask = other._lineChekcMask;
            _distanceCheck = other._distanceCheck;
            _lineCheckRadius = other._lineCheckRadius;
            _shootAfterAnimation = other._shootAfterAnimation;
            _animationAnimationReady = other._animationAnimationReady;
            _projectilePrefab = other._projectilePrefab;
            _team = other._team;
            _pool = pool;
        }

        public TeamComponent Team { get => _team; }

        public IShooter Clone(IPoolProjectile pool)
        {
            return new ProjectileShooter(this, pool);
        }

        public event Action OnStartShoot;

        public void SetTeam(TeamComponent team)
        {
            _team = team;
        }

        public void Shot(WeaponContext ctx)
        {
            if (!_animationAnimationReady) return;
            OnStartShoot?.Invoke();
            IProjectile t_projectTile = null;
            DamageInfo data = new DamageInfo();
            //
            if (ctx.Provider != null) 
                ctx.Provider.TryGet<DamageInfo>(out data);
            
            
            if (!_lineCheck)
            {
                t_projectTile = _pool.GetObjectInPool(_projectilePrefab);
                t_projectTile.SetPositionAndRotation(
                    ctx.ShotOrigin.position,
                    Quaternion.LookRotation(ctx.ShotDirection));
            }

            BulletInfo bulletInfo;
            bulletInfo = new BulletInfo(ctx.ShotDirection, data.Damage, 0,_speedProjectile,_lifeTimeProjectile, _team);
            
            if (t_projectTile!=null)
            {
                t_projectTile.SetProjectile(info: bulletInfo);
                t_projectTile.OnHitTarget += OnHitTarget;
                t_projectTile.Active(); 
            }
            else
            {
                TakeDamageInLine(info: bulletInfo, ctx.ShotDirection);
            }
        }

        private IDamageable _lineTarget;
        private const int MaxLineHitBufferSize = 64;
        private Collider[] _lineHits = new Collider[8];
        private bool _lineCheck = false;

        public void CheckLineBeforeShot(WeaponContext ctx)
        {
            
            _lineCheck = false;
            _lineTarget = null;
            int count = OverlapLineCheck(ctx);
            Debug.DrawRay(ctx.ShotOrigin.position,ctx.ShotDirection *  _distanceCheck, Color.red, 0.5f);
    
            for (int i = 0; i < count; i++)
            {
                if (DamageTargetResolver.TryResolve(_lineHits[i], _team, out _lineTarget, out _))
                {
                    _lineCheck = true;
                    return;
                }
            }
        }

        private int OverlapLineCheck(WeaponContext ctx)
        {
            Vector3 start = ctx.ShotOrigin.position;
            Vector3 end = start + ctx.ShotDirection * _distanceCheck;
            float radius = Mathf.Max(0f, _lineCheckRadius);
            int count;

            do
            {
                count = Physics.OverlapCapsuleNonAlloc(
                    start,
                    end,
                    radius,
                    _lineHits,
                    _lineChekcMask,
                    QueryTriggerInteraction.UseGlobal);

                if (count < _lineHits.Length || _lineHits.Length >= MaxLineHitBufferSize)
                    return count;

                Array.Resize(ref _lineHits, Mathf.Min(_lineHits.Length * 2, MaxLineHitBufferSize));
            }
            while (true);
        }

        private void TakeDamageInLine(BulletInfo info,Vector3 direction)
        {
            if (_lineTarget == null || !_lineTarget.IsCanDamage)
                return;

            DamageInfo infoDamage = new DamageInfo(info.damage, direction)
            {
                SourceTeam = info.team
            };

            _lineTarget.TakeDamage(infoDamage);
            OnHitTarget?.Invoke(_lineTarget);
        }
    }
}
