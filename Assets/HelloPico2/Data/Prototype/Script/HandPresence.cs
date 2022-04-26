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
    
    private GameObject spawnedController;
    private GameObject spawnedHandModel;

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
        
        handAnimator = handAnimator.GetComponent<Animator>();
    }

    void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger,out float triggetValue))
        {
            Debug.Log(triggetValue);
        }
    }
    // Update is called once per frame
    void Update()
    {


        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
            Debug.Log("Primary");
        /*
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
            Debug.Log("secondaryButtonValue");
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue>0.1f)
            Debug.Log("triggerValue");
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue>0.1f)
            Debug.Log("gripValue");
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primmary2DAxisValue) && primmary2DAxisValue != Vector2.zero)
            Debug.Log("primary2DAxis");
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 secondary2DAxisValue) && secondary2DAxisValue != Vector2.zero)
            Debug.Log("secondary2DAxisValue");
        */
    }
}
