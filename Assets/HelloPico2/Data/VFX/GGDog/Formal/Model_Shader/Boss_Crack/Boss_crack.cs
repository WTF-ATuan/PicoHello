using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Boss_crack : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("BossCrack_GlobalPos", transform.localPosition);
    }
}
