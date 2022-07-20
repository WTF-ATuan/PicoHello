using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_CameraProject : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().projectionMatrix = Camera.main.projectionMatrix;
    }
    
}
