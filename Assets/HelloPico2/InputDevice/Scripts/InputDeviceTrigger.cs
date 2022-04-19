using HelloPico2.Hand.Scripts.Event;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InputDeviceTrigger : MonoBehaviour{
		private XRController _xrController;
		private UnityEngine.XR.InputDevice _inputDevice;

		private IGrip _grip;
		private ITrigger _trigger;
		private ITouchPad _touchPad;
		private IPrimaryButton _primaryButton;
		private ISecondaryButton _secondaryButton;

		private void Start(){
			_xrController = GetComponent<XRController>();
			_inputDevice = _xrController.inputDevice;

			_grip = GetComponent<IGrip>();
			_trigger = GetComponent<ITrigger>();
			_touchPad = GetComponent<ITouchPad>();
			_primaryButton = GetComponent<IPrimaryButton>();
			_secondaryButton = GetComponent<ISecondaryButton>();
		}

		private void Update(){
			_inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			_inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButtonValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var secondaryButtonValue);


			if(gripValue > 0.1f){
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