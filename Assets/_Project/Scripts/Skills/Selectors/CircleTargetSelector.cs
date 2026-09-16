using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Skills.Selectors
{
    [Serializable]
    public class CircleTargetSelector : ISkillTargetSelector
    {
        [SerializeField] private float _radius = 5f;
        [SerializeField] private LayerMask _targetMask;

        public void CollectTargets(SkillCastContext context, List<SkillTarget> results)
        {
            context.Runtime.UniqueRoots.Clear();

            int count = Physics.OverlapSphereNonAlloc(
                context.Position,
                _radius,
                context.Runtime.ColliderBuffer,
                _targetMask,
                QueryTriggerInteraction.Ignore);
            Debug.DrawLine(context.Origin.position,context.Origin.position+ context.Origin.forward * _radius, Color.green, 1);
            for (int i = 0; i < count; i++)
            {
                Collider hit = context.Runtime.ColliderBuffer[i];
                if (hit == null)
                    continue;

                Transform root = hit.transform.root;
                if (!context.Runtime.UniqueRoots.Add(root))
                    continue;

                results.Add(new SkillTarget(hit));
            }
        }

       
    }
}
