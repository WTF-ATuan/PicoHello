using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Anime_Key_BackLight : MonoBehaviour
{

    [Range(0,1)]

    public float _BackLightLerp;

    [Range(0, 1)]
    public float _cloudLerp;
    public Material cloud_m;


    Color cloud_m_col;
    private void Awake()
    {
        cloud_m_col = cloud_m.GetColor("_Color");
    }

    private void Update()
    {
        Shader.SetGlobalFloat("_BackLightLerp", _BackLightLerp);

        cloud_m.SetColor("_Color", Color.Lerp( cloud_m_col, new Color(1, 1, 1, 1), _cloudLerp));
    }


}
