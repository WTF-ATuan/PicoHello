using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpFixPos : MonoBehaviour
{
    public GameObject Controller;
    public float Fix_Value = 0.2f;
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Controller.transform.position, 1- Fix_Value);
        transform.rotation = Quaternion.Lerp(transform.rotation, Controller.transform.rotation, 1 - Fix_Value);
    }
}
