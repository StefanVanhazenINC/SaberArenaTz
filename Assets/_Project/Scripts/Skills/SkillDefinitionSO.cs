using System.Collections.Generic;
using _Project.Scripts.Skills.CastEffects;
using _Project.Scripts.Skills.Selectors;
using _Project.Scripts.Skills.TargetEffects;
using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts.Skills
{
    [CreateAssetMenu(menuName = "Skills/Skill Definition", fileName = "Skill_")]
    [HideScriptField]
    public class SkillDefinitionSO: ScriptableObject
    {
        [SerializeField] private float _cooldown = 5f;
        [SerializeField] private float _targetingDuration = 0f;
        [SerializeField] private float _targetingInterval = 0.25f;
        [SerializeField] private bool _applyOncePerTarget = true;

        [Header("Cast Effects")]
        [SerializeReference] private List<SkillCastEffect> _castEffects = new List<SkillCastEffect>();

        [Header("Targeting")]
        [SerializeReference] private ISkillTargetSelector _targetSelector;

        [Header("Target Effects")]
        [SerializeReference] private List<ISkillTargetEffect> _targetEffects = new List<ISkillTargetEffect>();

        public float Cooldown => _cooldown;
        public float TargetingDuration => _targetingDuration;
        public float TargetingInterval => _targetingInterval;
        public bool ApplyOncePerTarget => _applyOncePerTarget;

        public void Cast(SkillCastContext context)
        {
            for (int i = 0; i < _castEffects.Count; i++)
            {
                SkillCastEffect effect = _castEffects[i];
                if (effect != null)
                    effect.Execute(context);
            }

            ApplyTargetEffects(context);
        }

        public void ApplyTargetEffects(SkillCastContext context)
        {
            if (_targetSelector == null || _targetEffects.Count == 0)
                return;

            List<SkillTarget> targets = context.Runtime.TargetBuffer;
            targets.Clear();

            _targetSelector.CollectTargets(context, targets);

            for (int i = 0; i < targets.Count; i++)
            {
                SkillTarget target = targets[i];
                Transform targetRoot = target.Root;

                if (_applyOncePerTarget &&
                    context.AppliedTargets != null &&
                    targetRoot != null &&
                    !context.AppliedTargets.Add(targetRoot))
                {
                    continue;
                }

                for (int j = 0; j < _targetEffects.Count; j++)
                {
                    ISkillTargetEffect effect = _targetEffects[j];
                    if (effect != null)
                        effect.Apply(context, target);
                }
            }
        }
    }
}
