using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneColorFade : MonoBehaviour
{
    public enum Level
    {
        _0_intro_黑暗中,
        _1_第一關_精靈樂園,
        _2_第二關_樹根,
        _3_第三關_紫色遺跡,
        _4_轉現實_紫色遺跡,
        _5_回到遊戲_覺醒的黃昏,
        _6_第四關_雲洞,
        _7_顏色漸亮_雲洞,
        _8_End_大結局
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
            case Level._0_intro_黑暗中:
                ColorFade(_m0);
                break;


            case Level._1_第一關_精靈樂園:
                ColorFade(_m1);
                break;


            case Level._2_第二關_樹根:
                ColorFade(_m2);
                break;


            case Level._3_第三關_紫色遺跡:
                ColorFade(_m3);
                break;


            case Level._4_轉現實_紫色遺跡:
                break;


            case Level._5_回到遊戲_覺醒的黃昏:
                break;


            case Level._6_第四關_雲洞:
                break;


            case Level._7_顏色漸亮_雲洞:
                break;


            case Level._8_End_大結局:
                break;
        }
    }
}
