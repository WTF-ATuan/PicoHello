using Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class SwordExample : MonoBehaviour{
		private void Start(){
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}

		private void OnDeviceInputDetected(DeviceInputDetected obj){
			var isSameObject = obj.IsSameObject(gameObject);
			if(!isSameObject) return;
			var isGrip = obj.IsGrip;
			var isPrimary = obj.IsPrimary;
			var padAxis = obj.TouchPadAxis;
			var isSecondary = obj.IsSecondary;
			var selector = obj.Selector;
			if(isGrip){
				OnSelect();
			}
			else{
				OnDrop();
			}

			if(isPrimary){
				OnXOrAButton();
			}

			if(isSecondary){
				var selectInteractable = GetComponent<IXRSelectInteractable>();
				selector.CancelSelect(selectInteractable);
			}

			if(padAxis.magnitude > 0.1f){
				OnTouchPad(padAxis);
			}
		}

		private void OnDrop(){
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.color = Color.white;
		}

		private void OnSelect(){
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.color = Color.red;
		}

		private void OnXOrAButton(){ }

		private void OnTouchPad(Vector2 axis){
			var axisX = axis.x;
			var axisY = axis.y;
			if(axisX > 0.1f){
				transform.localScale += Vector3.up * 0.01f;
			}
			else{
				transform.localScale -= Vector3.up * 0.01f;
			}

			if(axisY > 0.1f){
				transform.localScale += Vector3.forward * 0.01f;
			}
			else{
				transform.localScale -= Vector3.forward * 0.01f;
			}
		}
	}
}