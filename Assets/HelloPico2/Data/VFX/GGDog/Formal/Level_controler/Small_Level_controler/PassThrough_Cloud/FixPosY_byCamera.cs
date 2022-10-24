using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosY_byCamera : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }
}
