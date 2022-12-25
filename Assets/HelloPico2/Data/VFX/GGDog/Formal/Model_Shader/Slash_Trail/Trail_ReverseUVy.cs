using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail_ReverseUVy : MonoBehaviour
{

    public Material _mTrail;

    void OnEnable()
    {
    }

    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);
    void Update()
    {
        currentDir = transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;

        if (deltaDir.x > 0)
        {
            _mTrail.SetFloat("_ReversUVy", 0);
        }

        if (deltaDir.x < 0)
        {
            _mTrail.SetFloat("_ReversUVy", 1);
        }

    }
}
