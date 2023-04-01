using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Moving : MonoBehaviour
{

    public float speed;

    [Range(0, 1)]
    public float FaultValue = 0.75f;

    [HideInInspector]
    public Vector3 Forward;

    GameObject TargetObj;

    Vector3 Target_Dir;

    public Transform Parent;


    void ToParent(Transform Parent)
    {
        transform.parent = Parent;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.parent = null;
    }

    void Start()
    {
        ToParent(Parent);

        Target_Dir = transform.forward;

        var objs = FindObjectsOfType<Enemy>(); //這句很耗能，避免持續觸發這句

        for (int i = 0; i < objs.Length; i++)
        {
            Vector3 vector_obj = objs[i].transform.position - transform.position;

            if (Vector3.Dot(transform.forward, vector_obj.normalized) > FaultValue)
            {
                Target_Dir = vector_obj.normalized;
                TargetObj = objs[i].gameObject;
            }
        }

    }


    void Update()
    {

        transform.Translate(Target_Dir * speed,Space.World);

        if (TargetObj)
        {
            transform.position = Vector3.Lerp(transform.position, TargetObj.transform.position, 0.01f);
        }

    }
}
