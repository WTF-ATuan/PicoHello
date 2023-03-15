using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;
using Unity.XR.PXR;

namespace HelloPico2.PlayerController.Arm{
	public class ArmConVisualEvents : MonoBehaviour{
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
		public UltEvents.UltEvent WhenJoyStickStay;
		public UltEvents.UltEvent WhenJoyStickMove;
		public UltEvents.UltEvent WhenJoyStickForward;
		public UltEvents.UltEvent WhenJoyStickBackward;
		public UltEvents.UltEvent WhenJoyStickIdle;
		public UltEvents.UltEvent WhenPrimaryButtonTouch;
		public UltEvents.UltEvent WhenPrimaryButtonNotTouch;
		public UltEvents.UltEvent WhenSecondaryButtonTouch;
		public UltEvents.UltEvent WhenSecondaryButtonNotTouch;
		public UltEvents.UltEvent WhenPrimaryButtonDown;
		public UltEvents.UltEvent WhenPrimaryButtonUp;
		public UltEvents.UltEvent WhenSecondaryButtonDown;
		public UltEvents.UltEvent WhenSecondaryButtonUp;

		private Vector2 primary2DAxisValueCurrent;

		public float DeadValue = 0.3f;
		private int invert;
		private bool _isTrigger;
		private bool _isGrip;
		private bool _padAxisTouch;
		private bool _primaryButtonValue;
		private bool _secondaryButtonValue;
		private bool _primaryTouchValue;
		private bool _secondaryTouchValue;
		private Vector2 _primary2DAxisValue;
		
		private AnimatorValueChanger _animatorValueChanger;

		private void Start(){
			_animatorValueChanger = GetComponent<AnimatorValueChanger>();
		}

		private void Update(){
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out _isTrigger);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out _isGrip);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out _primary2DAxisValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out _secondaryButtonValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out _secondaryTouchValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out _primaryButtonValue);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out _primaryTouchValue);

			if(_animatorValueChanger){
				invert = (_InvertX) ? -1 : 1;
				_animatorValueChanger.animator.SetFloat(_PrimaryAxisXName, _primary2DAxisValue.x * invert);
				_animatorValueChanger.animator.SetFloat(_PrimaryAxisYName, _primary2DAxisValue.y);
			}

			if(_isTrigger)
				WhenTriggerTouch?.Invoke();
			else
				WhenTriggerNotTouch?.Invoke();

			if(_isGrip)
				WhenGripTouch?.Invoke();
			else
				WhenGripNotTouch?.Invoke();

			if(_padAxisTouch)
				WhenJoyStickTouch?.Invoke();
			else
				WhenJoyStickNotTouch?.Invoke();

			if(_primaryButtonValue)
				WhenPrimaryButtonDown?.Invoke();
			else
				WhenPrimaryButtonUp?.Invoke();

			if(_secondaryButtonValue)
				WhenSecondaryButtonDown?.Invoke();
			else
				WhenSecondaryButtonUp?.Invoke();

			if(_primaryTouchValue && !_secondaryTouchValue)
				WhenPrimaryButtonTouch?.Invoke();
			else if(!_primaryTouchValue)
				WhenPrimaryButtonNotTouch?.Invoke();

			if(_secondaryTouchValue){
				WhenSecondaryButtonTouch?.Invoke();
				WhenPrimaryButtonNotTouch?.Invoke();
			}

			if(!_secondaryTouchValue)
				WhenSecondaryButtonNotTouch?.Invoke();

			if(Mathf.Abs(_primary2DAxisValue.x) >= 0.1f || Mathf.Abs(_primary2DAxisValue.y) >= 0.1f){
				WhenJoyStickMove?.Invoke();
			}

			if(Mathf.Abs(_primary2DAxisValue.x) <= 0.1f && Mathf.Abs(_primary2DAxisValue.y) <= 0.1f){
				WhenJoyStickStay?.Invoke();
			}

			if(_primary2DAxisValue.y > DeadValue){
				WhenJoyStickForward?.Invoke();
			}

			if(Mathf.Abs(_primary2DAxisValue.y) <= DeadValue && _padAxisTouch){
				WhenJoyStickIdle?.Invoke();
			}

			if(_primary2DAxisValue.y < -DeadValue){
				WhenJoyStickBackward?.Invoke();
			}
		}
	}
}