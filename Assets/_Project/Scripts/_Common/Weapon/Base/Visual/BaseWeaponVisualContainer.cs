
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts._Common.Weapon.Base.Visual
{
    
    public class VisualContainerContext
    {
        
    }
    
    [HideScriptField]
    public class BaseWeaponVisualContainer : MonoBehaviour
    {
        [SerializeField] private Transform[] _shotDir;
        [SerializeField] private Transform _positionLineCheck;

        [SerializeField] protected Animator _animator;
        [SerializeField] private WeaponAnimationController _weaponAnimation;
        
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnShoot;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnDraw;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnHide;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnAfterShot ;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnReloading;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnReloadingEmpty;
        [FoldoutGroup("Events")]
        [SerializeField] public UnityEvent OnShotInAniamtion;
        

        #region  KeyAniamtion
        private readonly int shotHash = Animator.StringToHash("Shot");
        private readonly int afterShotHash = Animator.StringToHash("bolt_action");
        private readonly int IdleHash = Animator.StringToHash("static_idle");
        private readonly int DrawHash = Animator.StringToHash("Draw");
        private readonly int HideHash = Animator.StringToHash("Hide");
    
        private readonly int StartReloadHash = Animator.StringToHash("Reload_start");
        private readonly int EndReloadHash = Animator.StringToHash("Reload_end");
    
        private readonly int Reload_tacticalHash = Animator.StringToHash("Reload_tactical");
        private readonly int Reload_emptyHash = Animator.StringToHash("Reload_empty");
        private readonly int Run_startHash = Animator.StringToHash("Run_start");
        private readonly int Run_endHash = Animator.StringToHash("Run_end");
        private readonly int shot_adsHash = Animator.StringToHash("Shot_ads");
        private readonly int ReloadingFloatHash= Animator.StringToHash("Reloading");
        #endregion

        public WeaponAnimationController WeaponAnimation => _weaponAnimation;

        public Animator WeaponAnimator
        {
            get => _animator;
            set => _animator = value;
        }

        public Transform[] ShotDir => _shotDir;
        public Transform PositionLineCheck => _positionLineCheck;


        public virtual void SetupOverrideAnimator(VisualContainerContext visualContainerContext)
        {
            
        }

        public virtual void PlayCustomAnimation(string nameAnimation)
        {
            _animator?.Play(nameAnimation,0,0f);
        }
        public virtual void CrossFadeCustomAnimation(string nameAnimation, float duration = 0)
        {
            _animator?.CrossFade(nameAnimation,duration);
        }

        public virtual void SetTrigget(string nameTrigger)
        {
            _animator.SetTrigger(nameTrigger);
        }

        public virtual void Shoot()
        {
            _animator?.Play(shotHash,0,0f);
        }

        public void ShootEvent()
        {
            OnShoot?.Invoke();
        }
        public void ShotInAnimation()
        {
            OnShotInAniamtion?.Invoke();
        }
        public void AfterShoot()
        {
            _animator?.Play(afterShotHash);
        }

        public void AfterShotEvent()
        {
            OnAfterShot?.Invoke();
        }

        public virtual void Draw()
        {
            _animator?.Play(DrawHash);
        }

        public virtual void DrawEvent()
        {
            OnDraw?.Invoke();
        }

        public virtual void HideWeapon()
        {
            _animator?.Play(HideHash);
        }

        public virtual void HideEvent()
        {
            OnHide?.Invoke();

        }

        public virtual void SetReloadingTime(float newTime)
        {
            _animator?.SetFloat(ReloadingFloatHash, newTime);
        }

        public virtual void Realoding(bool p0)
        {
            if (!p0)
            {    
                _animator?.Play(Reload_tacticalHash,0,0f);
                OnReloading?.Invoke();
            }
            else
            { 
                _animator?.Play(Reload_emptyHash,0,0f);
                OnReloadingEmpty?.Invoke();
            }
        }
        public virtual void StartRealoding()
        {
            _animator?.Play(StartReloadHash,0,0f);
        }

        public virtual void ProcessRealoding()
        {
            _animator?.Play(Reload_tacticalHash,0,0f);
        }

        public virtual void EndRealoding()
        {
            _animator?.Play(EndReloadHash,0,0f);

        }
    }
}