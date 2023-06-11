using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObject : MonoBehaviour
{
    public Vector3 pos = new Vector3(0, 0, 0.1f);

   GameObject go;

    static public Vector3 HandZDir;

    void OnEnable()
    {
        go = GameObject.Find("RightHand Controller");
    }


    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, go.transform.position + transform.worldToLocalMatrix.MultiplyVector(go.transform.forward) * 0.0035f, 0.95f);
        
        currentDir = go.transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        
        HandZDir = transform.worldToLocalMatrix.MultiplyVector(go.transform.forward)* Vector3.Magnitude(deltaDir);
        
        /*
        if (Vector3.Magnitude(deltaDir) > 0.0001F)
        {
            HandZDir = transform.worldToLocalMatrix.MultiplyVector(go.transform.forward);
        }
        if (Vector3.Magnitude(deltaDir) <= 0.0001F)
        {
            HandZDir = JointMaxDistance.deltaDir;
        }*/
    }
}
