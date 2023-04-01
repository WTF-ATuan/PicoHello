using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Moving : MonoBehaviour
{

    public float speed;

    [HideInInspector]
    [Range(0, 1)]
    public float FaultValue = 0.75f;


    [HideInInspector]
    public Transform Parent;

    void ToParent(Transform Parent)
    {
        transform.parent = Parent;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.parent = null;
    }

    GameObject TargetObj;
    Vector3 Target_Dir;


    private List<(float, GameObject, Vector3)> TargetObj_List = new List<(float, GameObject,  Vector3)>();


    void Start()
    {
        ToParent(Parent);

        Target_Dir = transform.forward;

        var objs = FindObjectsOfType<Enemy>(); //�o�y�ܯӯ�A�קK����Ĳ�o�o�y

        for (int i = 0; i < objs.Length; i++)
        {
            //�p��l�u�¦V�ؼЪ���V
            Vector3 vector_obj = objs[i].transform.position - transform.position;

            float dot = Vector3.Dot(transform.forward, vector_obj.normalized);
            if ( dot > FaultValue)
            {
                TargetObj_List.Add((1-dot, objs[i].gameObject, vector_obj.normalized));  //List.Sort()���ƧǬO�Ѥp��j�A�ҥH��1-dot�ӱa
                TargetObj_List.Sort();
            }
        }

        //���G��X
        if (TargetObj_List.Count > 0)
        {
            TargetObj = TargetObj_List[0].Item2;    //���odot�̤j������
            Target_Dir = TargetObj_List[0].Item3;   //���odot�̤j�����󪺤l�u�¦V��V
        }
    }


    void Update()
    {
        
        transform.Translate(Target_Dir * speed,Space.World);

        if (TargetObj)
        {
            transform.position = Vector3.Lerp(transform.position, TargetObj.transform.position, 0.025f);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.025f);


        if(transform.localScale.z<0.1f)
        {
            Destroy(gameObject);
        }

    }
}
