using System;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamEventHandler : MonoBehaviour{
		private LightBeamRigController _rigController;

		public bool percentMode;

		private void Start(){
			_rigController = GetComponent<LightBeamRigController>();
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(_rigController.gameObject);
			if(!isSameObject) return;
			var touchPadAxis = obj.TouchPadAxis;
			OnTouchPadAxis(touchPadAxis);
		}

		private void OnTouchPadAxis(Vector2 touchPadAxis){
			var axisY = touchPadAxis.y;
			if(!percentMode) _rigController.ModifyPositionLenght(axisY * 0.01f);
			else _rigController.SetPositionLenghtByPercent(1, axisY);
		}
	}
}