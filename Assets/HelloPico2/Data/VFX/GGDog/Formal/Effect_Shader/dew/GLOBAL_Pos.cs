using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLOBAL_Pos : MonoBehaviour
{
    public float distance;

    GameObject[] gos;

    void Update()
    {
        Vector4 pos = transform.position;
        Shader.SetGlobalVector("GLOBAL_Pos", pos);

        /*
        gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        Vector4[] gos2 = new Vector4[gos.Length];

        int j = 0;
        for (int i = 0; i < gos.Length; i++)
        {
            gos2[i] = pos;
            if (gos[i].layer == 8 && Vector3.Distance(gos[i].transform.position, pos)<= distance)
            {
                if (gos[i].GetComponent<MeshRenderer>()&&gos[i].GetComponent<MeshRenderer>().sharedMaterial.name == "dew_GPUInstance") 
                { 
                    gos2[j] = gos[i].transform.position;
                    j++;
                }
            }
        }
        Shader.SetGlobalVectorArray("All_Pos", gos2);*/
    }


    [ContextMenu("[ResetPos]")]
    void ResetPos()
    {
        Vector4[] gos2 = new Vector4[10];

        for (int i = 0; i < 10; i++)
        {
            gos2[i] = transform.position;
        }
        Shader.SetGlobalVectorArray("All_Pos", gos2);
    }
}
