using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmData))]
    public class ArmLogic : MonoBehaviour
    {
        private ArmData _data;
        private ArmData data { 
            get {
                if (_data == null)
                    _data = GetComponent<ArmData>();

                return _data;
            } 
        }

		public XRController _controller;
		public Interactor controllerInteractor { get; set; }

		public delegate void ValueAction (ArmData data);
		public delegate void AxisAction (Vector2 data);
		public ValueAction OnEnergyChanged;
		public ValueAction OnGripUp;
		public ValueAction OnTriggerDown;
		public AxisAction OnPadInput;

		public bool _shoot;
		public GameObject test;
		
		private void Start()
		{
			controllerInteractor = _controller.GetComponent<Interactor>();
		}
		private void OnEnable()
		{
			ArmEventHandler.OnChargeEnergy += ChargeEnergy;

			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
		}
		private void OnDisable()
		{
			ArmEventHandler.OnChargeEnergy -= ChargeEnergy;
		}
        private void Update()
        {
			if (_shoot)
			{
				//sOnTriggerDown(data);
				//OnGripUp?.Invoke(data);
			}

			_controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);

			if (isTrigger)
			{
				test.SetActive(false);
				OnTriggerDown(data);
			}
			if (!isGrip) {
				// Use all energy to shoot
				OnGripUp?.Invoke(data);
			}
		}
        public void OnDeviceInputDetected(DeviceInputDetected obj)
        {
			if (obj.Selector.SelectableObject == null) return;

			var interactable = obj.Selector.SelectableObject.GetComponent<InteractableBase>();
            var isGrip = obj.IsGrip;
            var isPrimary = obj.IsPrimary;
			var isTrigger = obj.IsTrigger;
            var padAxis = obj.TouchPadAxis;

            CheckSummon(padAxis, isPrimary);

			if (isGrip)
			{
				interactable.OnSelect(obj);
			}
			else
			{
				interactable.OnDrop();
				// Use all energy to shoot
				OnGripUp?.Invoke(data);
			}

			if (isPrimary)
			{
				interactable.OnXOrAButton();
			}

			if (isTrigger)
			{
				print("isTriggered");
				test.SetActive(false);
				OnTriggerDown(data);
			}

			if (padAxis.magnitude > .1f)
			{
				interactable.OnTouchPad(padAxis);
				OnPadInput(padAxis);
			}

		}
        
        private void CheckSummon(Vector2 touchPadAxis, bool isTrigger) {
			if (touchPadAxis.y > 0 && isTrigger) {
				Summon(InteractableSettings.InteractableType.Sword);
			}
		}
		
		public void ChargeEnergy(float amount, IXRSelectInteractable interactable, DeviceInputDetected obj) {

			data.Energy += amount;

			controllerInteractor.CancelSelect(interactable);

			OnEnergyChanged?.Invoke(data);

			Destroy(interactable.transform.gameObject,2f);
		}
		public void Summon(InteractableSettings.InteractableType type) {

			if (data.currentWeapon != null) return;

			var objData = data.InteractableSettings.GetInteractableObject(type);

			// Check energy cost
			if (data.Energy < objData.energyCost) return;

			data.Energy -= objData.energyCost;

            data.currentWeapon = Instantiate(objData.prefab, data.SummonPoint);

            // select
			
        }
		
    }
}
