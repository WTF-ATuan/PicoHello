using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool_Spawner_Jy : MonoBehaviour
{
    public ObjectPool_Jy pool;
    public float spawnTime = 1f;
    private float _timer;
    
    // Update is called once per frame


    void Update()
    {
       
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
