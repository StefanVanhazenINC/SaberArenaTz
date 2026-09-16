namespace Common.ProcedureAnimation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class WeaponSway : MonoBehaviour
    {
        [SerializeField] private Transform _body;

        [Header("Reference (что движется/крутится)")] [SerializeField]
        private Transform _reference; // камера/голова/руки/корень

        [Header("Position sway source")]
        [Tooltip("TRUE = positional sway от движения. FALSE = positional sway от поворота.")]
        [SerializeField]
        private bool _positionFromMovement = true;

        [Header("Tilt Sway (Position)")] [SerializeField]
        private float _amount = 1f;

        [SerializeField] private float _maxSway = 0.05f;
        [SerializeField] private float _smoothAmount = 10f;

        [Header("Rotational Sway")] [SerializeField]
        private float _tiltAmount = 1f;

        [SerializeField] private float _maxTiltSway = 5f;
        [SerializeField] private float _smoothAmountTilt = 10f;

        [SerializeField] private bool _tiltDirX = true;
        [SerializeField] private bool _tiltDirY = true;
        [SerializeField] private bool _tiltDirZ = true;

        [Header("Auto tuning (Movement -> Position)")]
        [Tooltip("Насколько sway реагирует на скорость движения reference (м/с -> sway units)")]
        [SerializeField]
        private float _moveToPosSway = 0.02f;

        [Header("Auto tuning (Turn -> Position/Rotation)")]
        [Tooltip("Сколько positional sway давать от поворота (deg/s -> units)")]
        [SerializeField]
        private float _turnToPosSway = 0.0008f;

        [Tooltip("Сколько rotational sway давать от поворота (deg/s -> degrees)")] [SerializeField]
        private float _turnToRotSway = 0.02f;

        private Vector3 _initialPos;
        private Quaternion _initialRot;

        private Vector3 _prevRefPos;
        private Quaternion _prevRefRot;

        void Start()
        {
            if (_body == null)
            {
                enabled = false;
                return;
            }

            _initialPos = _body.localPosition;
            _initialRot = _body.localRotation;

            if (_reference == null)
                _reference = transform.root;

            _prevRefPos = _reference.position;
            _prevRefRot = _reference.rotation;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f || _reference == null) return;

            // --- velocity (movement) ---
            Vector3 worldVel = (_reference.position - _prevRefPos) / dt;
            Vector3 localVel = _reference.InverseTransformDirection(worldVel);

            // --- angular velocity (turn) ---
            Quaternion delta = _reference.rotation * Quaternion.Inverse(_prevRefRot);
            delta.ToAngleAxis(out float angleDeg, out Vector3 axis);
            if (angleDeg > 180f) angleDeg -= 360f;

            Vector3 worldAngVelDeg = axis * (angleDeg / dt);
            Vector3 localAngVelDeg = _reference.InverseTransformDirection(worldAngVelDeg);

            // save prev
            _prevRefPos = _reference.position;
            _prevRefRot = _reference.rotation;

            // В терминах "мыши":
            float yawRate = localAngVelDeg.y; // влево/вправо
            float pitchRate = localAngVelDeg.x; // вверх/вниз

            // ===== POSITION SWAY (переключаем источник) =====
            float moveX, moveY;

            if (_positionFromMovement)
            {
                // как раньше: sway от движения
                // X = стрейф, Y(в твоём finalPos идёт в Z) = вперёд/назад
                moveX = localVel.x * _moveToPosSway * _amount;
                moveY = localVel.z * _moveToPosSway * _amount;
            }
            else
            {
                // sway от поворота
                moveX = yawRate * _turnToPosSway * _amount;
                moveY = pitchRate * _turnToPosSway * _amount;
            }

            moveX = Mathf.Clamp(moveX, -_maxSway, _maxSway);
            moveY = Mathf.Clamp(moveY, -_maxSway, _maxSway);

            Vector3 finalPos = new Vector3(moveX, 0f, moveY);
            _body.localPosition = Vector3.Lerp(_body.localPosition, finalPos + _initialPos, dt * _smoothAmount);

            // ===== ROTATION SWAY (от поворота) =====
            float tiltY = yawRate * _turnToRotSway * _tiltAmount;
            float tiltX = pitchRate * _turnToRotSway * _tiltAmount;

            tiltY = Mathf.Clamp(tiltY, -_maxTiltSway, _maxTiltSway);
            tiltX = Mathf.Clamp(tiltX, -_maxTiltSway, _maxTiltSway);

            float tiltZ = tiltY;

            Quaternion finalRot = Quaternion.Euler(new Vector3(
                _tiltDirX ? -tiltX : 0f,
                _tiltDirY ? tiltY : 0f,
                _tiltDirZ ? -tiltZ : 0f
            ));

            _body.localRotation = Quaternion.Lerp(_body.localRotation, finalRot * _initialRot, dt * _smoothAmountTilt);
        }

        // Если хочешь переключать из кода
        public void SetPositionFromMovement(bool value) => _positionFromMovement = value;
    }
}