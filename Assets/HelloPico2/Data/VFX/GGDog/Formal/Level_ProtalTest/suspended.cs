using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class suspended : MonoBehaviour
{
    public float Random_interval = 0.3f;

    void Start()
    {
        ReSetPos();
    }

    Vector3 Sphere_Random ;

    float _timer = 0;

    Vector3 Ori_Pos;
    Quaternion Ori_Rotation;
    void Update()
    {

        if (Time.time > _timer + Random.Range(Random_interval, Random_interval + 0.1f))
        {
            _timer = Time.time;
            Sphere_Random = Random.insideUnitSphere *2;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(Ori_Pos.x, Ori_Pos.y,transform.position.z) + Sphere_Random, 0.005f);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Ori_Rotation.eulerAngles + Sphere_Random*2), 0.005f);

    }

    public void ReSetPos()
    {
        Ori_Pos = transform.position;
        Ori_Rotation = transform.rotation;
    }
}
