using _Project.Scripts._Common.Weapon.Base;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character
{
    public class CharacterComponentProvider : MonoBehaviour
    {
        [Inject] private  PlayerWeaponHolder _weaponHolder;
        [SerializeField] private Player _player;

        public PlayerWeaponHolder WeaponHolder => _weaponHolder;
        public Player Player => _player;
    }
}