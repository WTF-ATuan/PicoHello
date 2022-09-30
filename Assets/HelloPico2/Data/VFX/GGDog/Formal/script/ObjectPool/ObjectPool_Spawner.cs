using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ObjectPool_Spawner_Range))]
[RequireComponent(typeof(ObjectPool))]


public class ObjectPool_Spawner : MonoBehaviour
{
    ObjectPool pool;

    public bool Emission = true;

    [Range(0,50)]
    public float SeedSpeed = 1;

    public Vector2 Random_spawnTime = new Vector2(1.5f,3f);  //Range(min,max)

    public Vector3 Random_rotation_min;
    public Vector3 Random_rotation_max;

    public Vector2 Random_Size = new Vector2(1,1);

    private float _timer;


    ObjectPool_Spawner_Range ObjectPool_Spawner_Range;
    private void Start()
    {
        ObjectPool_Spawner_Range = GetComponent<ObjectPool_Spawner_Range>();

        pool = GetComponent<ObjectPool>();
        pool.ObjectPool_Spawner = GetComponent<ObjectPool_Spawner>();
        pool.ObjectPool_Spawner_Range = ObjectPool_Spawner_Range;

        
        if (!Emission)
        {
            return;
        }
        Vector3 Pos = transform.position;
        pool.Instantiate(

            Pos +
            new Vector3(Random.Range(-ObjectPool_Spawner_Range.Range_X, ObjectPool_Spawner_Range.Range_X)
            , Random.Range(-ObjectPool_Spawner_Range.Range_Y, ObjectPool_Spawner_Range.Range_Y)
            , ObjectPool_Spawner_Range.Range_Z),

            Quaternion.Euler(
            Random.Range(Random_rotation_min.x, Random_rotation_max.x),
            Random.Range(Random_rotation_min.y, Random_rotation_max.y),
            Random.Range(Random_rotation_min.z, Random_rotation_max.z)),

            Random.Range(Random_Size.x, Random_Size.y)

            );
        
    }

    void Update()
    {
        if(!Emission)
        {
            return;
        }

        Vector3 Pos = transform.position;
        
        if (Time.time > _timer + Random.Range(Random_spawnTime.x, Random_spawnTime.y)/SeedSpeed)
        {
            _timer = Time.time;

            pool.Instantiate(

                Pos +
                new Vector3(Random.Range(-ObjectPool_Spawner_Range.Range_X, ObjectPool_Spawner_Range.Range_X)
                , Random.Range(-ObjectPool_Spawner_Range.Range_Y, ObjectPool_Spawner_Range.Range_Y)
                , ObjectPool_Spawner_Range.Range_Z),

                Quaternion.Euler(
                Random.Range(Random_rotation_min.x, Random_rotation_max.x),
                Random.Range(Random_rotation_min.y, Random_rotation_max.y),
                Random.Range(Random_rotation_min.z, Random_rotation_max.z)),

                Random.Range(Random_Size.x, Random_Size.y)

                );
        }
    }

}
