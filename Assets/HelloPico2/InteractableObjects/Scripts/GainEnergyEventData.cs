using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.InteractableObjects {
    public class GainEnergyEventData
    {
        public float Energy;
        public IXRSelectInteractable Interactable;
        public DeviceInputDetected InputReceiver;
    } 
}
