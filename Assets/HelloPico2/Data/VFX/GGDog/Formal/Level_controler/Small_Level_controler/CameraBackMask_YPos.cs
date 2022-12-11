using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackMask_YPos : MonoBehaviour
{
    Vector3 Ori_Pos;

    void Start()
    {
        Ori_Pos = transform.position;
    }

    void Update()
    {
        transform.position = Ori_Pos + Camera.main.transform.position;
    }
}
