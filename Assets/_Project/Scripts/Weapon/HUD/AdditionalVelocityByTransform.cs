using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdditionalVelocityByTransform : MonoBehaviour
{

    private Rigidbody _rb;
    [SerializeField] private Transform _trackingObject;
    [SerializeField] private bool _trackEulers;
    [SerializeField] private bool _setCamera;
    [SerializeField] private float _postionEffect = 10f;
    [SerializeField] private float _rotationEffect = 1f;

    private Vector3 _prevPos;
    private Vector3 _prevEuler;
    private Vector3 _additionalForce;
    private Vector3 _additionalTorque;



    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_setCamera)
        {
            _trackingObject = Camera.main.transform;  
        } 
    }

    void Update()
    {
        
        _additionalForce = (_prevPos - _trackingObject.position) * _postionEffect;

        _rb.AddForce(_additionalForce);

        if(_trackEulers)
        {
            Vector3 force = new Vector3(0, 0, Mathf.DeltaAngle(_prevEuler.y, _trackingObject.eulerAngles.y)) * _rotationEffect;   
            
            _additionalTorque = force;
            //_additionalTorque = _trackingObject.TransformVector(force);

            _rb.AddRelativeTorque(_additionalTorque, ForceMode.Acceleration);
        }

        _prevPos = _trackingObject.position;
        _prevEuler = _trackingObject.eulerAngles;
    }

    
    [ContextMenu("Reset Position")]
    private void ResetPosition()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.position = Vector3.zero;
        _rb.rotation = Quaternion.identity;
    }
}
