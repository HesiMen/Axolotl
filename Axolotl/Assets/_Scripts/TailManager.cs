using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    [SerializeField] private CharacterController _charController;
    [SerializeField] private Material _originalMat;
    [SerializeField] private Material collectMaterial;
    [SerializeField] private Material adapterMaterial;
    [SerializeField] private TailMovement tailMovement;
    [SerializeField] private MeshRenderer[] _MatToChange;
    [SerializeField] private MeshRenderer[] adaptingMesh;
    public float scaleGrowth = 1f;
    public int counter = -1;
    public SlowTailDamage.AOETail currState = SlowTailDamage.AOETail.none;
    private bool isGrowing = false;
    public float growthPercent = .3f;
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
        currState = SlowTailDamage.AOETail.slow;

    }

    public void TakeTailOff()
    {
        NoJumps();
        tailMovement.segmentDistance = -.3f;
        currState = SlowTailDamage.AOETail.tailoff;
        //start growing
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collector>() != null)
        {
            Collector collected = other.GetComponent<Collector>();
            if (currState != SlowTailDamage.AOETail.tailoff)
            {
                CollectedFood();
            }
            else if (collected.isAdapt)
            {
                EnhanceGrowth();
            }
            other.gameObject.SetActive(false);
        }
        // Do slow or take tail
        if (other.GetComponent<SlowTailDamage>() != null)
        {
            SlowTailDamage slowTail = other.GetComponent<SlowTailDamage>();

            switch (slowTail.aoeTail)
            {
                case SlowTailDamage.AOETail.slow:
                    SlowDown();
                    break;
                case SlowTailDamage.AOETail.tailoff:
                    TakeTailOff();
                    break;

                case SlowTailDamage.AOETail.adapt:
                    if (!isGrowing)
                        StartCoroutine(StartTailGrowth());
                    break;
                case SlowTailDamage.AOETail.none:
                    break;
                default:
                    break;
            }


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SlowTailDamage>() != null)
        {
            SlowTailDamage slowTail = other.GetComponent<SlowTailDamage>();

            switch (slowTail.aoeTail)
            {
                case SlowTailDamage.AOETail.slow:
                    currState = SlowTailDamage.AOETail.none;
                    break;
                case SlowTailDamage.AOETail.tailoff:
                    break;

                case SlowTailDamage.AOETail.adapt:
                    break;
                case SlowTailDamage.AOETail.none:
                    break;
                default:
                    break;
            }

        }
    }


    private void EnhanceGrowth()
    {
        scaleGrowth += .3f;
    }
    IEnumerator StartTailGrowth()
    {
        isGrowing = true;
        currState = SlowTailDamage.AOETail.adapt;
        while (growthPercent < 100f)
        {

            growthPercent += Time.deltaTime * scaleGrowth;

            tailMovement.segmentDistance = (.35f * growthPercent) / 100f;
            yield return null;
        }

        foreach (var mesh in adaptingMesh)
        {
            mesh.material = adapterMaterial;
        }
    }

}
