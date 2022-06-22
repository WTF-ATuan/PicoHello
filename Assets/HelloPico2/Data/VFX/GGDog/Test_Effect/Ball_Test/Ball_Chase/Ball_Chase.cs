using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Chase : MonoBehaviour
{
    public float Speed = 50;

    public AnimationCurve SpeedCurve;

    public AnimationCurve CatchCurve;

    float avSpeed;

    float SpeedCurve_Time = 0;

    float CatchCurve_Time = 0;

    bool IsChasing = true;

    Ball_Chase _Ball_Chase;
    TrailRenderer _TrailRenderer;


    RayCast_collider _RayCast_collider;
    private void Awake()
    {
        _Ball_Chase = GetComponent<Ball_Chase>();
        //_TrailRenderer =transform.GetChild(0).GetComponent<TrailRenderer>();
        _RayCast_collider = GetComponent<RayCast_collider>();
    }

    Vector3 Direction;
    Vector3 CatchShake_Direction;

    GameObject Player_Hand_Pos;
    GameObject Effect_Catch_InHand;
    void OnEnable()
    {
        Player_Hand_Pos = GameObject.Find("Player_Hand_Pos");
        Effect_Catch_InHand = Player_Hand_Pos.transform.Find("Effect_Catch_InHand").gameObject;
        CatchShake_Direction = Player_Hand_Pos.transform.position - transform.position;
        SpeedCurve_Time = 0;
        CatchCurve_Time = 0;
        IsChasing = true;
        //_TrailRenderer.enabled = false;
        _RayCast_collider.enabled = false;
    }
    float Distance;
    void FixedUpdate()
    {
        Player_Hand_Pos = GameObject.Find("Player_Hand_Pos");

        //物件到玩家手心的方向向量
        Direction = Player_Hand_Pos.transform.position - transform.position;

        Distance = Mathf.Clamp( Vector3.Distance(Player_Hand_Pos.transform.position, transform.position),0,1);

        if (IsChasing)
        {
            //速度變形
           // Skew(SpeedCurve, SpeedCurve_Time, Direction, 50f* (1 + Distance)/2);
            
            avSpeed = Speed / Vector3.Distance(transform.position, Player_Hand_Pos.transform.position);
            transform.position = Vector3.Lerp(transform.position, Player_Hand_Pos.transform.position, avSpeed * Time.deltaTime * SpeedCurve.Evaluate(SpeedCurve_Time));
            SpeedCurve_Time += Time.deltaTime;

        }

        if (IsChasing && Vector3.Distance(transform.position, Player_Hand_Pos.transform.position)<0.01f)
        {
            Effect_Catch_InHand.SetActive(false);
            Effect_Catch_InHand.SetActive(true);
            IsChasing = false;
        }

        if(!IsChasing)
        {
            //速度變形
           // Skew(CatchCurve, CatchCurve_Time, Direction, 2.5f);
            
            transform.Translate(CatchShake_Direction * Speed * Time.deltaTime * CatchCurve.Evaluate(CatchCurve_Time)/100);
            CatchCurve_Time += Time.deltaTime;

            if(CatchCurve_Time>0.2f)
            {
                transform.localScale =new Vector3(1, 1, 1);
                transform.position = Player_Hand_Pos.transform.position;
                transform.rotation = Quaternion.Euler(0,0,0);
                
                _RayCast_collider.enabled = true;

                _Ball_Chase.enabled = false;
            }
        }
    }


    //速度變形
    void Skew(AnimationCurve Curve  ,float Curve_Time,Vector3 Direction, float SkewRange )
    {
        //從Up軸轉到指定方向
        Vector3 axis = Vector3.Cross(transform.rotation * Vector3.up, Direction).normalized;
        float angle = Vector3.Angle(transform.rotation * Vector3.up, Direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, axis) * transform.rotation, 0.75f);
        
        float SkewValue = SkewRange * Mathf.Clamp(Curve.Evaluate(Curve_Time),0,1);

        transform.localScale =
            Vector3.Lerp(transform.localScale,
            new Vector3(1, 1+SkewValue, 1)
            , 0.75f);

        //限制最大scale
        transform.localScale = new Vector3(1, Mathf.Clamp(transform.localScale.y, 1, 7.5f ), 1);
    }
}
