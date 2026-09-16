using System;
using _Project.Scripts._Common.Weapon.Base;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Character.Data
{
    
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Character/CharacterConfig", order = 0)]
    [HideScriptField]
    public class CharacterConfig : ScriptableObject
    {
        [SerializeField] private CharacterData _characterData;
        
        [Header("Camera")]
        [SerializeField] private PlayerCameraData _cameraData;
        [SerializeField] private CameraLeanData _leanData;

        [Header("Weapon")]
        [SerializeField] private PlayerWeaponHolderData _weaponData;
        
        public CharacterData CharacterData => _characterData;

        public PlayerCameraData CameraData => _cameraData;

        public CameraLeanData LeanData => _leanData;


        public PlayerWeaponHolderData WeaponData => _weaponData;



        
    }
}