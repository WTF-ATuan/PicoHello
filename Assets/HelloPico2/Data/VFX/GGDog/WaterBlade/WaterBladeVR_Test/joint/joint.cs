using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class joint : MonoBehaviour
{

    public GameObject Parent;
    public GameObject Child;
    public float MaxDistance = 0.1f;

    void Update()
    {
        
        //�쵲:����⪫�餧�����Z��
        if (Vector3.Distance(transform.position, Parent.transform.position) > MaxDistance)
        {
            //���o�⪫�餧�������V�q�A���H�̤j�������
            Vector3 pos = (transform.position - Parent.transform.position).normalized * MaxDistance;

            //�b�s�������m�[�W�������q
            transform.position = pos + Parent.transform.position;
        }

        transform.position += new Vector3(0, -0.03f, 0);
    }
}
