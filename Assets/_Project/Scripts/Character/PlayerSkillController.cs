using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Skills;
using UnityEngine;

namespace _Project.Scripts.Character
{
    public class PlayerSkillController: MonoBehaviour
    {
        
        [SerializeField] private Transform _castPoint;
        [SerializeField] private List<SkillSlot> _slots = new List<SkillSlot>();

        private float[] _cooldowns;
        private SkillRuntimeData _runtime;

        private void Awake()
        {
            if (_castPoint == null)
                _castPoint = transform;

            _cooldowns = new float[_slots.Count];
            _runtime = new SkillRuntimeData();
        }

        private void Update()
        {
            for (int i = 0; i < _cooldowns.Length; i++)
            {
                if (_cooldowns[i] > 0f)
                {
                    _cooldowns[i] -= Time.deltaTime;
                    if (_cooldowns[i] < 0f)
                        _cooldowns[i] = 0f;
                }
            }
        }

        public bool TryUseSkill(int index)
        {
            if (index < 0 || index >= _slots.Count)
                return false;

            SkillDefinitionSO skill = _slots[index].Skill;
            if (skill == null)
                return false;

            if (_cooldowns[index] > 0f)
                return false;

            HashSet<Transform> appliedTargets = skill.ApplyOncePerTarget ? new HashSet<Transform>() : null;
            SkillCastContext context = new SkillCastContext(transform, _castPoint, _runtime, appliedTargets);
            skill.Cast(context);
            _slots[index].OnCast?.Invoke();

            if (skill.TargetingDuration > 0f)
                StartCoroutine(ApplyTargetEffectsOverTime(skill, appliedTargets));

            _cooldowns[index] = skill.Cooldown;
            return true;
        }

        public bool TryUseSkill(string id)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].Id == id)
                    return TryUseSkill(i);
            }

            return false;
        }

        public float GetCooldownNormalized(int index)
        {
            if (index < 0 || index >= _slots.Count)
                return 0f;

            SkillDefinitionSO skill = _slots[index].Skill;
            if (skill == null || skill.Cooldown <= 0f)
                return 0f;

            return Mathf.Clamp01(_cooldowns[index] / skill.Cooldown);
        }

        public bool IsReady(int index)
        {
            if (index < 0 || index >= _slots.Count)
                return false;

            return _cooldowns[index] <= 0f;
        }

        private IEnumerator ApplyTargetEffectsOverTime(SkillDefinitionSO skill, HashSet<Transform> appliedTargets)
        {
            float elapsed = 0f;
            float interval = Mathf.Max(0.01f, skill.TargetingInterval);

            while (elapsed < skill.TargetingDuration)
            {
                yield return new WaitForSeconds(interval);
                elapsed += interval;

                SkillCastContext context = new SkillCastContext(transform, _castPoint, _runtime, appliedTargets);
                skill.ApplyTargetEffects(context);
            }
        }
    }
}
