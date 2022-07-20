using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;


public class seethrough : MonoBehaviour
{

    public void EnableSeeThroughManual()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }
}
