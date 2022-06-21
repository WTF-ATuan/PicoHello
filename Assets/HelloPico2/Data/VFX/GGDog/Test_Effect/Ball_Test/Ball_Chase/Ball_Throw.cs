using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Throw : MonoBehaviour
{
    
    public float Speed = 50;

    public AnimationCurve SpeedCurve;

    public Vector3 Direction;

    float avSpeed;

    float SpeedCurve_Time = 0;

    TrailRenderer _TrailRenderer;
    private void Awake()
    {
        //_TrailRenderer = transform.GetChild(0).GetComponent<TrailRenderer>();
    }

    GameObject Player_Hand_Pos;
    void OnEnable()
    {
        SpeedCurve_Time = 0;
        //_TrailRenderer.enabled = true;
    }
    float Distance;
    void FixedUpdate()
    {
        Player_Hand_Pos = GameObject.Find("Player_Hand_Pos");
        //�P��߶Z������scale�A�קK�b��x�N�L���Ԧ�
        Distance = Vector3.Distance(Player_Hand_Pos.transform.position, transform.position);
        Distance = Distance * Distance * Distance * Distance * Distance * Distance * Distance;
        Distance = Mathf.Clamp(Distance, 0, 1);
        //�t���ܧ�
        //Skew(SpeedCurve, SpeedCurve_Time, Direction, 50*(1+Distance)/2);

        avSpeed = Speed / Vector3.Distance(transform.position, Direction * 1000);
        transform.position = Vector3.Lerp(transform.position, Direction*1000, avSpeed * Time.deltaTime * SpeedCurve.Evaluate(SpeedCurve_Time));

        SpeedCurve_Time += Time.deltaTime;
    }

    
    //�t���ܧ�
    void Skew(AnimationCurve Curve, float Curve_Time,Vector3 Direction, float SkewRange)
    {
        //�qUp�b�����w��V
        Vector3 axis = Vector3.Cross(transform.rotation * Vector3.up, Direction).normalized;
        float angle = Vector3.Angle(transform.rotation * Vector3.up, Direction);
        transform.rotation = Quaternion.AngleAxis(angle, axis) * transform.rotation;

        float SkewValue = SkewRange * Mathf.Clamp(Curve.Evaluate(Curve_Time), 0, 1);

        transform.localScale =
            Vector3.Lerp(transform.localScale,
            new Vector3(1, SkewValue, 1)
            , 1f);

        //����̤jscale
        transform.localScale = 
            new Vector3(transform.localScale.x, 
                        Mathf.Clamp(transform.localScale.y, 0, 10 * (1 + Distance))
                       ,transform.localScale.z);
    }
}
