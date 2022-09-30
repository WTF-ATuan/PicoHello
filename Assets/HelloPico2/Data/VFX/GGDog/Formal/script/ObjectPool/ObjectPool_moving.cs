using UnityEngine;
using System.Collections;

public class ObjectPool_moving : MonoBehaviour
{
    [HideInInspector] public ObjectPool _ObjectPool;
    [HideInInspector] public ObjectPool_Spawner_Range ObjectPool_Spawner_Range;
    [HideInInspector] public ObjectPool_Spawner ObjectPool_Spawner;
    

    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (transform.position.z < -ObjectPool_Spawner_Range.Range_Z)
        {
            _ObjectPool.Destroy(gameObject);
        }

        transform.position += new Vector3(0, 0, -10)* ObjectPool_Spawner.SeedSpeed * Time.deltaTime;
    }
}