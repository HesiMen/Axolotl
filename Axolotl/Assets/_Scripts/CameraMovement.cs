using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _Follow;
    [SerializeField] private float _speedToLook;
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform camera1;
    [SerializeField] Transform camera2;
    [SerializeField] Transform camera3;
    //[SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _smoothSpeed;
    private Vector3 smoothVelocity = Vector3.zero;

    private Vector3 _offSet1 = Vector3.zero;
    private Vector3 _offSet2 = Vector3.zero;
    private Vector3 _offSet3 = Vector3.zero;
    private Vector3 currOffset  = Vector3.zero;
    private void Start()
    {

        _offSet1 = _Follow.position + camera1.position;
        _offSet2 = _Follow.position + camera2.position;
        _offSet3 = _Follow.position + camera3.position;

        currOffset = _offSet1;
    }
    private void Update()
    {
        Vector3 cameraGoal = currOffset + _Follow.position;
        cameraGoal = new Vector3(cameraGoal.x, currOffset.y, cameraGoal.z);
        Vector3 lookPos = _Follow.position - mainCamera.position;

        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, targetRotation, _speedToLook * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, cameraGoal, ref smoothVelocity, _smoothSpeed);
       
        lookPos.y = 0;
        targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speedToLook * Time.deltaTime);




        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currOffset = _offSet1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currOffset = _offSet2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currOffset = _offSet3;
        }
    }

}
