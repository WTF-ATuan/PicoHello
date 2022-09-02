using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class LOD_GlobalController : MonoBehaviour
{

    public enum Max_LOD { Very_High_DepthTexture, High_Reflection, Medium_dew_and_Env, Low , Partical, None }

    public Max_LOD _LOD;

    GameObject Camera_RT;
    void OnEnable()
    {
        Camera_RT = GameObject.Find("Camera_RT");

        Camera_RT = FindInActiveObjectByName("Camera_RT");

        //修正RT用Camera的畫面尺寸
       // Camera_RT.GetComponent<Camera>().projectionMatrix = Camera.main.projectionMatrix;


        switch (_LOD)
        {
            case Max_LOD.Very_High_DepthTexture:

                Camera_RT.SetActive(true);
                Camera.main.depthTextureMode |= DepthTextureMode.Depth;
                Shader.globalMaximumLOD = 400;

                break;


            case Max_LOD.High_Reflection:

                Camera_RT.SetActive(true);
                Camera.main.depthTextureMode |= DepthTextureMode.None;
                Shader.globalMaximumLOD = 300;

                break;


            case Max_LOD.Medium_dew_and_Env:

                Camera_RT.SetActive(true);
                Camera.main.depthTextureMode |= DepthTextureMode.None;
                Shader.globalMaximumLOD = 200;

                break;


            case Max_LOD.Low:

                Camera_RT.SetActive(false);
                Camera.main.depthTextureMode |= DepthTextureMode.None;
                Shader.globalMaximumLOD = 100;

                break;

                
            case Max_LOD.Partical:

                Camera_RT.SetActive(false);
                Camera.main.depthTextureMode |= DepthTextureMode.None;
                Shader.globalMaximumLOD = 1;

                break;
            case Max_LOD.None:

                Camera_RT.SetActive(false);
                Camera.main.depthTextureMode |= DepthTextureMode.None;
                Shader.globalMaximumLOD = 0;

                break;
        }
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }


    
}
