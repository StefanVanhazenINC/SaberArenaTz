using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Character.SpawnSystem
{
    public sealed class PlayerSpawnPoint : MonoBehaviour
    {
        [FormerlySerializedAs("Number")]
        [SerializeField] private int _priority;

        public int Priority => _priority;
        public Transform Transform => transform;
    }
}
