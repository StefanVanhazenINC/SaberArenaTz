namespace _Project.Scripts.Skills.TargetEffects
{
    public interface ISkillTargetEffect
    {
        void Apply(SkillCastContext context, SkillTarget target);
    }
}