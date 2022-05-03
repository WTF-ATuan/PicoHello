using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;

    private Queue<GameObject> m_pool = new Queue<GameObject>();
    
    public void Instantiate(Vector3 position, Quaternion rotation)
    {
        //�q�������������X�Өϥ�
        if (m_pool.Count > 0)
        {
            GameObject reuse = m_pool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        //�۰ʸɻ�����
        else
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.transform.parent = transform;
            go.transform.position = position;
            go.transform.rotation = rotation;
        }
    }
    
    //���m����^�����
    public void Destroy(GameObject Reset)
    {
        m_pool.Enqueue(Reset);
        Reset.SetActive(false);
    }
}