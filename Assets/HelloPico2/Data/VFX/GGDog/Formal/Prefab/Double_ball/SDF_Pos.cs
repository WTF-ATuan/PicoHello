using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDF_Pos : MonoBehaviour
{

    public GameObject GO;

    Material _m;

    void OnEnable()
    {
        _m = GetComponent<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        _m.SetVector("SDF_Pos", GO.transform.position);

    }
}
