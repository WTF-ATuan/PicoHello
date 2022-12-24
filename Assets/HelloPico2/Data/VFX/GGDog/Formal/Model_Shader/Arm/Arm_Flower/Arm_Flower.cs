using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arm_Flower : MonoBehaviour
{
    [Range(0, 1)]
    public float _h;
    [Range(0, 1)]
    public float _injured;

    public Material _m1;
    public Material _m2;
    public Material _m3;
    public Material _m4;

    public ParticleSystem PS;

    ParticleSystem injured_PS_PointGlow;
    ParticleSystem injured_PS_BlackPiece;
    void OnEnable()
    {
        injured_PS_PointGlow = PS.transform.GetChild(0).GetComponent<ParticleSystem>();
        injured_PS_BlackPiece = PS.transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        ParticleSystem ps = PS;
        var em = ps.emission;
        em.rateOverTime = 20+_h * 70;

        float i = _injured * (1 - _injured) * 4;

        ParticleSystem ps_injured = injured_PS_PointGlow;
        var em_injured = ps_injured.emission;
        em_injured.rateOverTime = i * 120;

        ParticleSystem ps_injured2 = injured_PS_BlackPiece;
        var em_injured2 = ps_injured2.emission;
        em_injured2.rateOverTime = i * 150;

        _m1.SetFloat("_h", _h);
        _m2.SetFloat("_h", (_h - 0.25f) * (4));
        _m3.SetFloat("_h", (_h - 0.50f) * (4));
        _m4.SetFloat("_h", (_h - 0.75f) * (4));



        _m1.SetFloat("_injured", _injured * 1.5f);
        _m2.SetFloat("_injured", _injured * 1.5f);
        _m3.SetFloat("_injured", _injured * 1.5f);
        _m4.SetFloat("_injured", _injured * 1.5f);
    }
}
