using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Lerp : MonoBehaviour
{
    public GameObject Parent;
    public float Speed;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Parent.transform.position, Speed);
    }
}
