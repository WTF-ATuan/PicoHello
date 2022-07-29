using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve_moving : MonoBehaviour
{
    public float Speed = 50;
    public AnimationCurve SpeedCurve;



    float avSpeed;
    float ori_d;

    GameObject Player_Hand_Pos;
    public GameObject Test;
    void OnEnable()
    {
        Player_Hand_Pos = GameObject.Find("Player_Hand_Pos");

        ori_d = Vector3.Distance(transform.position, Player_Hand_Pos.transform.position);
    }

    void Update()
    {
        Test.SetActive(true);
        float d = Vector3.Distance(transform.position, Player_Hand_Pos.transform.position);

        avSpeed = Speed * ori_d / d;
        transform.position = Vector3.Lerp(transform.position, Player_Hand_Pos.transform.position, avSpeed * Time.deltaTime * SpeedCurve.Evaluate(1- d/ori_d ) );

    }
}
