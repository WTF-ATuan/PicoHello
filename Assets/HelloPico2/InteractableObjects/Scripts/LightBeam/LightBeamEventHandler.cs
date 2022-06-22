using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamEventHandler : MonoBehaviour{
		private LightBeamRigController _rigController;
		[SerializeField] public float speedLimit;
		[SerializeField] public float returnDuring;
		[SerializeField] private bool activeUseSpeed;


		private float _timer;

		private void Start(){
			_rigController = GetComponent<LightBeamRigController>();
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(_rigController.gameObject);
			if(!isSameObject) return;
			var touchPadAxis = obj.TouchPadAxis;
			var triggerValue = obj.TriggerValue;
			ModifyBlendWeight(triggerValue);
			if(activeUseSpeed){
				var selectorSpeed = obj.Selector.Speed;
				ActiveLightBeam(selectorSpeed);
			}
			else{
				if(touchPadAxis.y < 0) return;
				ActiveLightBeam(touchPadAxis);
			}
		}

		private void ActiveLightBeam(Vector2 touchPadAxis){
			var axisY = touchPadAxis.y;
			var curveSpeed = _rigController.GetLengthModifiedCurveSpeed();
			if(axisY > 0.5f){
				_rigController.ModifyControlRigLength(+curveSpeed);
			}
			else{
				_rigController.ModifyControlRigLength(-curveSpeed);
			}
		}

		private void ActiveLightBeam(float speedOfSelector){
			if(speedOfSelector > speedLimit){
				_rigController.SetRigTotalLength(_rigController.minMaxLenghtLimit.y);
				_timer = 0;
			}
			else{
				if(speedOfSelector > speedLimit / 2) return;
				_timer += Time.fixedDeltaTime;
				if(_timer > returnDuring){
					_rigController.SetRigTotalLength(_rigController.minMaxLenghtLimit.x);
				}
			}
		}

		private void ModifyBlendWeight(float value){
			if(value > 0.9f){
				_rigController.ModifyBlendWeight(-1);
			}

			if(value <= 0.1f){
				_rigController.ModifyBlendWeight(1);
			}
		}
	}
}