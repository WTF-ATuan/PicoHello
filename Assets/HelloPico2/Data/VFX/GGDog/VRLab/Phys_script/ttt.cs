using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttt : MonoBehaviour
{

    public GameObject followPos;  //±q°ÊÂI

    void Start()
    {
        
    }


    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        currentDir = transform.position;

        followPos.transform.localPosition -= deltaDir*Time.deltaTime;
    }
}
