using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Env_FogSetting : MonoBehaviour
{

    [Header(" -------- Fog Color -------- ")]
    public Color Fog_Color = new Color(0.85f, 0.75f, 0.9f, 1);

    [Header(" --- SDF Near Camera Defrost --- ")]

    public float DefrostRange = 25;

    [Range(-1,1)]
    public float DefrostRange_Intense = 0;

    [Header(" -------- Height Fog -------- ")]
    public float HeightFog_pos = -15;

	public float HeightFog_Range = 30;


    [Header(" -------- Far Fog -------- ")]
    public float FarFog_Range = 200;

    [Range(0, 1)]
    public float FarFog_Intense = 0;


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
    }
}
