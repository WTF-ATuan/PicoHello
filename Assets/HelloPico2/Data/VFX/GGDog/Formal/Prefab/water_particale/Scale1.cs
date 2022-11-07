using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale1 : MonoBehaviour
{

    Vector3 Ori_Scale;

    void OnEnable()
    {
        Ori_Scale = new Vector3(
            Mathf.Abs(transform.localScale.x)
            , Mathf.Abs(transform.localScale.y)
            , Mathf.Abs(transform.localScale.z)
            );
    }


    void Update()
    {
        transform.localScale = Ori_Scale;
    }
}
