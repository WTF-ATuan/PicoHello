using Game.Project;
using HelloPico2.Hand.Scripts.Event;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interact.Grip_Gun{
	public class GripGun : MonoBehaviour, ITrigger{
		private XRBaseInteractable _interactable;
		private PowerCarrier _powerCarrier;
		[SerializeField] private GameObject bulletPrefab;

		private ColdDownTimer _timer;


		private bool _isSelect;

		private void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_powerCarrier = GetComponent<PowerCarrier>();

			_interactable.selectEntered.AddListener(x => { _isSelect = true; });
			_interactable.selectExited.AddListener(x => { _isSelect = false; });

			_timer = new ColdDownTimer(2f);
		}

		public void OnTrigger(float triggerValue){
			if(!_timer.CanInvoke()) return;
			if(!_isSelect || _powerCarrier.isEmpty){
				var meshRenderer = GetComponentInChildren<MeshRenderer>();
				meshRenderer.material.color = Color.black;
			}

			var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
			Fire(bullet);
			transform.localScale /= 1.5f;
			_powerCarrier.ModifyPower(-1);
			_timer.Reset();
		}

		private void Fire(GameObject bullet){
			var rigid = bullet.GetComponent<Rigidbody>();
			rigid.AddForce(rigid.transform.forward * 30f);
		}
	}
}