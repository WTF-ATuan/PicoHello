using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_moving : MonoBehaviour
{

    Vector3 CameraForward;

    void OnEnable()
    {
        CameraForward = Camera.main.transform.forward;
    }

    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {
        transform.Translate( Vector3.forward*5 ) ;
        
        currentDir = transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        
        //從Up軸轉到指定方向
        Vector3 axis = Vector3.Cross(transform.rotation * Vector3.forward, CameraForward).normalized;
        float angle = Vector3.Angle(transform.rotation * Vector3.forward, CameraForward);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, axis) * transform.rotation, 0.75f);

        Destroy(gameObject, 2.5f);
    }
}



