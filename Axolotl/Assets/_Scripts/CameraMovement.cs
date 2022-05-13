using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _Follow;
    [SerializeField] private float _speedToLook;
    //[SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _smoothSpeed;
    private Vector3 smoothVelocity = Vector3.zero;

    private Vector3 _offSet = Vector3.zero;

    private void Start()
    {

        _offSet = _Follow.position + transform.position;
    }
    private void Update()
    {
        Vector3 cameraGoal = _offSet + _Follow.position;
        cameraGoal = new Vector3(cameraGoal.x, _offSet.y, cameraGoal.z);
        Quaternion targetRotation = Quaternion.LookRotation(_Follow.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speedToLook * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, cameraGoal, ref smoothVelocity, _smoothSpeed);
    }

}
