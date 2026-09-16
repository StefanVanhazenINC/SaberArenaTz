using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Aiming
{
    public sealed class TransformAimProvider : IAimProvider
    {
        private readonly Transform _aimTransform;
        private readonly float _fallbackDistance;
        private readonly int _aimMask;
        private readonly QueryTriggerInteraction _triggerInteraction;

        public TransformAimProvider(
            Transform aimTransform,
            float fallbackDistance = 1000f,
            int aimMask = Physics.DefaultRaycastLayers,
            QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            _aimTransform = aimTransform;
            _fallbackDistance = fallbackDistance;
            _aimMask = aimMask;
            _triggerInteraction = triggerInteraction;
        }

        public Vector3 GetAimPoint(Transform shotOrigin)
        {
            if (_aimTransform != null)
            {
                Vector3 origin = shotOrigin != null ? shotOrigin.position : _aimTransform.position;
                return origin + _aimTransform.forward * _fallbackDistance;
            }

            if (shotOrigin != null)
                return shotOrigin.position + shotOrigin.forward * _fallbackDistance;

            return Vector3.forward * _fallbackDistance;
        }
    }
}
