using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;


public class seethrough : MonoBehaviour
{
    public void Start()
    {
        PXR_Boundary.EnableSeeThroughManual(false);
    }

    public void EnableSeeThroughManual()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }

    public void OffSeeThroughManual()
    {
        PXR_Boundary.EnableSeeThroughManual(false);
    }
}
