using HelloPico2.Hand.Scripts.Event;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interact.Grip_Gun{
	public class GripGun : MonoBehaviour, ITrigger{
		private XRBaseInteractable _interactable;
		private PowerCarrier _powerCarrier;
		[SerializeField] private GameObject bulletPrefab;


		private bool _isSelect;

		private void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_powerCarrier = GetComponent<PowerCarrier>();

			_interactable.selectEntered.AddListener(x => { _isSelect = true; });
			_interactable.selectExited.AddListener(x => { _isSelect = false; });
		}

		public void OnTrigger(float triggerValue){
			if(!_isSelect || _powerCarrier.isEmpty){
				var meshRenderer = GetComponentInChildren<MeshRenderer>();
				meshRenderer.material.color = Color.black;
			}

			var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation, transform);
			Fire(bullet);
			//todo In VR environment can,t change scale set; //transform.localScale *= 0.5f;  
			_powerCarrier.ModifyPower(-1);
		}

		private void Fire(GameObject bullet){
			var rigid = bullet.GetComponent<Rigidbody>();
			rigid.AddForce(Vector3.forward * 30f);
		}
	}
}