using DG.Tweening;
using HelloPico2.InputDevice.Scripts;
using Project;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace HelloPico2.InteractableObjects{
	public class LightBeamEventHandler : MonoBehaviour{
		private LightBeamRigController _rigController;
		[SerializeField] private float speedLimit;
		[SerializeField] private float returnDuring;
		[SerializeField] private bool blendWeightWithSpeed;

		private SkinnedMeshRenderer _beamMesh;


		private void Start(){
			_rigController = GetComponent<LightBeamRigController>();
			_beamMesh = _rigController.GetComponentInChildren<SkinnedMeshRenderer>();
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
		[MaxValue(1)] [MinValue(0)] private float _colorValue;

		[Button]
		private void SetBlendWeight(float speed){
			if(speed > speedLimit){
				_rigController.ModifyBlendWeight(0.01f);
				_colorValue += 0.01f;
				var lerpColor = Color.Lerp(Color.blue, Color.red, _colorValue);
				_beamMesh.material.color = lerpColor;
				_timer = 0;
			}
			else{
				_timer += Time.fixedDeltaTime;
				if(_timer > returnDuring){
					_rigController.ModifyBlendWeight(-0.01f);
					_colorValue -= 0.01f;
					var lerpColor = Color.Lerp(Color.blue, Color.red, _colorValue);
					_beamMesh.material.color = lerpColor;
				}
			}
		}

		private void OnTouchPadAxis(Vector2 touchPadAxis){
			var axisY = touchPadAxis.y;
			var curveSpeed = _rigController.GetLengthModifiedCurveSpeed();
			if(axisY > 0.5f){
				_rigController.ModifyControlRigLength(+curveSpeed);
			}
			else{
				_rigController.ModifyControlRigLength(-curveSpeed);
			}
		}
	}
}