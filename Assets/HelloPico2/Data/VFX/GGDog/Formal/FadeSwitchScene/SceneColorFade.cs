using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneColorFade : MonoBehaviour
{
    public enum Level
    {
        _0_intro_�·t��,
        _1_�Ĥ@��_���F�ֶ�,
        _2_�ĤG��_���,
        _3_�ĤT��_������,
        _4_��{��_������,
        _5_�^��C��_ı��������,
        _6_�ĥ|��_���},
        _7_�C�⺥�G_���},
        _8_End_�j����
    }

    public Level level;

    public GameObject GO;

    public string[] Sky_VarNames;
    public string[] Env_VarNames;
    public string[] Ground_VarNames;

    public Material _m0;
    public Material _m1;
    public Material _m2;
    public Material _m3;

    [Range(0, 0.01f)]
    public float speed;

    Material _Go;
    
    void OnEnable()
    {
        if (!GetComponent<ParticleSystem>())
        {
            gameObject.AddComponent<ParticleSystem>();
            ParticleSystem _p = GetComponent<ParticleSystem>();
            _p.hideFlags = HideFlags.HideInInspector;
        }

        _Go = GO.GetComponent<Renderer>().sharedMaterial;
        
        Level_switch();
    }
    
    void Update()
    {
        Level_switch();
        
        // _Go.SetFloat("_Clip", Mathf.Lerp(_m1.GetFloat("_Clip"), _m2.GetFloat("_Clip"), value));

    }

    void ColorFade(Material m)
    {
        for (int i = 0; i < Sky_VarNames.Length; i++)
        {
            _Go.SetColor(Sky_VarNames[i], Color.Lerp(_Go.GetColor(Sky_VarNames[i]), m.GetColor(Sky_VarNames[i]), speed));
        }
    }

    
    void Level_switch()
    {
        switch (level)
        {
            case Level._0_intro_�·t��:
                ColorFade(_m0);
                break;


            case Level._1_�Ĥ@��_���F�ֶ�:
                ColorFade(_m1);
                break;


            case Level._2_�ĤG��_���:
                ColorFade(_m2);
                break;


            case Level._3_�ĤT��_������:
                ColorFade(_m3);
                break;


            case Level._4_��{��_������:
                break;


            case Level._5_�^��C��_ı��������:
                break;


            case Level._6_�ĥ|��_���}:
                break;


            case Level._7_�C�⺥�G_���}:
                break;


            case Level._8_End_�j����:
                break;
        }
    }
}
