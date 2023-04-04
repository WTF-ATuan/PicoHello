using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassObj : MonoBehaviour
{

    public float stiffness = 60f; //彈力系數
    public float damping = 2f;   //阻尼係數
    public float mass = 2f;      //質量

    GameObject followPos;  //從動點
    Vector3 followPos_Velocity = Vector3.zero;//從動點速度
    void Start()
    {
        followPos = new GameObject();
        followPos.transform.position = transform.position; //從動點歸零
    }

    void Update()
    {
        //虎克定律: F = k*X
        Vector3 force = stiffness * (transform.position - followPos.transform.position);  
       
        //阻尼運動，拉回的力
        force -= followPos_Velocity * damping;

        followPos_Velocity += mass * force * Time.deltaTime; //質量為1, force等於加速度

        followPos.transform.position += followPos_Velocity * Time.deltaTime;

    }


}
