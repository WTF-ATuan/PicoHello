using Game.Project;
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

		private XRGrabInteractable grabInteractable;
		private new Rigidbody rigidbody;
		private ColdDownTimer timer;

		private void Start(){
			rigidbody = GetComponent<Rigidbody>();
			grabInteractable = GetComponent<XRGrabInteractable>();
			timer = new ColdDownTimer(chargeTime);
			grabInteractable.selectEntered.AddListener(OnSelectEntered);
			grabInteractable.selectExited.AddListener(OnSelectExited);
		}

		private void OnSelectEntered(SelectEnterEventArgs obj){
			timer.Reset();
		}

		private void OnSelectExited(SelectExitEventArgs obj){
			if(timer.CanInvoke()){
				grabInteractable.throwOnDetach = true;
			}
			else{
				grabInteractable.throwOnDetach = false;
			}
		}
	}
}