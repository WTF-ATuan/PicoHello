using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arm_Flower : MonoBehaviour
{
    [Range(0,1)]
    public float _h;

    public Material _m1;
    public Material _m2;
    public Material _m3;
    public Material _m4;

    public ParticleSystem PS;

    void Start()
    {
        
    }

    void Update()
    {
        ParticleSystem ps = PS; 
        var em = ps.emission;
        em.rateOverTime = _h * 200;

        _m1.SetFloat("_h", _h );
        _m2.SetFloat("_h", (_h - 0.25f ) * (4) );
        _m3.SetFloat("_h", (_h - 0.50f ) * (4) );
        _m4.SetFloat("_h", (_h - 0.75f ) * (4) );
    }
}
