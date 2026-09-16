using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Skills
{
    public sealed class SkillRuntimeData
    {
        public readonly List<SkillTarget> TargetBuffer = new List<SkillTarget>(32);
        public readonly HashSet<Transform> UniqueRoots = new HashSet<Transform>();
        public readonly Collider[] ColliderBuffer = new Collider[64];
    }
}
