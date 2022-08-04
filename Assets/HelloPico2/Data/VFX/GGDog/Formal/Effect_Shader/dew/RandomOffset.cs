using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomOffset : MonoBehaviour
{
    void Start()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        MeshRenderer renderer;

        props.SetVector("_RandomOffset", new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));

        renderer = GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);
    }

}
