using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Z_Moving : MonoBehaviour
{
    public Vector3 Speed;

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime);
    }
}
