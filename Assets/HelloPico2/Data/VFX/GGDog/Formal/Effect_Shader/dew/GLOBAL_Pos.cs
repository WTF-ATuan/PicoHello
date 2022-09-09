using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLOBAL_Pos : MonoBehaviour
{
    GameObject[] gos;

    Vector3 followPos;

    private void OnEnable()
    {
        followPos = new Vector3(transform.position.x, transform.position.y, 10f);
    }
    void Update()
    {
        Vector3 pos = transform.position;

        followPos = Vector3.Lerp(followPos, transform.position, 0.05f);

        Shader.SetGlobalVector("GLOBAL_Pos", pos);

        Shader.SetGlobalVector("Follow_Pos", followPos);

    }
}
