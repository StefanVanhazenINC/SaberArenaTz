using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Skills.Selectors
{
    public interface  ISkillTargetSelector 
    {
        public void CollectTargets(SkillCastContext context, List<SkillTarget> results);
    }
}