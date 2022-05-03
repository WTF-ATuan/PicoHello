using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class SkewObject : MonoBehaviour
{

    public float SkewRange=1;
    [Range(0, 1)]
    public float minSpeed = 0.2f;

    [Range(0, 50)]
    public float maxScale = 20;

    public enum Type
    {
        Free,
        Lookat_Up,
        Lookat_NegativeUp
    }

    public Type _Type;

    Vector3 currentPos;
    Vector3 deltaPos;
    Vector3 lastPos = new Vector3();


    Vector3 Center;

    Vector3 Origin_Scale;

    private void Awake()
    {
        //初始化
        Origin_Scale = Vector3.one;
        transform.localScale = Origin_Scale;
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void OnEnable()
    {
        Awake();
    }

    void Update()
    {
        //速度: 計算當前Position變化量deltaPos
        currentPos = transform.position;
        deltaPos = currentPos - lastPos;
        lastPos = currentPos;

        //正規化速度
        deltaPos = Vector3.ClampMagnitude(deltaPos, 1f);
       
        //限制最小速度
        Center.x = deltaPos.x * (Mathf.Abs(deltaPos.x) > minSpeed ? 1 : 0);
        Center.y = deltaPos.y * (Mathf.Abs(deltaPos.y) > minSpeed ? 1 : 0);
        Center.z = deltaPos.z * (Mathf.Abs(deltaPos.z) > minSpeed ? 1 : 0);
        
        //從Up軸轉到指定方向
        Vector3 axis = Vector3.Cross(transform.rotation * Vector3.up, deltaPos).normalized;
        float angle = Vector3.Angle(transform.rotation * Vector3.up, deltaPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, axis) * transform.rotation, 0.75f);

        //速度變形
        float SkewValue = SkewRange * Center.magnitude;
        transform.localScale = 
            Vector3.Lerp( transform.localScale, 
            new Vector3( Origin_Scale.x  , Origin_Scale.y * ( 1 + SkewValue ) , Origin_Scale.z )
            ,0.75f);

        //限制最大scale
        transform.localScale = new Vector3(1, Mathf.Clamp(transform.localScale.y , 0, maxScale),1);
        
        switch (_Type)
        {
            case Type.Free:
                //將子物件回轉，至始至終都保持原度
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(-transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y, -transform.rotation.eulerAngles.z);
                break;
                
            case Type.Lookat_Up:
                //以Up軸跟隨變形運動，原本就是這樣做所以這裡就不用做什麼了
                break;
                
            case Type.Lookat_NegativeUp:
                //反向軸，適合用來做火焰類拖尾感的效果
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(0,0,-180);
                break;
        }
    }

}

