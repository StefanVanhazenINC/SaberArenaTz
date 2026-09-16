using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Skills
{
    public struct SkillCastContext
    {
        public Transform Caster;
        public Transform Origin;
        public Vector3 Position;
        public Vector3 Forward;
        public SkillRuntimeData Runtime;
        public HashSet<Transform> AppliedTargets;

        public SkillCastContext(Transform caster, Transform origin, SkillRuntimeData runtime, HashSet<Transform> appliedTargets = null)
        {
            Caster = caster;
            Origin = origin != null ? origin : caster;
            Position = Origin.position;
            Forward = Origin.forward;
            Runtime = runtime;
            AppliedTargets = appliedTargets;
        }
    }
}
