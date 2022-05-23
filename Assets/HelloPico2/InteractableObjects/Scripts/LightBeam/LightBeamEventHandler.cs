using DG.Tweening;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamEventHandler : MonoBehaviour{
		private LightBeamRigController _rigController;
		[SerializeField] private float speedLimit;
		[SerializeField] private float returnDuring;


		private void Start(){
			_rigController = GetComponent<LightBeamRigController>();
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(_rigController.gameObject);
			if(!isSameObject) return;
			SetBlendWeight(obj.Selector.Speed);
			var touchPadAxis = obj.TouchPadAxis;
			if(touchPadAxis.magnitude < 0.1f) return;
			OnTouchPadAxis(touchPadAxis);
		}

		private float _timer;

		private void SetBlendWeight(float speed){
			if(speed > speedLimit){
				_rigController.ModifyBlendWeight(0.01f);
				_timer = 0;
			}
			else{
				_timer += Time.fixedDeltaTime;
				if(_timer > returnDuring){
					_rigController.ModifyBlendWeight(-0.01f);
				}
			}
		}

		private void OnTouchPadAxis(Vector2 touchPadAxis){
			var axisY = touchPadAxis.y;
			if(Mathf.Abs(axisY) > 0.1f){
				_rigController.ModifyControlRigLenght(axisY * 0.1f);
			}
		}
	}
}