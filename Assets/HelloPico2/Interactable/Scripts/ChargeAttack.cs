using Game.Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interactable.Scripts{
	//被 select 的掌控時間 
	//移動速度 + 方位 
	public class ChargeAttack : MonoBehaviour{
		[SerializeField] private int additionForce = 1;
		[SerializeField] private float chargeTime = 2f;


		private XRGrabInteractable _grabInteractable;
		private ColdDownTimer _timer;

		private void Start(){
			_grabInteractable = GetComponent<XRGrabInteractable>();
			_timer = new ColdDownTimer(chargeTime);
			_grabInteractable.selectEntered.AddListener(OnSelectEntered);
			_grabInteractable.selectExited.AddListener(OnSelectExited);
			_grabInteractable.throwVelocityScale *= additionForce;
		}

		private void OnSelectEntered(SelectEnterEventArgs obj){
			_timer.Reset();
		}

		private void OnSelectExited(SelectExitEventArgs obj){
			_grabInteractable.throwOnDetach = _timer.CanInvoke();
		}
	}
}