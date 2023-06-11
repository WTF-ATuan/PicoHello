using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spwan_Object : MonoBehaviour
{


    public GameObject[] GO;

    public Vector2 Random_spawnTime = new Vector2(1.5f, 3f);  //Range(min,max)

    [Range(0,1)]
    public float Y_Scale = 0.65f;

    public float Z_max = 0.65f;

    public float Max_Distance = 50;

    private float _timer;

    void Update()
    {
        //間格隨機時間發射物件
        if (Time.time > _timer + Random.Range(Random_spawnTime.x, Random_spawnTime.y))
        {
            _timer = Time.time;

            //隨機發射不同種的物件
            int r = Random.Range(0, GO.Length);

            float GO_z = Random.onUnitSphere.z;
            if(GO_z < Z_max) { GO_z = Z_max; }

            Instantiate(

                GO[r],

                Max_Distance * new Vector3(Random.onUnitSphere.x, Random.onUnitSphere.y * Y_Scale, GO_z) , 
                
                transform.rotation
                
                );
        }
    }
}
