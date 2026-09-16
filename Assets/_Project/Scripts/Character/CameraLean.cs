using System;
using _Project.Scripts.Character.Data;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Character
{
    public class CameraLean : IInitializable, IDisposable
    {
        private CameraLeanData _cameraLeanData;

        [Header("Reference")]
        [Tooltip("Ориентация, относительно которой считаем 'вперёд' и 'вправо'. Обычно это тело игрока (yaw). " +
                 "Если не задано — возьмём родителя.")]
        private Transform _reference;

        private Vector3 _dampedAcceleration;
        private Vector3 _dampedAccelerationVel;

        private float _smoothSideStrength;
        private float _smoothForwardStrength;
        private Transform transform;

        #region DI
        
        [Inject]
        private  CameraLean(CameraLeanData data,Transform reference,Transform self)
        {
            _cameraLeanData = data;
            _reference = reference;
            transform = self;
        }

        public void Initialize()
        {
            _smoothSideStrength = _cameraLeanData.walkSideStrength;
            _smoothForwardStrength = _cameraLeanData.walkForwardStrength;
        }
        #endregion


        public void UpdateLean(float deltaTime, Vector3 acceleration, Vector3 up)
        {
            // 1) Берём только плоскостное ускорение (без "вверх/вниз")
            var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);

            // 2) Сглаживаем само ускорение
            var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude
                ? _cameraLeanData.attackDamping
                : _cameraLeanData.decayDamping;

            _dampedAcceleration = Vector3.SmoothDamp(
                current: _dampedAcceleration,
                target: planarAcceleration,
                currentVelocity: ref _dampedAccelerationVel,
                smoothTime: damping,
                maxSpeed: float.PositiveInfinity,
                deltaTime: deltaTime    
            );

            // 3) Сглаживаем силы отдельно
            float targetSide = _cameraLeanData.walkSideStrength;
            float targetForward = _cameraLeanData.walkForwardStrength;

            float t = 1f - Mathf.Exp(-_cameraLeanData.strengthResponse * deltaTime);
            _smoothSideStrength = Mathf.Lerp(_smoothSideStrength, targetSide, t);
            _smoothForwardStrength = Mathf.Lerp(_smoothForwardStrength, targetForward, t);

            // 4) Разложение ускорения на "вправо" и "вперёд" относительно reference
             //var basis = _reference != null ? _reference : transform.parent;
            var basis = _reference ;
            
            
            // if (basis == null)
            // {
            //     // На всякий случай, если вообще нет родителя.
            //     basis = transform;
            // }

            Vector3 localAccel = basis.InverseTransformDirection(_dampedAcceleration);
            // localAccel.x -> вправо/влево
            // localAccel.z -> вперёд/назад

            // 5) Углы (в градусах) отдельно для pitch и roll
            float pitch = -localAccel.z * _smoothForwardStrength; // вперёд => камера наклоняется вперёд (обычно минус)
            float roll = -localAccel.x * _smoothSideStrength; // вправо => наклон вправо (знак подстрой если нужно)

            
            
            
            pitch  = Mathf.Clamp( pitch,  -_cameraLeanData.maxRoll,  _cameraLeanData.maxRoll);
            roll = Mathf.Clamp(roll, -_cameraLeanData.maxPitch, _cameraLeanData.maxPitch);
            // 6) Применяем как локальный поворот (удобно, если камера под ригом)
            // Pitch вокруг X, Roll вокруг Z (в локальных осях)
            transform.localRotation = Quaternion.AngleAxis(pitch, Vector3.right) *
                                      Quaternion.AngleAxis(roll, Vector3.forward);
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}
