using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLOBAL_Pos : MonoBehaviour
{
    void Update()
    {
        Vector4 pos = transform.position;
        Shader.SetGlobalVector("GLOBAL_Pos", pos);
    }
}
