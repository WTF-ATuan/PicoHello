using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Trail_FixUV : MonoBehaviour {
    
    Material _Shader;

    Vector4 _MainTex_ST;
    
    Vector3 currentPos;
    Vector3 deltaPos;
    Vector3 lastPos = new Vector3();

    private void Awake()
    {
        _Shader = GetComponent<TrailRenderer>().sharedMaterial;
    }

    void Update ()
    {
        //速度: 計算當前Position變化量deltaPos
        currentPos = transform.position;
        deltaPos = currentPos - lastPos;
        lastPos = currentPos;
        //算出移動的總量
        float d = Mathf.Sqrt(deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y + deltaPos.z * deltaPos.z);

        if (d < 0.001f) { _Shader.SetFloat("_OffSetUV_x",0); }

        //以移動總量去逆向調整Shader的OffSet偏移
        _Shader.SetFloat("_OffSetUV_x", _Shader.GetFloat("_OffSetUV_x") + d * -_Shader.mainTextureScale.x);
    }
}
