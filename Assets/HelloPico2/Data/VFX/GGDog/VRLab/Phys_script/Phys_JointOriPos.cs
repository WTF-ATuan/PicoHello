using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phys_JointOriPos : MonoBehaviour
{

    public float lerp_value=0.025f;

    Vector3 OriPos;
    void OnEnable()
    {
        OriPos = transform.localPosition - transform.parent.GetChild(0).transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, OriPos + transform.parent.GetChild(0).transform.localPosition, lerp_value);

    }
}
