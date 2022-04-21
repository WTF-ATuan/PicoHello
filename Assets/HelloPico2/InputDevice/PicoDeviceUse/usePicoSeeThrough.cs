using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;


public class usePicoSeeThrough : MonoBehaviour
{
    void Start()
    {
        showSee();
    }

    public void showSee()
    {
        PXR_Boundary.EnableSeeThroughManual(true);
    }

}
