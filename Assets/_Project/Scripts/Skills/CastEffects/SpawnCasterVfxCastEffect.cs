using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Skills.CastEffects
{
    [Serializable]
    public class SpawnCasterVfxCastEffect : SkillCastEffect
    {
        [SerializeField] private GameObject _vfxPrefab;
        [SerializeField] private bool _parentToOrigin = true;
        [SerializeField] private Vector3 _localPositionOffset;
        [SerializeField] private Vector3 _localEulerOffset;
        [SerializeField] private float _destroyAfter = 5f;

        public void Execute(SkillCastContext context)
        {
            if (_vfxPrefab == null)
                return;

            Transform anchor = _parentToOrigin && context.Origin != null
                ? context.Origin
                : context.Caster;

            if (anchor == null)
                return;

            GameObject instance;

            if (_parentToOrigin)
            {
                instance = Object.Instantiate(_vfxPrefab, anchor);
                instance.transform.localPosition = _localPositionOffset;
                instance.transform.localRotation = Quaternion.Euler(_localEulerOffset);
            }
            else
            {
                Vector3 worldPos = anchor.TransformPoint(_localPositionOffset);
                Quaternion worldRot = anchor.rotation * Quaternion.Euler(_localEulerOffset);
                instance = Object.Instantiate(_vfxPrefab, worldPos, worldRot);
            }

            if (_destroyAfter > 0f)
                Object.Destroy(instance, _destroyAfter);
        }
    }
}