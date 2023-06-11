using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindObject : MonoBehaviour
{


    [Range(0,1)]
    public float FaultValue = 0.75f;

    void Update()
    {


    }


    GameObject TargetObj;
    GameObject FindTarget()
    {
        var objs = FindObjectsOfType<Enemy>(); //這句很耗能，避免持續觸發這句

        for (int i = 0; i < objs.Length; i++)
        {
            Vector3 Controller_To_obj = objs[i].transform.position - transform.position;

            if (Vector3.Dot(transform.forward, Controller_To_obj) > FaultValue)
            {
                TargetObj = objs[i].gameObject;
            }
        }
        return TargetObj;
    }

}
