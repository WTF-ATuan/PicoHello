using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class joint : MonoBehaviour
{

    public GameObject Parent;
    public GameObject Child;
    public float MaxDistance = 0.1f;

    void Update()
    {
        
        //鏈結:限制兩物體之間的距離
        if (Vector3.Distance(transform.position, Parent.transform.position) > MaxDistance)
        {
            //取得兩物體之間的單位向量再乘以最大限制長度
            Vector3 pos = (transform.position - Parent.transform.position).normalized * MaxDistance;

            //在連結物件位置加上此偏移量
            transform.position = pos + Parent.transform.position;
        }

        transform.position += new Vector3(0, -0.03f, 0);
    }
}
