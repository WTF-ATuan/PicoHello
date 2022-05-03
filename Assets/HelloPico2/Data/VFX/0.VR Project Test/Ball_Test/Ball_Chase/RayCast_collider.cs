using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast_collider : MonoBehaviour
{

    public float RayLength = 0.35f; //射線長度

    public GameObject Hitting_Effect; //擊中特效物件

    RaycastHit hit;


    Ball_Throw _Ball_Throw;

    private void Awake()
    {
        if (GetComponent<Ball_Throw>())
        {
            _Ball_Throw = GetComponent<Ball_Throw>();
        }
    }

    void Update()
    {
        //射線碰到任何collider時
        if (Physics.Raycast(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up), out hit, RayLength))
        {
            if (hit.collider)
            {
                Debug.DrawRay(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up) * hit.distance, Color.green);  //顯示射線為綠色
            }
            //擊中特效
            Hitting_Effect.transform.position = hit.point;
            Hitting_Effect.SetActive(false);
            Hitting_Effect.SetActive(true);

            //關閉物件
            if (_Ball_Throw) { _Ball_Throw.enabled = false; } 
            gameObject.SetActive(false);
        }

        //未碰到時
        else
        {
            Debug.DrawRay(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up) * RayLength, Color.white);  //顯示射線為白色
        }

        
    }
}
