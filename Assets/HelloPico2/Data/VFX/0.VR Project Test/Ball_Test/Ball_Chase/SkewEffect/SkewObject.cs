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
        //��l��
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
        //�t��: �p���ePosition�ܤƶqdeltaPos
        currentPos = transform.position;
        deltaPos = currentPos - lastPos;
        lastPos = currentPos;

        //���W�Ƴt��
        deltaPos = Vector3.ClampMagnitude(deltaPos, 1f);
       
        //����̤p�t��
        Center.x = deltaPos.x * (Mathf.Abs(deltaPos.x) > minSpeed ? 1 : 0);
        Center.y = deltaPos.y * (Mathf.Abs(deltaPos.y) > minSpeed ? 1 : 0);
        Center.z = deltaPos.z * (Mathf.Abs(deltaPos.z) > minSpeed ? 1 : 0);
        
        //�qUp�b�����w��V
        Vector3 axis = Vector3.Cross(transform.rotation * Vector3.up, deltaPos).normalized;
        float angle = Vector3.Angle(transform.rotation * Vector3.up, deltaPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, axis) * transform.rotation, 0.75f);

        //�t���ܧ�
        float SkewValue = SkewRange * Center.magnitude;
        transform.localScale = 
            Vector3.Lerp( transform.localScale, 
            new Vector3( Origin_Scale.x  , Origin_Scale.y * ( 1 + SkewValue ) , Origin_Scale.z )
            ,0.75f);

        //����̤jscale
        transform.localScale = new Vector3(1, Mathf.Clamp(transform.localScale.y , 0, maxScale),1);
        
        switch (_Type)
        {
            case Type.Free:
                //�N�l����^��A�ܩl�ܲ׳��O�����
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(-transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y, -transform.rotation.eulerAngles.z);
                break;
                
            case Type.Lookat_Up:
                //�HUp�b���H�ܧιB�ʡA�쥻�N�O�o�˰��ҥH�o�̴N���ΰ�����F
                break;
                
            case Type.Lookat_NegativeUp:
                //�ϦV�b�A�A�X�ΨӰ����K������P���ĪG
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(0,0,-180);
                break;
        }
    }

}

