using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _Follow;
    [SerializeField] private float _speedToLook;
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform[] camerasPos;

    //[SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _smoothSpeed;
    private Vector3 smoothVelocity = Vector3.zero;

    private Vector3[] _offSets;

    private Vector3 currOffset = Vector3.zero;
    private void Start()
    {
        _offSets = new Vector3[camerasPos.Length]; 
        
        for (int i = 0; i < camerasPos.Length; i++)
        {
            _offSets[i] = _Follow.position - camerasPos[i].position;
        }
        currOffset = _offSets[0];
        //Debug.Log(currOffset);
    }
    private void Update()
    {
        Vector3 cameraGoal = _Follow.position - currOffset;

        Vector3 lookPos = _Follow.position - mainCamera.position;

        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, targetRotation, _speedToLook * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, cameraGoal, ref smoothVelocity, _smoothSpeed);

        lookPos.y = 0;
        targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speedToLook * Time.deltaTime);




        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currOffset = _offSets[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currOffset = _offSets[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currOffset = _offSets[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currOffset = _offSets[3];
        }
    }

}
