using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class forwardnormal : MonoBehaviour
{

    public Vector3 v;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        v = Vector3.Normalize(transform.forward);
    }
}
