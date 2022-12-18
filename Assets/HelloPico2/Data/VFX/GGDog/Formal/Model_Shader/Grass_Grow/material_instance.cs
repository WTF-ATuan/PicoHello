using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class material_instance : MonoBehaviour
{
    Material _m;
    void Awake()
    {
        _m = GetComponent<MeshRenderer>().material;
    }

}
