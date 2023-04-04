using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassObj : MonoBehaviour
{

    public float stiffness = 60f; //�u�O�t��
    public float damping = 2f;   //�����Y��
    public float mass = 2f;      //��q

    GameObject followPos;  //�q���I
    Vector3 followPos_Velocity = Vector3.zero;//�q���I�t��
    void Start()
    {
        followPos = new GameObject();
        followPos.transform.position = transform.position; //�q���I�k�s
    }

    void Update()
    {
        //��J�w��: F = k*X
        Vector3 force = stiffness * (transform.position - followPos.transform.position);  
       
        //�����B�ʡA�Ԧ^���O
        force -= followPos_Velocity * damping;

        followPos_Velocity += mass * force * Time.deltaTime; //��q��1, force����[�t��

        followPos.transform.position += followPos_Velocity * Time.deltaTime;

    }


}
