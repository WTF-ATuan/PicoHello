using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Env_FogSetting : MonoBehaviour
{

    [Header(" ")]
    [Header("¡X¡X¡X¡X¡X¡X¡X[ Fog Color ]¡X¡X¡X¡X¡X¡X¡X¡X")]
    public Color Fog_Color = new Color(0.85f, 0.75f, 0.9f, 1);

    [Header("¡X--------------------------------------¡X")]
    [Header("¡X¡X¡X¡X[ SDF Near Camera Defrost ]¡X¡X¡X¡X")]

    public float DefrostRange = 25;

    [Range(-1,1)]
    public float DefrostRange_Intense = 0;

    [Header("¡X--------------------------------------¡X")]
    [Header("¡X¡X¡X¡X¡X¡X¡X[ Height Fog ]¡X¡X¡X¡X¡X¡X¡X")]
    public float HeightFog_pos = -15;

	public float HeightFog_Range = 30;


    [Header("¡X--------------------------------------¡X")]
    [Header("¡X¡X¡X¡X¡X¡X¡X¡X[ Far Fog ]¡X¡X¡X¡X¡X¡X¡X¡X")]
    public float FarFog_Range = 200;

    [Range(0, 1)]
    public float FarFog_Intense = 0;


    [Header("¡X--------------------------------------¡X")]
    [Header("¡X¡X¡X¡X¡X¡X¡X¡X[ Depth Dark ]¡X¡X¡X¡X¡X¡X¡X¡X")]
    public Color DepthDark_Color = new Color(0.2f, 0.15f, 0.25f, 1);

    public float DepthDark_pos = -15;
    public float DepthDark_Range = 30;


    public float FarDark_Range = 200;

    [Range(0, 1)]
    public float FarDark_Intense = 0;


    void Update()
    {
        //Global Shader

        Shader.SetGlobalVector("GLOBAL_Pos", transform.position);


        DefrostRange = Mathf.Max(DefrostRange, 0);
        Shader.SetGlobalFloat("GLOBAL_DefrostRange", DefrostRange);
        Shader.SetGlobalFloat("GLOBAL_DefrostRange_Intense", DefrostRange_Intense);


        Shader.SetGlobalFloat("GLOBAL_HeightFog_pos", HeightFog_pos);

        HeightFog_Range = Mathf.Max(HeightFog_Range, 0);
        Shader.SetGlobalFloat("GLOBAL_HeightFog_Range", HeightFog_Range);



        FarFog_Range = Mathf.Max(FarFog_Range, 0);
        Shader.SetGlobalFloat("GLOBAL_FarFog_Range", FarFog_Range);
        Shader.SetGlobalFloat("GLOBAL_FarFog_Intense", FarFog_Intense);

        Shader.SetGlobalColor("GLOBAL_Fog_Color", Fog_Color);

        Camera.main.backgroundColor = Fog_Color;




        Shader.SetGlobalFloat("GLOBAL_DepthDark_pos", DepthDark_pos);

        DepthDark_Range = Mathf.Max(DepthDark_Range, 0);
        Shader.SetGlobalFloat("GLOBAL_DepthDark_Range", DepthDark_Range);

        Shader.SetGlobalColor("GLOBAL_DepthDark_Color", DepthDark_Color);


        FarFog_Range = Mathf.Max(FarFog_Range, 0);
        Shader.SetGlobalFloat("GLOBAL_FarDark_Range", FarDark_Range);
        Shader.SetGlobalFloat("GLOBAL_FarDark_Intense", FarDark_Intense);

    }
}
