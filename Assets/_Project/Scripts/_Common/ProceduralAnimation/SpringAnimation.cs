using System;
using UnityEngine;

namespace _Project.Scripts._Common.ProceduralAnimation
{
    public class SpringAnimation : MonoBehaviour
    {
        [Tooltip("Чем меньше время, тем быстрее будет затухание")]
        [SerializeField] private float _halfLife = 0.075f;
        [Tooltip("Чем выше значение , тем меньше будет отставание от цели. Но не слишком маленькой,чтоб избежать вибрации")]
        [SerializeField] private float _frequency  = 18f;

        [SerializeField] private bool _drawGizmos = false;
        private Vector3 _springPosition;
        private Vector3 _springVelocity;
        
        public Vector3 SpringPosition => _springPosition;
        public Vector3 SpringVelocity => _springVelocity;
        
        public void Initialize()
        {
            _springPosition = transform.position;
            _springVelocity = Vector3.zero;
        }
    
        public void UpdateSpring(float deltaTime)
        {
            Spring(ref _springPosition, ref _springVelocity, transform.position,_halfLife,_frequency,deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos)
            {
                Gizmos.color = Color.green;
                //Gizmos.DrawLine(transform.position,transform.position + _springPosition);
                Gizmos.DrawLine(transform.position, _springPosition);
               // Gizmos.DrawSphere(transform.position + _springPosition,0.1f);  
                Gizmos.DrawSphere(_springPosition,0.1f);  
            }
        }

        public void Spring(ref Vector3 current, ref Vector3  velocity, Vector3 target, float halfLife, float frequency, float timeStep)
        {
            var dampingRatio = -Mathf.Log(0.5f) / (_frequency * _halfLife);
            float f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
            float oo = frequency * frequency;
            float hoo = timeStep * oo;
            float hhoo = timeStep * hoo;
            float detInv = 1.0f / (f + hhoo);
            var detX = f * current + timeStep * velocity + hhoo * target;
            var detV = velocity + hoo * (target - current);
            current = detX * detInv;
            velocity = detV * detInv;
        }
    }
}