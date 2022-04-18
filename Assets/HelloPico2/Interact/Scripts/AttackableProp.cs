using Game.Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interactable.Scripts{
	[RequireComponent(typeof(XRGrabInteractable))]
	public class AttackableProp : MonoBehaviour{
		private XRGrabInteractable _grabInteractable;
		private ColdDownTimer _timer;

		private IChargeable _chargeable;

		private void Start(){
			_grabInteractable = GetComponent<XRGrabInteractable>();
			_chargeable = GetComponent<IChargeable>();
			_timer = new ColdDownTimer(_chargeable.chargeTime);
			RegisterListener();
		}

		private void RegisterListener(){
			_grabInteractable.selectEntered.AddListener(OnSelectEntered);
			_grabInteractable.selectExited.AddListener(OnSelectExited);
		}

		private void OnSelectEntered(SelectEnterEventArgs arg0){
			_timer.Reset();
		}

		private void OnSelectExited(SelectExitEventArgs arg0){
			if(_timer.CanInvoke()){
				_chargeable.Activate(_grabInteractable);
			}
		}
	}
}