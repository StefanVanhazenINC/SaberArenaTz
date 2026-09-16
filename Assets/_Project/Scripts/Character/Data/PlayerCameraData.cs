using System;
using UnityEngine;

namespace _Project.Scripts.Character.Data
{
    [Serializable]
    public class PlayerCameraData
    {
        [Header("Base")] 
        public float sensitivity = 0.1f;
        public float yClamp = 89f;
        
        [Header("Wall Camera Turn")] 
        public  float autoTurnSpeed = 14f;
        
        [Header("Wall Along Limits")] 
        public  float alongYawLimit = 60f;
        
        
        public float alongPitchUpLimit = 35f;
        public  float alongPitchDownLimit = 35f;

        [Header("Wall Up Limits")] 
        public  float upYawLimit = 25f;

        public float upPitchUpLimit = 10f;
        public float upPitchDownLimit = 55f;

        [Header("Auto Rotate Away")] 
        public  float autoAwaySpeed = 12f;
        public  float autoAimTime = 0.12f; // только стартовая доводка на Along/Up

        public float autoAwayTargetPitch = 10f; // куда вернуть pitch при развороте от стены

        [Header("Auto Face Up")]
        public  float faceWallUpPitch = 70f; // при старте wall-up
    }
}