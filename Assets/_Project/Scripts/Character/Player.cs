using System;
using System.Collections.Generic;
using _Project.Scripts._Common.ProceduralAnimation;
using _Project.Scripts._Common.Weapon.Base;
using _Project.Scripts._Common.Weapon.Base.Data;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem;
using _Project.Scripts._Common.Weapon.Damageable.HealthSystem.UI;
using _Project.Scripts.Character.Data;
using _Project.Scripts.Weapon.Visual;
using Alchemy.Inspector;
using Common.Weapon.Damageable.Modifiers;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using Alchemy.Editor;
using UnityEditor;
#endif

using Common.BaseComponent;
using Common.Proxy;
using Common.Weapon.Damageable;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace _Project.Scripts.Character
{

    public class Player : MonoBehaviour, IDisposable
    {
        [Header("Component")]
        [SerializeField] private TeamComponent _teamComponent;
        [SerializeField] private DamageableProxy _prox;
        
        [Header("Module")]
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private PlayerSkillController _skillController;
        [Inject] private PlayerWeaponHolder _weaponHolder;
        
        [Header("SpringControllers")]
        [SerializeField] private SpringAnimation _cameraSpring;
        [SerializeField] private CameraSpringAnimation _cameraSpringController;
       
        [SerializeField] private UnityEvent _onDamageTaken = new UnityEvent();


        [Inject] private PlayerDeathHandler _deathHandler;
        [Inject] private PlayerCamera _playerCamera;
        [Inject] private CameraLean _cameraLean;
        private PlayerInputReader _inputReader;
        private bool _isDisposed;
        
        private HealthData _healthData;
        private HealthData _armorData;
        
        private HealthSystem _healthSystem;
        private HealthSystem _armorSystem;
        private ArmorDamageModifier _armorDamageModifier;
        
        public TeamComponent Team=> _teamComponent;
        public HealthSystem HealthSystem => _healthSystem;
        public HealthData HealthData => _healthData;
        public HealthSystem ArmorSystem => _armorSystem;
        public HealthData ArmorData => _armorData;
        public event Action OnAfterInitialize = delegate { };
        public UnityEvent OnDamageTaken => _onDamageTaken;
        
        public PlayerWeaponHolder WeaponHolder => _weaponHolder;
        public PlayerCharacter PlayerCharacter => _playerCharacter;
        [Inject]
        public void Initialize(CharacterData data)
        {
            _healthData =  data.health;
            _armorData =  data.armor;
        }



        [Button]
        public void Death()
        {
            _healthData.Death();
        }

        [Button]
        public void ResetHealth()
        {
            _healthData.ResetHealth();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            //Health
            {
                _healthSystem = new HealthSystem(_healthData);
                _prox.Constuct(_teamComponent,_healthSystem);
                _healthSystem.OnTakeDamage += ()=> _onDamageTaken.Invoke();
            }
            
            //Armor
            {
                _armorSystem = new HealthSystem(_armorData);
                _armorDamageModifier = new ArmorDamageModifier(_armorSystem);
                _healthSystem.AddModifier(_armorDamageModifier);
            }
            _healthData.OnDeath += _deathHandler.HandleDeath;

            
            #region Initialization
            _inputReader = new PlayerInputReader();
            _inputReader.Enable();

            _playerCharacter.Initialize();
            _playerCharacter.SetCamera(_playerCamera);
            _cameraSpring.Initialize();

           
            _weaponHolder.Initialize();
            OnAfterInitialize?.Invoke();
            #endregion

        } 
        private void ProcessInput()
        {
            float deltaTime = Time.deltaTime;
            PlayerFrameInput input = _inputReader.Read(_playerCamera.transform.rotation);

            _playerCamera.UpdateRotation(input.Camera, deltaTime * Time.timeScale);
            _weaponHolder.UpdateInput(input.Weapon);
            _playerCharacter.UpdateInput(input.Character);
            _playerCharacter.UpdateBody(deltaTime);

            if (input.UseFirstSkill)
                _skillController.TryUseSkill(0);

            if (input.UseSecondSkill)
                _skillController.TryUseSkill(1);
        }
        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            var cameraTarget = _playerCharacter.CameraTarget;
            var state = _playerCharacter.CurrentState;
            
            
            _playerCamera.UpdatePosition(cameraTarget);
            _cameraSpringController.BeforeUpdate();
            
            _cameraSpring.UpdateSpring(deltaTime);

            _cameraSpringController.UpdateSpring(deltaTime,cameraTarget.up);
            
            _cameraLean.UpdateLean(deltaTime, state.Velocity, cameraTarget.up);
            
        }
        private void Update()
        {
            if (_healthSystem.IsDeath)
            {
                return;
            }

            ProcessInput();
        }
        private void OnDestroy()
        {
            Dispose();
        }

        [Button]
        public void AddGun(WeaponConfig newWeapon)
        {
            _weaponHolder.InstallNewWeapon(newWeapon);
        }
    
        public void SpawnAt(Transform point)
        {
            _playerCamera.BlockMouseInput = true;
            _playerCamera.RotateToAngleHorizontal(point.eulerAngles.y);
            Teleport(point.position);
        }
        public void Teleport(Vector3 position)
        {
            _playerCharacter.SetPosition(position);  
            _playerCamera.BlockMouseInput = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (_healthData != null && _deathHandler != null)
                _healthData.OnDeath -= _deathHandler.HandleDeath;

            _inputReader?.Dispose();
            _healthSystem?.Dispose();
            _armorSystem?.Dispose();
        }
    }

}
