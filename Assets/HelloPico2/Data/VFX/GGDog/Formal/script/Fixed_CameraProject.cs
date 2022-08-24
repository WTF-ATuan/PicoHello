using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Fixed_CameraProject : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Camera>().projectionMatrix = Camera.main.projectionMatrix;
    }
    
}
