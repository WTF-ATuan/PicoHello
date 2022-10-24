using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FarUp : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {

        Vector2 CameraAngle = new Vector2(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y);

        float CameraDot = Vector3.Dot(Vector3.Normalize(Camera.main.transform.TransformDirection(Vector3.forward)), Vector3.forward);

        CameraDot = (Mathf.Clamp(CameraDot * 2, 0, 1) + Mathf.Clamp((1 - CameraDot) * 2, 0, 1))-1;

        Quaternion angle = Quaternion.Euler(-CameraAngle.x, -CameraAngle.y, 0);

        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), angle, CameraDot/3);

    }
}
