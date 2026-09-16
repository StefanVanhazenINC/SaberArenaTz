using System;
using Alchemy.Inspector;
using KinematicCharacterController;
using UnityEngine;

namespace _Project.Scripts.Character
{
    public enum Stance
    {
        Stand
    }

    public struct CharacterState
    {
        public bool Grounded;
        public Stance Stance;
        public Vector3 Velocity;
        public Vector3 Acceleration;
    }

    public struct CharacterInput
    {
        public Quaternion Rotation;
        public Vector2 Move;
        public bool Jump;
        public bool JumpSustain;
    }

    [HideScriptField]
    public class PlayerCharacter : MonoBehaviour, ICharacterController
    {
        [FoldoutGroup("References")] [SerializeField]
        private KinematicCharacterMotor _motor;

        [FoldoutGroup("References")] [SerializeField]
        private Transform _cameraTarget;

        [FoldoutGroup("References")] [SerializeField]
        private bool _jumpSustainEnable = true;

        [Header("Settings")] [TabGroup("AllTabs", "Speed")] [SerializeField]
        private float _walkSpeed = 20f;

        [TabGroup("AllTabs", "Speed")] [SerializeField]
        private float _walkResponse = 25f;

        [TabGroup("AllTabs", "Jump")] [SerializeField]
        private float _jumpSpeed = 20f;

        [TabGroup("AllTabs", "Jump")] [SerializeField]
        private float _gravity = -90f;

        [TabGroup("AllTabs", "Jump")] [SerializeField]
        private float _coyoteTime = 0.2f;

        [TabGroup("AllTabs", "Jump")] [SerializeField, Range(0, 1f), ShowIf("_jumpSustainEnable")]
        private float _jumpSustainGravity = 0.4f;

        [Space] [TabGroup("AllTabs", "Air Control")] [SerializeField]
        private float _airSpeed = 15f;

        [TabGroup("AllTabs", "Air Control")] [SerializeField]
        private float _airAcceleration = 70f;

        [TabGroup("AllTabs", "Camera")] [SerializeField, Range(0, 1f)]
        private float _cameraTargetHeight = 0.9f;

        [TabGroup("AllTabs", "Camera")] [SerializeField]
        private float _cameraHeightResponse = 15f;

        private CharacterState _state;
        private CharacterState _lastState;
        private CharacterState _tempState;

        private Quaternion _requestedRotation;
        private Vector3 _requestedMovement;

        private bool _requestedJump;
        private bool _requestedSustainJump;
        private bool _ungroundedDueToJump;
        private float _timeSinceUngrounded;
        private float _timeSinceJumpRequest;
        private Vector3 _externalImpulse;

        public Transform CameraTarget => _cameraTarget;
        public CharacterState CurrentState => _state;
        public CharacterState LastState => _lastState;

        public void Initialize()
        {
            _state.Stance = Stance.Stand;
            _lastState = _state;
            _motor.CharacterController = this;
        }

        public void SetCamera(PlayerCamera camera)
        {
        }

        public void UpdateInput(CharacterInput input)
        {
            _requestedRotation = input.Rotation;
            _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
            _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
            _requestedMovement = input.Rotation * _requestedMovement;

            var wasRequestingJump = _requestedJump;
            _requestedJump = _requestedJump || input.Jump;
            if (_requestedJump && !wasRequestingJump)
                _timeSinceJumpRequest = 0f;

            _requestedSustainJump = input.JumpSustain && _jumpSustainEnable;
        }

        public void UpdateBody(float deltaTime)
        {
            if (_cameraTarget == null || _motor == null)
                return;

            var targetHeight = _motor.Capsule.height * _cameraTargetHeight;
            _cameraTarget.localPosition = Vector3.Lerp(
                _cameraTarget.localPosition,
                new Vector3(0f, targetHeight, 0f),
                1f - Mathf.Exp(-_cameraHeightResponse * deltaTime)
            );
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, _motor.CharacterUp);
            if (forward != Vector3.zero)
                currentRotation = Quaternion.LookRotation(forward, _motor.CharacterUp);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            _state.Acceleration = Vector3.zero;

