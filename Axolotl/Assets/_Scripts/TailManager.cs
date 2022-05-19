using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    [SerializeField] private CharacterController _charController;
    [SerializeField] private Material _originalMat;
    [SerializeField] private Material collectMaterial;
    [SerializeField] private TailMovement tailMovement;
    [SerializeField] private MeshRenderer[] _MatToChange;

    public int counter = -1;

    public bool isTailOff = false;
    public bool isSlow = false;
    private void Start()
    {
        counter = -1;
        _charController.OnJump.AddListener(HasJump);
        foreach (var mat in _MatToChange)
        {
            mat.material = _originalMat;
        }
    }

    private void HasJump()
    {
        NoJumps();
    }

    private void NoJumps()
    {
        counter = -1;
        foreach (var tailSeg in _MatToChange)
        {
            tailSeg.material = _originalMat;
        }
    }

    private void CollectedFood()
    {

        counter += 1;
        if (counter < 3)
        {
            _MatToChange[counter].material = collectMaterial;
        }
        else
        {
            counter = 2;
        }

    }

    public void SlowDown()
    {
        NoJumps();
        isSlow = true;

    }

    public void TakeTailOff()
    {
        NoJumps();
        tailMovement.segmentDistance = 0;
        isTailOff = true;
        //start growing
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collector>() != null)
        {
            CollectedFood();
            other.gameObject.SetActive(false);
        }

        if (other.GetComponent<SlowTailDamage>())
        {
            if (!other.GetComponent<SlowTailDamage>().shouldTakeTail)
                SlowDown();

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SlowTailDamage>())
        {
            isSlow = false;

        }
    }



}
