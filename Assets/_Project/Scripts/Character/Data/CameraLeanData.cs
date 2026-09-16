using System;
using UnityEngine;

namespace _Project.Scripts.Character.Data
{
    [Serializable]
    public class CameraLeanData
    {
        [Header("Damping")] [Tooltip("Входное ускорение больше чем демпфированое")] 
        public float attackDamping = 0.5f;

        [Tooltip("Входное ускорение меньше чем демпфированое")] 
        public float decayDamping = 0.3f;

        [Header("Strength (degrees per accel unit)")] 
        public float walkSideStrength = 0.1f; // наклон в стороны (roll)

        public  float maxRoll = 8f;
        public  float maxPitch = 6f;
        public  float walkForwardStrength = 0.035f; // наклон вперёд/назад (pitch)

        public float strengthResponse = 5f;

    }
}
