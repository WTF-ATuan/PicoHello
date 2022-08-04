using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScrollC : MonoBehaviour
{

    public float ScrollX = 0.5f;
    public float ScrollY = 0.5f;
 
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = Time.time * ScrollX;
        float offsetY = Time.time * ScrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
