using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Performance_Test : MonoBehaviour
{

    public GameObject subGameObject1;
    // Use this for initialization
    void Start()
    {
        Stopwatch obj = new Stopwatch();


        obj.Reset();
        obj.Start();
        for (int i = 0; i < 5000000; ++i)
        {
            subGameObject1.transform.parent = transform;
        }
        obj.Stop();
        UnityEngine.Debug.Log("transform.parent¡G" + obj.ElapsedMilliseconds);

        obj.Reset();
        obj.Start();
        for (int i = 0; i < 5000000; ++i)
        {
            subGameObject1.transform.SetParent(transform);
        }
        obj.Stop();
        UnityEngine.Debug.Log("SetParent¡G" + obj.ElapsedMilliseconds);



        obj.Reset();
        obj.Start();
        for (int i = 0; i < 5000000; ++i)
        {
            subGameObject1.transform.parent = null;
        }
        obj.Stop();
        UnityEngine.Debug.Log("transform.parent null¡G" + obj.ElapsedMilliseconds);

        obj.Reset();
        obj.Start();
        for (int i = 0; i < 5000000; ++i)
        {
            subGameObject1.transform.SetParent(null);
        }
        obj.Stop();
        UnityEngine.Debug.Log("SetParent null¡G" + obj.ElapsedMilliseconds);

    }

    void Update()
    {

    }
}
