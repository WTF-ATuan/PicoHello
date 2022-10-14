using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UVRandomColor : MonoBehaviour
{

    Material _m;
    void Start()
    {
        _m = GetComponent<SkinnedMeshRenderer>().material;

      //  _m.SetVector("_RandomOffset", new Vector2((float)Random.Range(0, 5)/5, (float)Random.Range(0, 2)/2));


        _m.SetColor("_FadeColor1", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),1));
        _m.SetColor("_FadeColor2", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1));
    }

}
