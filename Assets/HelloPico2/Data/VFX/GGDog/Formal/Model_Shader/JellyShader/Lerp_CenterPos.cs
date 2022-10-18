using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Lerp_CenterPos : MonoBehaviour
{
    public float lerp=0.05f;

    Vector3 ChasePos;


    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {
        currentDir = transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        float Speed = Vector3.Magnitude(deltaDir);

        ChasePos = Vector3.Lerp(ChasePos, transform.position, 1/Speed);
        //ChasePos = Vector3.Lerp(ChasePos, transform.position, 1);

        Shader.SetGlobalVector("Jelly_LerpPos", -ChasePos/ ChasePos.z);
    }
}
