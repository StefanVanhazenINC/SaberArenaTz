using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Skills
{
    [Serializable]
    public class SkillSlot
    {
        [SerializeField] private string _id;
        [SerializeField] private SkillDefinitionSO _skill;
        [SerializeField] private UnityEvent onCast;

        public string Id => _id;
        public SkillDefinitionSO Skill => _skill;

        public UnityEvent OnCast => onCast;
    }
}