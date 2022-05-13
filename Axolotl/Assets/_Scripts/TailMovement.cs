using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script makes segments follow a position along the x axis. 
/// </summary>
public class TailMovement : MonoBehaviour
{
    [SerializeField] Transform[] bodySegments;
    [SerializeField] Transform targetTransform;
    public float segmentDistance;
    public float smoothSpeed; 
    private Vector3[] _segmentsPos;
    private Vector3[] _segmentsVel;
    void Start()
    {
        _segmentsPos = new Vector3[bodySegments.Length];
        _segmentsVel = new Vector3[bodySegments.Length];
    }

    void Update()
    {

        _segmentsPos[0] = targetTransform.position;
        for (int i = 1; i < _segmentsPos.Length; i++)
        {
            _segmentsPos[i] = Vector3.SmoothDamp(_segmentsPos[i], _segmentsPos[i - 1] + targetTransform.right * segmentDistance, ref _segmentsVel[i], smoothSpeed);
        }

        for (int i = 0; i < bodySegments.Length; i++)
        {
            bodySegments[i].position = _segmentsPos[i];
        }
    }
}
