using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Phys_JointMaxDistance : MonoBehaviour
{

    public GameObject Parent;

    public float MaxDistance = 1;

    public Color32 RayCol;
    public Color32 RaySphereCol;
    public float RaySphereR = 0.025f;


    Vector3 OriPos;
    void OnEnable()
    {
       // OriPos = transform.localPosition - transform.parent.GetChild(0).transform.localPosition;
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, Parent.transform.position , RayCol);
        Debug.DrawSphere(transform.position, RaySphereR, RaySphereCol);
    }

    void Update()
    {
        // transform.localPosition = Vector3.Lerp(transform.localPosition, OriPos+transform.parent.GetChild(0).transform.localPosition, 0.025f);

        //�쵲:����⪫�餧�����Z��

        float d = Distance_NoSquareRoot(transform.position, Parent.transform.position);

        if (d >= MaxDistance* MaxDistance)
        {
            //���o�⪫�餧�������V�q�A���H�̤j�������
            Vector3 pos = (transform.position - Parent.transform.position).normalized * MaxDistance;

            //�b�s�������m�[�W�������q
            transform.position = pos + Parent.transform.position;
        }

    }

    float Distance_NoSquareRoot(Vector3 Pos,Vector3 Pos2)
    {
        return 
            (Pos.x - Pos2.x) * (Pos.x - Pos2.x) +
            (Pos.y - Pos2.y) * (Pos.y - Pos2.y) +
            (Pos.z - Pos2.z) * (Pos.z - Pos2.z);
    }
}
