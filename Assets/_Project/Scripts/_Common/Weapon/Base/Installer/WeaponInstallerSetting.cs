using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Installer
{
    [HideScriptField]
    
    [CreateAssetMenu(menuName = "WeaponSystem/InstallerSetting",fileName = "InstallerSetting")]
    public class WeaponInstallerSetting : ScriptableObject
    {
        [SerializeField] private int _maxSizePool = 1000;
        [SerializeField] private BaseWeapon _weapon;
        [SerializeReference]public IProviderCollection _providerCollection;

        public int MaxSizePool => _maxSizePool;
        public BaseWeapon BaseWeapon => _weapon;
        public IProviderCollection ProviderCollection => _providerCollection;
    }
}