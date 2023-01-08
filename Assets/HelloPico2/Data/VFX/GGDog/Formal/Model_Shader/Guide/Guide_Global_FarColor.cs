using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class Guide_Global_FarColor : MonoBehaviour
{
    public float _Guide_Far = 60;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_intro;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_Lv1;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_Lv2;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_Lv3;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_Lv4;

    [ColorUsage(true, true)]
    public Color _Guide_FarColor_Lv5;

    void Update()
    {
        Shader.SetGlobalFloat("_Guide_Far", _Guide_Far);
        Level_switch();
    }

    void Level_switch()
    {

        switch (Level_FadeController.now_level)
        {

            case 0: //Intro
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_intro);
                break;

            case 1: //¶³®ü
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_Lv1);
                break;

            case 2: //¸Â¼Ó
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_Lv2);
                break;

            case 3: //¶Â·t¤§´Ë
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_Lv3);
                break;

            case 5 : //¶³¬}
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_Lv4);
                break;

            case 6 : //¹Ú¤Û¶³®ü
                Shader.SetGlobalColor("_Guide_FarColor", _Guide_FarColor_Lv5);
                break;
        }
    }
}
