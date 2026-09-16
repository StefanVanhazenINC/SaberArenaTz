using UnityEngine;

namespace _Project.Scripts.Skills.CastEffects
{
    public interface SkillCastEffect 
    {
        public void Execute(SkillCastContext context);
    }
}