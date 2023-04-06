using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Global_Pos : MonoBehaviour
{


    public Material _m;

    void Start()
    {
        //_m = GetComponent<MeshRenderer>().material;
    }


    void Update()
    {
        _m.SetVector("GLOBAL_Pos", transform.position);
    }
}
