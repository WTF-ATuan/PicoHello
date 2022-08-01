using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Jy : MonoBehaviour
{
    public GameObject _prefab;
    public int initalSize = 20;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        for (int count = 0; count < initalSize; count++){
            GameObject insPrefab = Instantiate(_prefab,gameObject.transform) ;
            poolQueue.Enqueue(insPrefab);
            insPrefab.SetActive(false);
        }
        
    }
    private void createItem()
    {
        GameObject insPrefab = Instantiate(_prefab, gameObject.transform) ;
        poolQueue.Enqueue(insPrefab);
        insPrefab.SetActive(false);
    }
    public void ReUse(Vector3 position, Quaternion rotation)
    {
        if (poolQueue.Count > 0)
        {
            GameObject reuse = poolQueue.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        /*
        else
        {
            GameObject reuse = Instantiate(_prefab ,gameObject.transform) as GameObject;
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
        }*/
    }
    public void Recovery(GameObject _recovery)
    {
        poolQueue.Enqueue(_recovery);
        _recovery.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(gameObject.transform.childCount);
        if (gameObject.transform.childCount<initalSize)
        {

            createItem();
        }
    }
}
