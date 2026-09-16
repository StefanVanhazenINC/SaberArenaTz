using System.Collections.Generic;
using _Project.Scripts._Common.Weapon.Base.Data;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon
{
    [CreateAssetMenu(fileName = "WeaponCatalog", menuName = "WeaponSystem/WeaponCatalog")]
    public class WeaponCatalog : ScriptableObject
    {
        [SerializeField] private List<WeaponConfig> _weapons = new();
        private Dictionary<string, WeaponConfig> _cache;
        
        public WeaponConfig GetById(string id)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<string, WeaponConfig>();
                foreach (var weapon in _weapons)
                {
                    if (weapon == null || string.IsNullOrWhiteSpace(weapon.WeaponData.Id))
                        continue;

                    _cache[weapon.WeaponData.Id] = weapon;
                }
            }

            if (string.IsNullOrWhiteSpace(id))
                return null;

            _cache.TryGetValue(id, out var result);
            return result;
        }
    }
}