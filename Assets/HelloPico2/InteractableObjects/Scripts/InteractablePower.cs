using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.PlayerController.Arm;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.InteractableObjects
{
    public class InteractablePower : InteractableBase
    {
        [SerializeField] private float _Energy = 20;
             
        bool used { get; set; }
        public override void OnSelect(DeviceInputDetected obj)
        {
            base.OnSelect(obj);
            if (used) return;

            // Charge Energy            
            TryGetComponent<IXRSelectInteractable>(out var Interactable);

            GainEnergyEventData eventDate = new GainEnergyEventData();
            eventDate.Energy = _Energy;
            eventDate.Interactable = Interactable;
            eventDate.InputReceiver = obj;
            Project.EventBus.Post(eventDate);

            GetComponent<Collider>().enabled = false;
            used = true;
        }
        private void OnDisable()
        {
            
        }
    }
}
