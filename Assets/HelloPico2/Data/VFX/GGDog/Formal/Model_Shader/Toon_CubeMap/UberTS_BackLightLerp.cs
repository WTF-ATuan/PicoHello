using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UberTS_BackLightLerp : MonoBehaviour
{
    [Range(0, 1)]

    public float _BackLightLerp;

    void Update()
    {
        Shader.SetGlobalFloat("_BackLightLerp", _BackLightLerp);

    }
}
