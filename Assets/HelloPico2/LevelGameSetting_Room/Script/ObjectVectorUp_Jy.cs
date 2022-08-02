using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVectorUp_Jy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Vector3.up);
    }
}
