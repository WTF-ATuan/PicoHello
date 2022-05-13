using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glory : MonoBehaviour
{

    [Range(0, 100)]
    public int scale = 50;

    [Range(0,5)]
    public int steepness = 3;

    Transform camera ;

    void OnEnable()
    {
        camera = Camera.main.transform;
    }
    
    void Update()
    {

        float glory = Vector3.Dot(Vector3.Normalize(camera.TransformDirection(Vector3.forward)), Vector3.Normalize(transform.position - camera.position));

        for(int i=0; i< steepness;i++)
        {
            glory *= glory;
        }

        transform.localScale = new Vector3(scale, scale, scale) * glory;
        
    }
}
