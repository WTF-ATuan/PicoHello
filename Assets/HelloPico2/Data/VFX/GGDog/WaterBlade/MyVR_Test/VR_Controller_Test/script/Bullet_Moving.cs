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

        var objs = FindObjectsOfType<Enemy>(); //這句很耗能，避免持續觸發這句

        for (int i = 0; i < objs.Length; i++)
        {
            //計算子彈朝向目標的方向
            Vector3 vector_obj = objs[i].transform.position - transform.position;

            float dot = Vector3.Dot(transform.forward, vector_obj.normalized);
            if ( dot > FaultValue)
            {
                TargetObj_List.Add((1-dot, objs[i].gameObject, vector_obj.normalized));  //List.Sort()的排序是由小到大，所以用1-dot來帶
                TargetObj_List.Sort();
            }
        }

        //結果輸出
        if (TargetObj_List.Count > 0)
        {
            TargetObj = TargetObj_List[0].Item2;    //取得dot最大的物件
            Target_Dir = TargetObj_List[0].Item3;   //取得dot最大的物件的子彈朝向方向
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
