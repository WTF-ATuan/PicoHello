using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SpeedDeform : MonoBehaviour
{

    Material _m;

    Vector3 currentPos;
    Vector3 deltaPos;
    Vector3 lastPos = new Vector3();

    
    /*
    private void OnEnable()
    {
        _m = GetComponent<Renderer>().materials[0];
    }*/

    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,0);

        //�t��: �p���ePosition�ܤƶqdeltaPos
        currentPos = transform.position;
        deltaPos = currentPos - lastPos;
        lastPos = currentPos;

        //���W�Ƴt��
        deltaPos = Vector3.ClampMagnitude(deltaPos, 0.75f);

        //_m.SetVector("_SpeedDir", deltaPos);
    }
}
