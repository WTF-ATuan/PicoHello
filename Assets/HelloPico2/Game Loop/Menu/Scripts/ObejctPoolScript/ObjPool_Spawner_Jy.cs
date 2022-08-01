using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool_Spawner_Jy : MonoBehaviour
{
    public ObjectPool_Jy pool;
    public float spawnTime = 1f;
    private float _timer;
    public bool isOne;
    // Update is called once per frame

    private void Start()
    {
        SpawnerFunc();
    }
    void Update()
    {
        if (isOne) return;
        
        if (Time.time > _timer)
        {
            SpawnerFunc();
            _timer = Time.time + spawnTime;            
        }
    }

    private void SpawnerFunc()
    {
        pool.ReUse(transform.position, transform.rotation);
    }

}
