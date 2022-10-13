using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;
using Unity.XR.PXR;

namespace HelloPico2.PlayerController.Arm
{
    
    public class ArmConVisualEvents : MonoBehaviour
    {
        public string _PrimaryAxisXName = "xAxis";
        public string _PrimaryAxisYName = "yAxis";
        public bool _InvertX = false;
        public XRController _controller;
        public UltEvents.UltEvent WhenTriggerTouch;
        public UltEvents.UltEvent WhenTriggerNotTouch;
        public UltEvents.UltEvent WhenGripTouch;
        public UltEvents.UltEvent WhenGripNotTouch;
        public UltEvents.UltEvent WhenJoyStickTouch;
        public UltEvents.UltEvent WhenJoyStickNotTouch;
        public UltEvents.UltEvent WhenPrimaryButtonTouch;
        public UltEvents.UltEvent WhenPrimaryButtonNotTouch;
        public UltEvents.UltEvent WhenSecondaryButtonTouch;
        public UltEvents.UltEvent WhenSecondaryButtonNotTouch;
        public UltEvents.UltEvent WhenPrimaryButtonDown;
        public UltEvents.UltEvent WhenPrimaryButtonUp;
        public UltEvents.UltEvent WhenSecondaryButtonDown;
        public UltEvents.UltEvent WhenSecondaryButtonUp;   

        private void Update()
        {
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggetValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouchValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchValue);
            _controller.inputDevice.TryGetFeatureValue(PXR_Usages.grip1DAxis, out var grip1DAxis);

            if (TryGetComponent<AnimatorValueChanger>(out var valueChanger))
            {
                int invert = (_InvertX) ? -1 : 1;
                valueChanger.animator.SetFloat(_PrimaryAxisXName, primary2DAxisValue.x * invert);
                valueChanger.animator.SetFloat(_PrimaryAxisYName, primary2DAxisValue.y);
            }

            if (isTrigger)
                WhenTriggerTouch?.Invoke();
            else
                WhenTriggerNotTouch?.Invoke();

            if(isGrip)
                WhenGripTouch?.Invoke();
            else
                WhenGripNotTouch?.Invoke();

            if (padAxisTouch)
                WhenJoyStickTouch?.Invoke();
            else
                WhenJoyStickNotTouch?.Invoke();

            //if(primaryTouchValue)
            //    WhenPrimaryButtonTouch?.Invoke();
            //else
            //    WhenPrimaryButtonNotTouch?.Invoke();    

            //if(secondaryTouchValue)
            //    WhenSecondaryButtonTouch?.Invoke(); 
            //else
            //    WhenSecondaryButtonNotTouch?.Invoke();

            if (primaryButtonValue)
                WhenPrimaryButtonDown?.Invoke();
            else
                WhenPrimaryButtonUp?.Invoke();

            if (secondaryButtonValue)
                WhenSecondaryButtonDown?.Invoke();
            else
                WhenSecondaryButtonUp?.Invoke();

            if (primaryTouchValue && !secondaryTouchValue)
                WhenPrimaryButtonTouch?.Invoke();
            else if (!primaryTouchValue)
                WhenPrimaryButtonNotTouch?.Invoke();

            if (secondaryTouchValue)
                { WhenSecondaryButtonTouch?.Invoke(); WhenPrimaryButtonNotTouch?.Invoke(); }
            if (!secondaryTouchValue)
                WhenSecondaryButtonNotTouch?.Invoke();

        }
    }
}
