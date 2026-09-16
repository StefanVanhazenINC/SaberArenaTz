using System;
using _Project.Scripts._Common.Weapon.Base;
using UnityEngine;

namespace _Project.Scripts.Character
{
    public readonly struct PlayerFrameInput
    {
        public PlayerFrameInput(
            CameraInput camera,
            CharacterInput character,
            WeaponHolderInput weapon,
            bool useFirstSkill,
            bool useSecondSkill)
        {
            Camera = camera;
            Character = character;
            Weapon = weapon;
            UseFirstSkill = useFirstSkill;
            UseSecondSkill = useSecondSkill;
        }

        public CameraInput Camera { get; }
        public CharacterInput Character { get; }
        public WeaponHolderInput Weapon { get; }
        public bool UseFirstSkill { get; }
        public bool UseSecondSkill { get; }
    }

    public sealed class PlayerInputReader : IDisposable
    {
        private readonly PlayerInputActions _actions = new PlayerInputActions();

        public void Enable()
        {
            _actions.Enable();
        }

        public PlayerFrameInput Read(Quaternion cameraRotation)
        {
            PlayerInputActions.PlayerActions input = _actions.Player;
            bool blockAttacks = input.Hack.IsPressed();

            CameraInput cameraInput = new CameraInput
            {
                Look = input.Look.ReadValue<Vector2>()
            };

            WeaponHolderInput weaponInput = new WeaponHolderInput
            {
                Attack = !blockAttacks && input.Attack.IsPressed(),
                AltAttack = !blockAttacks && input.AltAttack.IsPressed(),
                Reloading = input.Reloading.WasPressedThisFrame(),
                SwitchWeapon = (int)input.ChangeWeapon.ReadValue<float>()
            };

            CharacterInput characterInput = new CharacterInput
            {
                Rotation = cameraRotation,
                Move = input.Move.ReadValue<Vector2>(),
                Jump = input.Jump.WasPressedThisFrame(),
                JumpSustain = input.Jump.IsPressed()
            };

            return new PlayerFrameInput(
                cameraInput,
                characterInput,
                weaponInput,
                input.FirstSkill.WasPerformedThisFrame(),
                input.SecondSkill.WasPerformedThisFrame());
        }

        public void Dispose()
        {
            _actions.Disable();
            _actions.Dispose();
        }
    }
}
