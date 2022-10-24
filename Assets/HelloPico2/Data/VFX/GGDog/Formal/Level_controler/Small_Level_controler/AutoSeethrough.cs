using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class AutoSeethrough : MonoBehaviour
{
    void Start()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }

}
