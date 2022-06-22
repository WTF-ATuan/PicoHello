using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Ball_Spawner_Range))]

public class Ball_Spawner : MonoBehaviour
{
    public ObjectPool pool;
    
    public Vector2 Random_spawnTime = new Vector2(1.5f,3f);  //Range(min,max)
    private float _timer;
    
    public float SeedSpeed=1;

    Ball_Spawner_Range Ball_Spawner_Range;
    private void Awake()
    {
        Ball_Spawner_Range = GetComponent<Ball_Spawner_Range>();
    }

    void Update()
    {
        Vector3 Pos = transform.localPosition;
        
        if (Time.time > _timer + Random.Range(Random_spawnTime.x, Random_spawnTime.y))
        {
            _timer = Time.time;

            pool.Instantiate( Pos + 
                new Vector3(Random.Range(-Ball_Spawner_Range.Range_X, Ball_Spawner_Range.Range_X) 
                , Random.Range(-Ball_Spawner_Range.Range_Y, Ball_Spawner_Range.Range_Y)
                , Ball_Spawner_Range.Range_Z), 
                transform.rotation);
        }
    }

}
