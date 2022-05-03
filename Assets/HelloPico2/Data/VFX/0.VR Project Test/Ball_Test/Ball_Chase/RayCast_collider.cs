using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast_collider : MonoBehaviour
{

    public float RayLength = 0.35f; //�g�u����

    public GameObject Hitting_Effect; //�����S�Ī���

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
        //�g�u�I�����collider��
        if (Physics.Raycast(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up), out hit, RayLength))
        {
            if (hit.collider)
            {
                Debug.DrawRay(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up) * hit.distance, Color.green);  //��ܮg�u�����
            }
            //�����S��
            Hitting_Effect.transform.position = hit.point;
            Hitting_Effect.SetActive(false);
            Hitting_Effect.SetActive(true);

            //��������
            if (_Ball_Throw) { _Ball_Throw.enabled = false; } 
            gameObject.SetActive(false);
        }

        //���I���
        else
        {
            Debug.DrawRay(transform.localPosition - Vector3.up * RayLength / 2, transform.TransformDirection(Vector3.up) * RayLength, Color.white);  //��ܮg�u���զ�
        }

        
    }
}
