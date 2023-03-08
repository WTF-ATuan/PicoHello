using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using HelloPico2.InputDevice.Scripts;

public class ChangeTutorialMesh : MonoBehaviour
{
    public GameObject Phoenix;
    public GameObject Neo;
    // Start is called before the first frame update
    void Start()
    {
        AutoSetVibrateType();
    }

    private void AutoSetVibrateType()
    {
        var controllerDevice = PXR_Input.GetControllerDeviceType();
        //_vrType = controllerDevice == PXR_Input.ControllerDevice.PICO_4 ? VRType.Phoenix : VRType.Neo3;
        if (controllerDevice == PXR_Input.ControllerDevice.PICO_4)
        {
            Phoenix.SetActive(true);
        }
        else
        {
            Neo.SetActive(true);
        }
    }
}
