using System;
using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Base.Data.Context;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Weapon.AdditionalWeaponModules
{
    
    [System.Serializable]
    public class WeaponSpread :  IWeaponDataModule
    {
        [SerializeField] private float _spread =  1;
        [SerializeField] private float _strenghtSpread = 0.7f;
        [SerializeField] private float _speedReturnToDefault = 5f;

        private float _currentSpread = 0;


        public float CurrentSpread 
        {
            get => _currentSpread;
            set 
            {
                _currentSpread = value;
                OnChangeSpread?.Invoke();
            }
        }
        
        public event Action OnChangeSpread = delegate { };

        public float MaxSpread => _spread;

        public WeaponSpread()
        {
        }
        public WeaponSpread( float  maxSpread)
        {
            _spread= maxSpread;
        }

        public void InstallModule(WeaponConfig config, BaseWeapon weapon)
        {
            WeaponSpread spread = new WeaponSpread(_spread);
            weapon.WeaponDataModules.Add(spread);
            weapon.OnUpdateModule += spread.UpdateModule;
            weapon.OnPrepareShotDirection += spread.BeforeUseShotDirection;
        }

        public void UpdateModule(float deltaTime)
        {
            ProcessSpread( deltaTime);
        }
        private void ProcessSpread(float deltaTime) 
        {
            if (_currentSpread > 0) 
            {
                CurrentSpread -= (deltaTime * _speedReturnToDefault);
            }
            if (_currentSpread <= 0 ) 
            {
                CurrentSpread = 0;
            }
    
        }
        public void BeforeUseShotDir(Transform dir,IProviderCollection collection)//сюда тоже передавать ProvaiderColliection , чтоб брать допустим процент разброса из СharacterStat 
        {      
            if (_currentSpread>0 )
            {
                _currentSpread = Mathf.Min(_currentSpread, _spread);


                float angleY = dir.localEulerAngles.y;
                angleY += (Random.Range(-_currentSpread, _currentSpread) );

                float angleX = dir.localEulerAngles.x;
                angleX += (Random.Range(-_currentSpread, _currentSpread) );

                dir.localRotation = Quaternion.Euler(angleX, angleY, 0);
            }
            CurrentSpread += _strenghtSpread;
        }

        public void BeforeUseShotDirection(ShotDirectionContext context,IProviderCollection collection)
        {
            if (_currentSpread > 0)
            {
                _currentSpread = Mathf.Min(_currentSpread, _spread);
                context.Direction = ApplySpread(context.Direction, _currentSpread);
            }

            CurrentSpread += _strenghtSpread;
        }

        private static Vector3 ApplySpread(Vector3 direction, float spread)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return direction;

            Vector3 normalizedDirection = direction.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, normalizedDirection);

            if (right.sqrMagnitude <= Mathf.Epsilon)
                right = Vector3.right;
            else
                right.Normalize();

            Quaternion yaw = Quaternion.AngleAxis(Random.Range(-spread, spread), Vector3.up);
            Quaternion pitch = Quaternion.AngleAxis(Random.Range(-spread, spread), right);

            return (yaw * pitch * normalizedDirection).normalized;
        }
    }
}
