using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;


public class seethrough : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
}
