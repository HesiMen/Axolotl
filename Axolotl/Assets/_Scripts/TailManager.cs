using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    [SerializeField] private CharacterController _charController;
    [SerializeField] private Material _originalMat;
    [SerializeField] private Material collectMaterial;

    [SerializeField] private MeshRenderer[] _MatToChange;

    public int counter = -1;



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




    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collector>() != null)
        {
            CollectedFood();
            other.gameObject.SetActive(false);
        }
    }


   
}
