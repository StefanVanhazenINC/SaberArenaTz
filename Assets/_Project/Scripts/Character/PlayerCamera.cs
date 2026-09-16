using System;
using _Project.Scripts.Character.Data;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character
{
  

    public struct CameraInput
    {
        public Vector2 Look;
    }

    public class PlayerCamera : IInitializable, IDisposable
    {
        private PlayerCameraData _cameraData;
        
        private float _autoAimTimeLeft;
        private Vector3 _eulerAngles; // x=pitch, y=yaw
        private Vector3 _lookInputWorkSpace;

        // lock state
        private float _anchorYaw;
        private float _anchorPitch;

        // for auto aiming
        private bool _hasAutoYaw;
        private float _autoTargetYaw;
        private bool _hasAutoPitch;
        private float _autoTargetPitch;

        public Transform transform;
        public bool BlockMouseInput { get; set; }


        [Inject]
        public PlayerCamera(PlayerCameraData data,Transform playerCharacterCameraTarget,Transform cameraTransform)
        {
            _cameraData = data;
            transform = cameraTransform;
            transform.position = playerCharacterCameraTarget.position;
            transform.eulerAngles = _eulerAngles = playerCharacterCameraTarget.eulerAngles;
        }
        public void Initialize()
        {
            _eulerAngles.x = NormalizeAngle(_eulerAngles.x);
            _eulerAngles.y = NormalizeAngle(_eulerAngles.y);
        }
        public void Dispose()
        {
            
        }
        public void UpdateRotation(CameraInput input,float dt)
        {
            if ( BlockMouseInput == true)
            {
                return;
            }
            _lookInputWorkSpace.Set(-input.Look.y, input.Look.x, 0);
            _eulerAngles += _lookInputWorkSpace * _cameraData.sensitivity;
            _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -_cameraData.yClamp, _cameraData.yClamp);
            transform.eulerAngles = _eulerAngles;
        }

        public void UpdatePosition(Transform target)
        {
            transform.position = target.position;
        }

        public void RotateToAngleHorizontal(float angle)
        {
            _eulerAngles.y =angle;
            _eulerAngles.x =0;
            transform.eulerAngles = _eulerAngles;
        }
      

      
      

      

        private float DeltaAngle(float a, float b)
        {
            // a - b in [-180;180]
            float d = NormalizeAngle(a) - NormalizeAngle(b);
            while (d > 180f) d -= 360f;
            while (d < -180f) d += 360f;
            return d;
        }

        private float NormalizeAngle(float a)
        {
            while (a > 180f) a -= 360f;
            while (a < -180f) a += 360f;
            return a;
        }

      

       
    }
}
