using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SphereMotion : MonoBehaviour
{

    public float SphereRadius=5;

    public float speed=10;

    Vector3 Dir;

    void OnEnable()
    {
        Dir = Random.onUnitSphere;
        StartCoroutine(LUpdate());
    }

    void Update()
    {
        if(Vector3.Magnitude(transform.position) > SphereRadius)
        {
            Dir = Vector3.Normalize(ReflectDir(Vector3.Normalize(Dir), Vector3.Normalize(-transform.position)));

            transform.position = Vector3.Normalize(transform.position) * (Vector3.Magnitude(transform.position) - 0.15f);
            transform.Translate(Dir * speed * SphereRadius * Time.deltaTime);

            speed = 0.15f;
        }

        if(speed<7.5f)
        {
            speed += 0.65f;
        }
        else
        {
            speed = 7.5f;
        }

        transform.Translate(Dir * speed * SphereRadius * Time.deltaTime);
    }

    //¤Ï®g¨¤¤è¦V
    Vector3 ReflectDir(Vector3 v, Vector3 n)
    {
        return v - 2 * Vector3.Dot(v, n) * n;
    }


    IEnumerator LUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.35f);
            Dir = Random.onUnitSphere;
        }
    }
}
