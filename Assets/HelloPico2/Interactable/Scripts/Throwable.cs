using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interactable.Scripts{
	//被 select 的掌控時間 
	//移動速度 + 方位 
	public class Throwable : MonoBehaviour{
		[SerializeField] private int additionForce = 1;
		[SerializeField] private float chargeTime = 2f;


		public Vector3 Velocity => rigidbody.velocity;
		public float Speed => rigidbody.velocity.magnitude;

		private XRBaseInteractable interactableComponent;
		private new Rigidbody rigidbody;

		private void Start(){
			rigidbody = GetComponent<Rigidbody>();
			interactableComponent = GetComponent<XRBaseInteractable>();
			interactableComponent.selectEntered.AddListener(OnSelectEntered);
			interactableComponent.selectExited.AddListener(OnSelectExited);
		}

		private void OnSelectExited(SelectExitEventArgs obj){
			
		}

		private void OnSelectEntered(SelectEnterEventArgs obj){ }
	}
}