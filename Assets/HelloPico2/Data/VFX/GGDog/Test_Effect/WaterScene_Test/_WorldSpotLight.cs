using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class _WorldSpotLight : MonoBehaviour {

    public Color GLOBAL_SpotColor;

    [Range(0, 10)]
    public float GLOBAL_SpotIntensive = 1;

    [Range(0,10)]
    public float GLOBAL_SpotRadius = 1;
    

    void Update ()
    {
        Vector4 pos = transform.position;
        Shader.SetGlobalVector("GLOBAL_Pos", pos);
        Shader.SetGlobalColor("GLOBAL_SpotColor", GLOBAL_SpotColor * GLOBAL_SpotColor.a* GLOBAL_SpotIntensive);
        Shader.SetGlobalFloat("GLOBAL_SpotRadius", GLOBAL_SpotRadius);


        

    }
}
