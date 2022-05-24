using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTailDamage : MonoBehaviour
{
    public enum AOETail { slow, tailoff, adapt, none}


    [SerializeField] public AOETail aoeTail = AOETail.slow;




}
