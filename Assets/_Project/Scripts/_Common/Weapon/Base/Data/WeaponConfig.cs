using _Project.Scripts._Common.Weapon.Base.Visual;
using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Data
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "WeaponSystem/WeaponConfig", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private BaseWeaponData _weaponData;
        [SerializeField] private BaseWeaponVisualContainer _visualContainer;
        [SerializeField] private GameObject _baseModel;
        public BaseWeaponData WeaponData => _weaponData;
        public BaseWeaponVisualContainer VisualContainer => _visualContainer;
        public GameObject BaseModel => _baseModel;

      
    }
}