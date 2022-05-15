using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    [SerializeField] private CharacterController _charController;
    [SerializeField] private Material _originalMat;
    [SerializeField] private Material collectMaterial;

    [SerializeField] private MeshRenderer[] _MatToChange;

    private int counter = 0;



    private void Start()
    {

        foreach (var mat in _MatToChange)
        {
            mat.material = _originalMat;
        }
    }



    private void CollectedFood()
    {


        if (counter < 3)
        {
            _MatToChange[counter].material = collectMaterial;
        }
        else
        {
            counter = 0;
        }
        counter += 1;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collector>() != null)
        {
            CollectedFood();
            other.gameObject.SetActive(false);
        }
    }


    private void CheckTailForJump()
    {
        switch (counter)
        {
            case 0:

                break;

            case 1:

                break;

            case 2:

                break;



            default:
                break;
        }
    }
}
