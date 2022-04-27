using System.Collections.Generic;
using System.Linq;
using HelloPico2.Hand.Scripts.Event;
using Project;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InputDeviceTrigger : MonoBehaviour{
		private XRController _controller;

		private UnityEngine.XR.InputDevice _inputDevice;

		private IGrip _grip;
		private ITrigger _trigger;
		private ITouchPad _touchPad;
		private IPrimaryButton _primaryButton;
		private ISecondaryButton _secondaryButton;

		private void Start(){
			_controller = GetComponent<XRController>();
			_inputDevice = _controller.inputDevice;
			EventBus.Subscribe<HandSelected>(OnHandSelected);
		}

		//Demo First refactor Todo
		public void OnHandSelected(HandSelected obj){
			var isEnter = obj.IsEnter;
			var selectedObject = obj.SelectedObject;
			var inputDevice = obj.inputDevice;
			var isEqual = inputDevice.Equals(_inputDevice);
			if(isEnter && isEqual){
				_grip = selectedObject.GetComponent<IGrip>();
				_trigger = selectedObject.GetComponent<ITrigger>();
				_touchPad = selectedObject.GetComponent<ITouchPad>();
				_primaryButton = selectedObject.GetComponent<IPrimaryButton>();
				_secondaryButton = selectedObject.GetComponent<ISecondaryButton>();
			}
			else{
				_grip = null;
				_trigger = null;
				_touchPad = null;
				_primaryButton = null;
				_secondaryButton = null;
			}
		}

		public void Update(){
			_inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			_inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButtonValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var secondaryButtonValue);
            
			if (gripValue > 0.1f){
				_grip?.OnGrip(gripValue);
			}

			if(triggerValue > 0.1f){
				_trigger?.OnTrigger(triggerValue);
			}

			if(touchPadAxis.magnitude > 0.1f){
				_touchPad?.OnTouchPad(touchPadAxis);
			}

			if(primaryButtonValue){
				_primaryButton?.OnPrimaryButtonClick();
			}

			if(secondaryButtonValue){
				_secondaryButton?.OnSecondaryButtonClick();
			}
		}
	}
}