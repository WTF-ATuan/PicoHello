using HelloPico2.Hand.Scripts.Event;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interact.Grip_Gun{
	public class GripGun : MonoBehaviour, ITrigger{
		private XRBaseInteractable _interactable;
		private PowerCarrier _powerCarrier;

		private bool _isSelect;

		private void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_powerCarrier = GetComponent<PowerCarrier>();

			_interactable.selectEntered.AddListener(x => { _isSelect = true; });
			_interactable.selectExited.AddListener(x => { _isSelect = false; });
		}

		public void OnTrigger(float triggerValue){
			if(!_isSelect || _powerCarrier.isEmpty) return;
			var powerAmount = _powerCarrier.currentPowerAmount;
			var bullet = Instantiate(gameObject, transform.position, Quaternion.identity, transform);
			bullet.transform.localScale /= powerAmount;
			Fire(bullet);
			transform.localScale -= bullet.transform.localScale;
			_powerCarrier.ModifyPower(-1);
		}

		private void Fire(GameObject bullet){
			var rigid = bullet.AddComponent<Rigidbody>();
			rigid.AddForce(rigid.transform.forward * 30f);
		}
	}
}