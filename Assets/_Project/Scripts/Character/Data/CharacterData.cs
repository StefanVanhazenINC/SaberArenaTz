using System;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;
using UnityEngine;

namespace _Project.Scripts.Character.Data
{
    [Serializable]
    public class CharacterData
    {
        [Header("Health System")]
        public HealthData health = new HealthData(100,"PlayerHealth");
        public HealthData armor = new HealthData(25,"PlayerArmor");
        public CharacterData Clone()
        {
            CharacterData newData = new CharacterData();
            newData.health = health.Clone() ;
            newData.armor  = armor .Clone() ;
            newData.health.ResetHealth();
            newData.armor.ResetHealth();
            return newData;
        }
      
        
    }
}
