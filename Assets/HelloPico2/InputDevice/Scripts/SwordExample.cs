using System;
using Project;
using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class SwordExample : MonoBehaviour{
		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(gameObject);
			if(isSameObject) return;
			var isGrip = obj.IsGrip;
			var isPrimary = obj.IsPrimary;
			var padAxis = obj.TouchPadAxis;
			if(isGrip){
				OnSelect();
			}

			if(isPrimary){
				OnXOrAButton();
			}

			if(padAxis.magnitude > 0.1f){
				OnTouchPad(padAxis);
			}
		}

		private void OnSelect(){ }

		private void OnXOrAButton(){ }

		private void OnTouchPad(Vector2 axis){
			var axisX = axis.x;
			var axisY = axis.y;
			if(axisX > 0.1f){
				//變長
			}
			else{
				//變短
			}

			if(axisY > 0.1f){
				//變硬
			}
			else{
				//變軟
			}
		}
	}
}