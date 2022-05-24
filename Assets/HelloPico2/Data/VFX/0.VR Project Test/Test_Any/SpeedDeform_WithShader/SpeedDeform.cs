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

        //速度: 計算當前Position變化量deltaPos
        currentPos = transform.position;
        deltaPos = currentPos - lastPos;
        lastPos = currentPos;

        //正規化速度
        deltaPos = Vector3.ClampMagnitude(deltaPos, 0.75f);

        //_m.SetVector("_SpeedDir", deltaPos);
    }
}
