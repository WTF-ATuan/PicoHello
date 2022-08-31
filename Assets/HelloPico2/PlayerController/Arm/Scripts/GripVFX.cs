using HelloPico2.PlayerController.Arm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController {
    public class GripVFX : MonoBehaviour
    {
        public InputDevice.Scripts.HandType _hand;
        public UltEvents.UltEvent WhenGripUp;
        public UltEvents.UltEvent WhenGripDown;
        private Arm.ArmLogic armLogic;
        private bool up;

        private void OnEnable()
        {
            SetUpArm();
            armLogic.OnGripUp += GripUp;
            armLogic.OnGripDown += GripDown;
        }
        private void OnDisable()
        {
            armLogic.OnGripUp -= GripUp;
            armLogic.OnGripDown -= GripDown;
        }
        private void SetUpArm() {
            var objs = GameObject.FindObjectsOfType<Arm.ArmLogic>();

            foreach (var obj in objs)
            {
                if (obj.data.HandType == _hand)
                    armLogic = obj;
            }
        }
        private void GripUp(ArmData data)
        {
            if (!up)
                WhenGripUp?.Invoke();

            up = true;
        }

        private void GripDown(ArmData data)
        {
            if (up)
                WhenGripDown?.Invoke();

            up = false;
        }

    } 
}
