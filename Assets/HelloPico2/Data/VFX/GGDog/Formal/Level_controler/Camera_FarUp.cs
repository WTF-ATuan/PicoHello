using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FarUp : MonoBehaviour
{
    public float Range_X=3.5F;
    public float Range_Y=1;
    [Range(0,1)]
    public float Lerp = 0.1F;
    void Update()
    {

        // Vector2 CameraAngle = new Vector2(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y);

        float CameraDot = Vector3.Dot(Vector3.Normalize(Camera.main.transform.TransformDirection(Vector3.forward)), Vector3.forward);

        Vector2 Camera_Forward = Camera.main.transform.TransformDirection(Vector3.forward);

        if(Camera_Forward.y<0)
        {
            Camera_Forward.y = 0;
        }

        Vector3 Camera_Move = Vector3.Lerp(transform.position, new Vector3(-Camera_Forward.x* Range_X, -Camera_Forward.y * Range_Y, 0) ,0.5f*(1- CameraDot/2));


        transform.position = Vector3.Lerp(transform.position, Camera_Move, Lerp);

    }


}


