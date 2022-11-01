using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.Arm
{
    public class InteractCollider : MonoBehaviour, ICollideFeedback
    {
        public UltEvents.UltEvent WhenNormalCollide;
        public UltEvents.UltEvent WhenCriticalCollide;
        public void NormalCollide()
        {
            print("Receive Normal");
            WhenNormalCollide?.Invoke();
        }

        public void CriticalCollide()
        {
            print("Receive Critical");
            WhenCriticalCollide?.Invoke();            
        }

    }
}
