using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.InteractableObjects
{
    public class GainBombEventData
    {
        public int Amount;
        public IXRSelectInteractable Interactable;
        public DeviceInputDetected InputReceiver;
    }
}
