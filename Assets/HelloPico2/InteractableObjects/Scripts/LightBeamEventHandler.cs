﻿using System;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
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
			var axisX = touchPadAxis.x;
			if(!percentMode) _rigController.ModifyControlRigLenght(axisY * 0.1f);
			else _rigController.SetPositionLenghtByPercent(10, axisY);
			_rigController.ModifyInert(axisX * 0.1f);
		}
	}
}