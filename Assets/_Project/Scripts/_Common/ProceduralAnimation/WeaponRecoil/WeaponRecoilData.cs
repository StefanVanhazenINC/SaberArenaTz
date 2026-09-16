using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponRecoilData 
{
    [Header("Speed Setting")]
    [SerializeField] public float PositionalRecoilSpeed = 8f;
    [SerializeField] public float _rotationalRecoilSpeed = 8f;

    [SerializeField] public float _positionalReturnSpeed = 18f;
    [SerializeField] public float _rotationalReturnSpeed = 38f;

    [Header("Amount Settings")]
    [SerializeField] public Vector3 _recoilRotation = new Vector3(10f, 5f, 7f);

    [SerializeField] public Vector3 _recoilKickBack = new Vector3(0.15f, 0f, -0.2f);


    [Header("Clamp Settings")]
    [SerializeField] public Vector3 _rotaionClamp;
    [SerializeField] public Vector3 _positionClamp;
}
