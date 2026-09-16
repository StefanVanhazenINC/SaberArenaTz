using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Skills.Selectors
{
    [Serializable]
    public class ConeTargetSelector : ISkillTargetSelector
    {
        [SerializeField] private float _range = 7f;
        [SerializeField, Range(1f, 180f)] private float _angle = 70f;
        [SerializeField] private float _backOffsset = 0.5f;
        [SerializeField] private float _upOffsset = 1.5f;
        [SerializeField] private LayerMask _targetMask;

        public  void CollectTargets(SkillCastContext context, List<SkillTarget> results)
        {
            context.Runtime.UniqueRoots.Clear();

            int count = Physics.OverlapSphereNonAlloc(
                context.Position- (context.Forward * _backOffsset),
                _range,
                context.Runtime.ColliderBuffer,
                _targetMask,
                QueryTriggerInteraction.Ignore);

            float halfAngle = _angle * 0.5f;
            Debug.DrawLine(context.Position- (context.Forward * _backOffsset) + (Vector3.up * _upOffsset),(context.Position- (context.Forward * _backOffsset)+ (Vector3.up * _upOffsset))+ context.Origin.forward *_range, Color.green, 1);
           
            for (int i = 0; i < count; i++)
            {
            
                Collider hit = context.Runtime.ColliderBuffer[i];
                if (hit == null)
                    continue;

                Transform root = hit.transform.root;
                if (!context.Runtime.UniqueRoots.Add(root))
                    continue;

                Vector3 toTarget = hit.bounds.center - (context.Position - (context.Forward * _backOffsset)+ (Vector3.up * _upOffsset));
                float angleToTarget = Vector3.Angle(context.Forward, toTarget);

                if (angleToTarget > halfAngle)
                {
                 
                    continue;
                }

                results.Add(new SkillTarget(hit));
            }
        }
    }
}
