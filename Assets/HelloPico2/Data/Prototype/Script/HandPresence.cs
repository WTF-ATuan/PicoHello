using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    //public List<GameObject> controllerPrefabs;
    private InputDevice targetDevice;

    public Animator handAnimator;


    public GameObject spawnedHandModel;
    public GameObject spawnedController;
    
    public bool showController = false;
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        /*
        foreach(var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }*/
        if(devices.Count > 0)
        {
            targetDevice = devices[0];
        }
        handAnimator = handAnimator.GetComponent<Animator>();

    }

    void UpdateHandAnimation()
    {
        if (!showController) return;
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger,out float triggetValue))
        {
            handAnimator.SetFloat("Trigger", triggetValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue))
        {
            handAnimator.SetBool("PrimaryBtn", primaryButtonValue);
        }
        else
        {
            handAnimator.SetBool("PrimaryBtn", false);
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue))
        {
            handAnimator.SetBool("SecondBtn", secondaryButtonValue);
        }
        else
        {
            handAnimator.SetBool("SecondBtn", false);
        }
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue))
        {
            handAnimator.SetFloat("yAxis", primary2DAxisValue.y);
            handAnimator.SetFloat("xAxis", primary2DAxisValue.x);
        }
        else
        {
            handAnimator.SetFloat("yAxis", 0);
            handAnimator.SetFloat("xAxis", 0);
        }
        

    }
    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
        if (showController)
        {
            spawnedHandModel.SetActive(false);
            spawnedController.SetActive(true);
        }
        else
        {
            spawnedHandModel.SetActive(true);
            spawnedController.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "handTargetType")
        {
            showController = true;
        }

    }
}
