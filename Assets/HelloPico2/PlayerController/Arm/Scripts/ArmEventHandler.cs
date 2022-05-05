using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    public static class ArmEventHandler
    {
        public delegate void ValueEvent(float value, IXRSelectInteractable interactable, DeviceInputDetected obj);
        public static ValueEvent OnChargeEnergy;
    }
}