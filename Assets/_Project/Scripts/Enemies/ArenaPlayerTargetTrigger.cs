using _Project.Scripts.Character;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Enemies
{
    public sealed class ArenaPlayerTargetTrigger : MonoBehaviour
    {
        [SerializeField] private ArenaController _arenas;
        [SerializeField] private UnityEvent<Transform> _onTargetFound = new UnityEvent<Transform>();

        private bool _wasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if ( _wasTriggered)
                return;

            Transform target = GetPlayerTarget(other);
            if (target == null)
                return;

            _wasTriggered = true;
            SendTarget(target);
        }

        public void SendTarget(Transform target)
        {
            if (target == null)
                return;

            _arenas.BeginArena(target);
            _onTargetFound.Invoke(target);
        }

        private Transform GetPlayerTarget(Collider other)
        {
            CharacterComponentProvider player = other.GetComponent<CharacterComponentProvider>();
            if (player != null)
                return player.transform;

            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Collider trigger = GetComponent<Collider>();
            if (trigger != null)
                trigger.isTrigger = true;
        }
#endif
    }
}
