using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Smear_Dir : MonoBehaviour
{

    public float SmearTrai = 1;

    Material _m;

    void Awake()
    {
        _m = GetComponent<MeshRenderer>().sharedMaterial;
        StartCoroutine(LUpdate());
    }


    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);

    Vector3 Delay_deltaDir;

    void Update()
    {
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        currentDir = transform.position;

        Delay_deltaDir = Vector3.Lerp(Delay_deltaDir, deltaDir, 0.95f)* SmearTrai;

        _m.SetVector("_Smear_Dir", -Vector3.Normalize(Delay_deltaDir)* Vector3.Magnitude(Delay_deltaDir));

    }



    IEnumerator LUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);


        }
    }

}
