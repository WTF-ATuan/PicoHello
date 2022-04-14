using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InputDeviceTrigger : MonoBehaviour{
		private XRController _xrController;
		private UnityEngine.XR.InputDevice _inputDevice;

		private void Start(){
			_xrController = GetComponent<XRController>();
			_inputDevice = _xrController.inputDevice;
		}

		private void Update(){
			_inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue);
			_inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			if(triggerValue > 0.1f){
				OnTrigger(triggerValue);
			}

			if(gripValue > 0.1f){
				OnGrip(gripValue);
			}

			if(touchPadAxis.magnitude > 0.1f){
				OnTouchPad(touchPadAxis);
			}
		}

		private void OnTrigger(float triggerValue){ }

		private void OnGrip(float gripVale){ }

		private void OnTouchPad(Vector2 axis){ }
	}
}