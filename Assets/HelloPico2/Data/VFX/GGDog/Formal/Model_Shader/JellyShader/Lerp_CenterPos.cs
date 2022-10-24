using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp_CenterPos : MonoBehaviour
{
   [Range(0,1)]
    public float Lerp=0.1f;
    Material _m;
    private void OnEnable()
    {
        _m = GetComponent<MeshRenderer>().material;
        _m.SetVector("Jelly_RandomUVOffSet", new Vector2(Random.Range(0, 300), Random.Range(0, 300)));
        _m.SetVector("Jelly_Size", transform.lossyScale);
    }

    Vector3 ChasePos;

    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    void Update()
    {
        currentDir = transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        float Speed = Vector3.Magnitude(deltaDir);

        ChasePos = Vector3.Lerp(ChasePos, transform.position, Lerp);

        _m.SetVector("Jelly_LerpPos", ChasePos);

        
    }
}
