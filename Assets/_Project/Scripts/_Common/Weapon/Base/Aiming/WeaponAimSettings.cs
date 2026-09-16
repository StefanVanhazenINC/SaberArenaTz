using System;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Aiming
{
    [Serializable]
    public sealed class WeaponAimSettings
    {
        [SerializeField] private float _maxDistance = 1000f;
        [SerializeField] private LayerMask _aimMask = Physics.DefaultRaycastLayers;
        [SerializeField] private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;

        public float MaxDistance => _maxDistance;
        public int AimMask => _aimMask;
        public QueryTriggerInteraction TriggerInteraction => _triggerInteraction;
    }
}