            if (_motor.GroundingStatus.IsStableOnGround)
            {
                _ungroundedDueToJump = false;
                _timeSinceUngrounded = 0f;
                Move(ref currentVelocity, deltaTime);
            }
            else
            {
                _timeSinceUngrounded += deltaTime;
                AirControl(ref currentVelocity, deltaTime);
                UseGravity(ref currentVelocity, deltaTime);
            }

            if (_requestedJump)
                Jump(ref currentVelocity, deltaTime);

            ApplyExternalImpulse(ref currentVelocity);
        }

        public void AddExternalImpulse(Vector3 impulse, bool forceUnground = true)
        {
            _externalImpulse += impulse;

            if (forceUnground)
                _motor.ForceUnground(0.08f);
        }

        private void ApplyExternalImpulse(ref Vector3 currentVelocity)
        {
            if (_externalImpulse.sqrMagnitude <= 0.0001f)
                return;

            currentVelocity += _externalImpulse;
            _externalImpulse = Vector3.zero;
        }

        private void Move(ref Vector3 currentVelocity, float deltaTime)
        {
            var groundedMovement = _motor.GetDirectionTangentToSurface(
                _requestedMovement,
                _motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            var targetVelocity = groundedMovement * _walkSpeed;
            var moveVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                1f - Mathf.Exp(-_walkResponse * deltaTime)
            );

            _state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;
            currentVelocity = moveVelocity;
        }

        private void AirControl(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_requestedMovement.sqrMagnitude <= 0f)
                return;

            var planarMovement = Vector3.ProjectOnPlane(_requestedMovement, _motor.CharacterUp).normalized *
                                 _requestedMovement.magnitude;

            var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp);
            var movementForce = planarMovement * _airAcceleration * deltaTime;

            if (currentPlanarVelocity.magnitude < _airSpeed)
            {
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, _airSpeed);
                movementForce = targetPlanarVelocity - currentPlanarVelocity;
            }
            else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
            {
                movementForce = Vector3.ProjectOnPlane(movementForce, currentPlanarVelocity.normalized);
            }

            if (_motor.GroundingStatus.FoundAnyGround &&
                Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
            {
                var obstructionNormal = Vector3.Cross(
                    _motor.CharacterUp,
                    Vector3.Cross(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal)
                ).normalized;

                movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
            }

            currentVelocity += movementForce;
        }

        private void Jump(ref Vector3 currentVelocity, float deltaTime)
        {
            var grounded = _motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < _coyoteTime && !_ungroundedDueToJump;

            if (grounded || canCoyoteJump)
            {
                _requestedJump = false;
                _motor.ForceUnground(0.1f);
                _ungroundedDueToJump = true;

                var currentVerticalSpeed = Vector3.Dot(currentVelocity, _motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, _jumpSpeed);
                currentVelocity += _motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                _requestedJump = _timeSinceJumpRequest < _coyoteTime;
            }
        }

        private void UseGravity(ref Vector3 currentVelocity, float deltaTime)
        {
            var effectiveGravity = _gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, _motor.CharacterUp);

            if (_requestedSustainJump && verticalSpeed > 0f)
                effectiveGravity *= _jumpSustainGravity;

            currentVelocity += _motor.CharacterUp * effectiveGravity * deltaTime;
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            _tempState = _state;
            _state.Stance = Stance.Stand;
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            _state.Grounded = _motor.GroundingStatus.IsStableOnGround;
            _state.Velocity = _motor.Velocity;

            if (deltaTime > 0f)
            {
                var totalAcceleration = (_state.Velocity - _lastState.Velocity) / deltaTime;
                _state.Acceleration = Vector3.ClampMagnitude(_state.Acceleration, totalAcceleration.magnitude);
            }

            _lastState = _tempState;
        }

        public bool IsColliderValidForCollisions(Collider coll) => true;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            _state.Acceleration = Vector3.ProjectOnPlane(_state.Acceleration, hitNormal);
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void SetPosition(Vector3 position, bool killVelocity = true)
        {
            _motor.SetPosition(position);
            if (killVelocity)
                _motor.BaseVelocity = Vector3.zero;
        }
    }
}
