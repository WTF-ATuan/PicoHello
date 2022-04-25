using Game.Project;
using HelloPico2.Hand.Scripts.Event;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.Interact.Grip_Gun;


namespace HelloPico2.Interact.weapon_Swicth{
	public class weaponSwicth : MonoBehaviour, ITrigger,ITouchPad, IPrimaryButton,ISecondaryButton{
		private XRBaseInteractable _interactable;
		private PowerCarrier _powerCarrier;
		[SerializeField] private GameObject bulletPrefab;
		[SerializeField] float _shootTime;
		private ColdDownTimer _timer;
		[SerializeField] private GameObject swordPrefab;
		
		[SerializeField] private GameObject shieldPrefab;

		
		private bool _isSelect;


		public  void Start(){
			_interactable = GetComponent<XRBaseInteractable>();
			_powerCarrier = GetComponent<PowerCarrier>();

			_interactable.selectEntered.AddListener(x => { _isSelect = true; });
			_interactable.selectExited.AddListener(x => { _isSelect = false; });

			_timer = new ColdDownTimer(_shootTime);

		}

		public void OnTrigger(float triggerValue){
			
			if (!_timer.CanInvoke()) return;
			if (!_isSelect || _powerCarrier.isEmpty)
			{
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
			rigid.AddForce(rigid.transform.forward * 50f);
		}

        public void OnTouchPad(Vector2 padAxis)
        {
			if (_isSelect && padAxis.y > 0.5f){
				SwrodType(true);
			}
        }

        public void OnPrimaryButtonClick()
        {
			if (!_timer.CanInvoke()) return;
			if (_isSelect)
			{
				ShieldType(true);
			}
		}

        public void OnSecondaryButtonClick()
        {
			SwrodType(true);
		}
		private void ShieldType(bool checkType)
        {
            if (checkType)
            {
				SwrodType(false);
			}
			shieldPrefab.SetActive(checkType);

		}
		private void SwrodType(bool checkType)
        {
			if (checkType)
			{
				ShieldType(false);
			}
			swordPrefab.SetActive(checkType);
		}
	}
}