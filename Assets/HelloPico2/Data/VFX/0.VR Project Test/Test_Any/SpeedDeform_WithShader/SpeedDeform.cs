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
     have error 
     Instantiating material due to calling renderer.material during edit mode.
     This will leak materials into the scene. You most likely want to use renderer.sharedMaterial instead.
    
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
