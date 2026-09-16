
using Alchemy.Inspector;

namespace Common.ProcedureAnimation
{
    using System.Collections;
    using System.Collections.Generic;
   
    using UnityEngine;


    [HideScriptField]
    public class WeaponRecoil : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float heightPositionRecoil= 1;
        [SerializeField, Range(0, 1)] private float heightRotationRecoil= 1;


        [Header("Reference Point")]
        [SerializeField] private Transform _recoilPosition;
        [SerializeField] private Transform _rotationPoint;

        [SerializeField]
        private WeaponRecoilData _weaponRecoilData;

        private Vector3 _rotationalRecoil;
        private Vector3 _positionalRecoil;
        private Vector3 _rot;

        private Vector3 _workSpace;

        public Transform RecoilPosition { get => _recoilPosition; set => _recoilPosition = value; }
        public Transform RotationPoint { get => _rotationPoint; set => _rotationPoint = value; }


        public float HeightPositionRecoil { get => heightPositionRecoil; set => heightPositionRecoil = value; }
        public float HeightRotationRecoil { get => heightRotationRecoil; set => heightRotationRecoil = value; }


        public void SetData(WeaponRecoilData weaponRecoilData) 
        {
            _weaponRecoilData = weaponRecoilData;
        }


        public void ChangeHeightPosition(float value = 0)
        {
            heightPositionRecoil = Mathf.Clamp01(value);
            //просто в 0 убирать   
        }
        public void ChangeHeightRotation(float value = 0)
        {
            heightRotationRecoil = Mathf.Clamp01(value);
            //просто в 0 убирать   
        }

        public void ReturnToDefaultHeightPosition()
        {
            heightPositionRecoil = 1;
        }
        public void ReturnToDefaultHeightRotation()
        {
            heightRotationRecoil = 1;
        }
        [Button]
        public void RecoilUpdate()
        {
            _workSpace.Set(-_weaponRecoilData._recoilRotation.x, Random.Range(-_weaponRecoilData._recoilRotation.y, _weaponRecoilData._recoilRotation.y), Random.Range(-_weaponRecoilData._recoilRotation.z, _weaponRecoilData._recoilRotation.z));
            _rotationalRecoil = _workSpace;


            _workSpace.Set(Random.Range(-_weaponRecoilData._recoilKickBack.x, _weaponRecoilData._recoilKickBack.x), Random.Range(-_weaponRecoilData._recoilKickBack.y, _weaponRecoilData._recoilKickBack.y), -_weaponRecoilData._recoilKickBack.z);
            _positionalRecoil = _workSpace;




        }
        private  void RecoilProcess() 
        {
            _rotationalRecoil = Vector3.Lerp(_rotationalRecoil, Vector3.zero, _weaponRecoilData._rotationalReturnSpeed * Time.deltaTime);
            _positionalRecoil = Vector3.Lerp(_positionalRecoil, Vector3.zero, _weaponRecoilData._positionalReturnSpeed * Time.deltaTime);

            //
            _positionalRecoil.Set(Mathf.Clamp(_positionalRecoil.x, -_weaponRecoilData._positionClamp.x, _weaponRecoilData._positionClamp.x), Mathf.Clamp(_positionalRecoil.y, -_weaponRecoilData._positionClamp.y, _weaponRecoilData._positionClamp.y), Mathf.Clamp(_positionalRecoil.z, -_weaponRecoilData._positionClamp.z, _weaponRecoilData._positionClamp.z));
            _rotationalRecoil.Set(Mathf.Clamp(_rotationalRecoil.x, -_weaponRecoilData._rotaionClamp.x, _weaponRecoilData._rotaionClamp.x), Mathf.Clamp(_rotationalRecoil.y, -_weaponRecoilData._rotaionClamp.y, _weaponRecoilData._rotaionClamp.y), Mathf.Clamp(_rotationalRecoil.z, -_weaponRecoilData._rotaionClamp.z, _weaponRecoilData._rotaionClamp.z));
            //

            _recoilPosition.localPosition = Vector3.Slerp(_recoilPosition.localPosition, _positionalRecoil, (_weaponRecoilData.PositionalRecoilSpeed * Time.fixedDeltaTime) * heightPositionRecoil);
            //_rot = Vector3.Slerp(_rot, _rotationalRecoil, _rotationalRecoilSpeed * Time.fixedDeltaTime);
           
            _rot = Vector3.Lerp(_rot, _rotationalRecoil, (_weaponRecoilData._rotationalRecoilSpeed * Time.fixedDeltaTime) * heightRotationRecoil);
            _rotationPoint.localRotation = Quaternion.Euler(_rot);

        }

        private void FixedUpdate()
        {
            RecoilProcess();
        }

    }
}