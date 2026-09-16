using System;
using UnityEngine;

namespace _Project.Scripts.Skills.CastEffects
{

    [Serializable]
    public class PlayCasterAudioCastEffect : SkillCastEffect
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] private bool _searchInChildren = true;

        public void Execute(SkillCastContext context)
        {
            if (_clip == null || context.Caster == null)
                return;

            AudioSource source = null;

            if (_searchInChildren)
                source = context.Caster.GetComponentInChildren<AudioSource>();
            else
                source = context.Caster.GetComponent<AudioSource>();

            if (source != null)
            {
                source.PlayOneShot(_clip, _volume);
                return;
            }

            AudioSource.PlayClipAtPoint(_clip, context.Position, _volume);
        }
    }
}