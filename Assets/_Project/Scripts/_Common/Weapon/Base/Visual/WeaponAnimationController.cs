using System;
using _Project.Scripts._Common.AnimationUtillity;
using _Project.Scripts.Weapon.Visual;
using Alchemy.Inspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts._Common.Weapon.Base.Visual
{
    [HideScriptField]
    public class WeaponAnimationController : MonoBehaviour
    {
        [Flags]
        public enum AnimationList
        {
            Shot = 1,
            AfterShot = 2,
            Reloading = 4,
            Hide = 8,
            Draw = 16,
            ShotInAnimation = 32,
        }

        [SerializeField] private BaseWeaponVisualContainer _visualContainer;
        [SerializeField] private AnimationDispatcher _animationDispatcher;
        [SerializeField, Tooltip("Если надо сделать ближний бой то поставь ShotInAnimation")] private AnimationList _animationList;
        [SerializeField] private bool _simpleReloading = false;

        private BaseWeapon _baseWeapon;
        private Action _setReadyAction;
        private Action _setReadyHideAction;
        private Action _setAnimationReadyShotAction;
        private Action _afterShotReadyAction;
        private UnityAction _onShootReadyAction;

        public AnimationDispatcher AnimationDispatcher
        {
            get=> _animationDispatcher;
            set => _animationDispatcher = value;
        }
        public bool AfterShotAnimation => _animationList.HasFlag(AnimationList.AfterShot);


#if UNITY_EDITOR

        [Button]
        public void SetSetting()
        {
            _visualContainer = GetComponent<BaseWeaponVisualContainer>();
            _animationDispatcher = GetComponent<AnimationDispatcher>();
            EditorUtility.SetDirty(this);
        }
#endif
        public void SetVisualContainer(BaseWeaponVisualContainer container)
        {
            _visualContainer = container;
        }

        public void SetupAnimationDispatcher(AnimationDispatcher dispatcher)
        {
            if (dispatcher == null)
                return;

            RemoveDispatcherBindings();
            _animationDispatcher = dispatcher;

            if (_animationList.HasFlag(AnimationList.ShotInAnimation )) 
            {
                //Взывать метод, который проиграет атаку, но вызавет выстрел по ключу из анимации 
                //Надо сначалп переключать что то в BaseWeapon, чтоб вызывался эвент атаки , но не вытсрел ( переключается в конфиге ,В настройках шутера )
                //подписать эвент , что после ключа вытсрела идет вызов выстрела 
                
                //BaseWeapon.UseWeapon => BaseWeapon.Fire (запускается эвент) =>
                //срабатывает подписка baseWeapon.OnUseWeapon += Shoot и запускается анимация => в анимации срабатывает ключ Shot и вызывается выстрел 
               dispatcher.SubscribeOnEvent("Shot", _baseWeapon.FireAfterAnimation);
               dispatcher.SubscribeOnEvent("ShotInAnimation",  ShotInAnimation);

            }
       

            if (_animationList.HasFlag(AnimationList.Draw)) // Подписываем анимацию по ключу 
            {
                dispatcher.SubscribeOnEvent("Draw", _setReadyAction);
            }
            else
            {
                _baseWeapon.SetReady(true);
            }

            if (_animationList.HasFlag(AnimationList.Hide))
            {
                dispatcher.SubscribeOnEvent("Hide", _setReadyHideAction);
            }

            if (_animationList.HasFlag(AnimationList.Shot) && !_animationList.HasFlag(AnimationList.ShotInAnimation))//анимация выстрела 
            {
                if (!_animationList.HasFlag(AnimationList.AfterShot) )//Если нет анимаци пост выстрела, то готов , сразу посел выстрела 
                {
                    dispatcher.SubscribeOnEvent("Shot", _setAnimationReadyShotAction);
                }
                else
                {
                    dispatcher.SubscribeOnEvent("Shot", AfterShot);
                }
            }

            if (_animationList.HasFlag(AnimationList.AfterShot))
            {
                dispatcher.SubscribeOnEvent("AfterShot", _afterShotReadyAction);
                dispatcher.SubscribeOnEvent("AfterShot", AfterShot);
            }

            if (!_animationList.HasFlag(AnimationList.Shot))
            {
                _visualContainer.OnShoot.RemoveListener(_onShootReadyAction);
                _visualContainer.OnShoot.AddListener(_onShootReadyAction);
            }

        }

        public void SetAnimation(BaseWeapon baseWeapon)
        {
            if (_baseWeapon != null)
            {
                _baseWeapon.OnDrawWeapon -= Draw;
                _baseWeapon.OnHideWeapon -= Hide;
                _baseWeapon.OnUseWeapon -= Shoot;

                if (_baseWeapon.Reloading != null)
                    _baseWeapon.Reloading.OnStartReloading -= Realoding;
            }

            RemoveDispatcherBindings();

            _baseWeapon = baseWeapon;
            _setReadyAction = SetBaseWeaponReady;
            _setReadyHideAction = SetBaseWeaponReadyHide;
            _setAnimationReadyShotAction = SetAnimationReadyShot;
            _afterShotReadyAction = SetAnimationReadyShot;
            _onShootReadyAction = SetAnimationReadyShot;

            baseWeapon.OnDrawWeapon += Draw;
            baseWeapon.OnHideWeapon += Hide;
            baseWeapon.OnUseWeapon += Shoot;

            if (_animationDispatcher)
            {
                SetupAnimationDispatcher(_animationDispatcher);
            }

            if (_animationList.HasFlag(AnimationList.Reloading))
            {
                if (baseWeapon.Reloading != null)
                    baseWeapon.Reloading.OnStartReloading += Realoding;
            }
        }

        private void RemoveDispatcherBindings()
        {
            if (_animationDispatcher == null)
                return;

            if (_baseWeapon != null)
                _animationDispatcher.UnsubscribeOnEvent("Shot", _baseWeapon.FireAfterAnimation);

            _animationDispatcher.UnsubscribeOnEvent("ShotInAnimation", ShotInAnimation);
            _animationDispatcher.UnsubscribeOnEvent("Draw", _setReadyAction);
            _animationDispatcher.UnsubscribeOnEvent("Hide", _setReadyHideAction);
            _animationDispatcher.UnsubscribeOnEvent("Shot", _setAnimationReadyShotAction);
            _animationDispatcher.UnsubscribeOnEvent("Shot", AfterShot);
            _animationDispatcher.UnsubscribeOnEvent("AfterShot", _afterShotReadyAction);
            _animationDispatcher.UnsubscribeOnEvent("AfterShot", AfterShot);

            if (_visualContainer != null && _onShootReadyAction != null)
                _visualContainer.OnShoot.RemoveListener(_onShootReadyAction);
        }

        private void SetBaseWeaponReady()
        {
            _baseWeapon.SetReady(true);
        }

        private void SetBaseWeaponReadyHide()
        {
            _baseWeapon.SetReadyHide(true);
        }

        private void SetAnimationReadyShot()
        {
            _baseWeapon.ShotSpawner.IsAnimationReadyShot = true;
        }
        public void Shoot() 
        {
            if (_animationList.HasFlag(AnimationList.Shot))
            {
                _visualContainer.Shoot();
            }

            _visualContainer.ShootEvent();
        }

        public void ShotInAnimation()
        {
            _visualContainer.ShotInAnimation();
        }

        public void AfterShot()
        {
            if (_animationList.HasFlag(AnimationList.AfterShot))
            {
                _visualContainer.AfterShoot(); 
            }

            _visualContainer.AfterShotEvent();
        }
            
        public void PlayCustomAnimation(string nameAnimation)
        {
            _visualContainer.PlayCustomAnimation(nameAnimation);
        }
        public void CrossFadeCustomAnimation(string nameAnimation, float duration = 0)
        {
            _visualContainer.CrossFadeCustomAnimation(nameAnimation,duration);
        }

        public void Draw() 
        {
            if (_animationList.HasFlag(AnimationList.Draw))
            {
                _baseWeapon.SetReadyHide(true);
                _visualContainer.Draw();
            }
            else 
            {
                _baseWeapon.SetReadyHide(false);
                _baseWeapon.SetReady(true);
            }

            _visualContainer.DrawEvent();
        }
        public void Hide() 
        {

            if (_animationList.HasFlag(AnimationList.Hide))
            {
                _baseWeapon.SetReady(false);
                _baseWeapon.SetReadyHide(false);
                _visualContainer.HideWeapon();
            }
            else
            {
                _baseWeapon.SetReadyHide(true);
                _baseWeapon.SetReady(false);
            }
            _visualContainer.HideEvent();

        }
        public void Realoding() 
        {
            if (_simpleReloading )
            {
                _visualContainer.Realoding(false);
            }
            else
            {
                _visualContainer.Realoding(!_baseWeapon.IsTacticalReloading());
            }

          
        }
        public void StartReloading() 
        {
            _visualContainer.StartRealoding();
        }
   
        public void ProcessReloading() 
        { 
            _visualContainer.ProcessRealoding();

        }
        public void EndReloading() 
        {
            _visualContainer.EndRealoding();
        }
    }
}
