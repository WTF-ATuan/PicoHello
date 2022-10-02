using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CenterFadeTrail_GlobalPos : MonoBehaviour
{

    void Update()
    {
        Shader.SetGlobalVector("CenterFadeTrail_GlobalPos", transform.localPosition);
    }
}
