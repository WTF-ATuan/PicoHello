using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_WaterPos : MonoBehaviour
{
    [Range(0,1)]
    public float Pos_Y;


    public MeshRenderer _Glass;
    public MeshRenderer _GlassBack;

    void OnEnable()
    {
       // _Glass.material = GetComponent<MeshRenderer>().material;
       // _GlassBack.material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {

        float Full_Pos = 1.2f * (Pos_Y * 2 - 1) / 10;

        _Glass.material.SetVector("Fixed_WorldPos", transform.position);
        _Glass.material.SetFloat("FullPos", Full_Pos);

        _GlassBack.material.SetVector("Fixed_WorldPos", transform.position);
        _GlassBack.material.SetFloat("FullPos", Full_Pos);
    }
}
