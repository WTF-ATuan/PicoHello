using DG.Tweening;
using HelloPico2.InputDevice.Scripts;
using Project;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamEventHandler : MonoBehaviour{
		private LightBeamRigController _rigController;
		[SerializeField] private float speedLimit;
		[SerializeField] private float returnDuring;
		[SerializeField] private bool blendWeightWithSpeed;
		


		private void Start(){
			_rigController = GetComponent<LightBeamRigController>();
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(_rigController.gameObject);
			if(!isSameObject) return;
			if(blendWeightWithSpeed){
				SetBlendWeight(obj.Selector.Speed);
			}
			var touchPadAxis = obj.TouchPadAxis;
			if(touchPadAxis.y < 0) return;
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
			if(axisY > 0.5f){
				_rigController.ModifyControlRigLength(+0.1f);
			}
			else{
				_rigController.ModifyControlRigLength(-0.1f);
			}
		}
	}
}