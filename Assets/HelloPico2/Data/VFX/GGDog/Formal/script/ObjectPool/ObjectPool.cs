using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(ObjectPool_Spawner))]
[RequireComponent(typeof(ObjectPool_Spawner_Range))]

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;

    private Queue<GameObject> m_pool = new Queue<GameObject>();

    ObjectPool _ObjectPool;
    [HideInInspector] public ObjectPool_Spawner_Range ObjectPool_Spawner_Range;
    [HideInInspector] public ObjectPool_Spawner ObjectPool_Spawner;

    private void Awake()
    {
        _ObjectPool = GetComponent<ObjectPool>();
    }


    public void Instantiate(Vector3 position, Quaternion rotation ,float size)
    {
        //從物件池提取物件出來使用
        if (m_pool.Count > 0)
        {
            GameObject reuse = m_pool.Dequeue();
            reuse.transform.localPosition = position ;
            reuse.transform.localRotation = rotation ;
            reuse.transform.localScale = new Vector3(size, size, size);
            reuse.SetActive(true);
        }
        //自動補齊物件(生成)
        else
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.GetComponent<ObjectPool_moving>().ObjectPool_Spawner_Range = ObjectPool_Spawner_Range;
            go.GetComponent<ObjectPool_moving>().ObjectPool_Spawner = ObjectPool_Spawner;
            go.GetComponent<ObjectPool_moving>()._ObjectPool = _ObjectPool;
            go.transform.parent = transform;
            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.transform.localScale = new Vector3(size, size, size);
        }
    }
    
    //重置物件回物件池
    public void Destroy(GameObject Reset)
    {
        m_pool.Enqueue(Reset);
        Reset.SetActive(false);
    }
}