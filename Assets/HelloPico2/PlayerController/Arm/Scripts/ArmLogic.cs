using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;

namespace HelloPico2.PlayerController.Arm
{
    [RequireComponent(typeof(ArmData))]
    public class ArmLogic : MonoBehaviour
    {
        private ArmData _data;
        public ArmData data { 
            get {
                if (_data == null)
                    _data = GetComponent<ArmData>();

                return _data;
            } 
        }

		public XRController _controller;
		public Interactor controllerInteractor { get; set; }

        #region Delegate
        public delegate void ValueAction (ArmData data);
		public delegate void AxisAction (Vector2 data);
		public ValueAction OnEnergyChanged;
		public ValueAction OnGripUp;
		public ValueAction OnTriggerDown;
		public ValueAction OnPrimaryAxisTouchUp;
		public ValueAction OnPrimaryAxisClick;
		public AxisAction OnPrimaryAxisInput;
		#endregion

		private void Start()
		{
			controllerInteractor = _controller.GetComponent<Interactor>();

			data.WhenGainEnergy.AddListener(() =>
			Audio.AudioPlayer.PlayAudio(data.GainEnergyBallClipName, data.AudioSource));
			data.WhenShootProjectile.AddListener(() =>
			Audio.AudioPlayer.PlayAudio(data.ShootEnergyBallClipName, data.AudioSource));
			data.WhenShootChargedProjectile.AddListener(() =>
			Audio.AudioPlayer.PlayAudio(data.ShootChargedEnergyBallClipName, data.AudioSource));
		}
		private void OnEnable()
		{
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
			EventBus.Subscribe<GainEnergyEventData>(ChargeEnergy);
		}
		private void OnDisable()
		{

		}
        private void Update()
        {
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var isTriggerTouch);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out var isGripTouch);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
			_controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var padAxis);

			if (isTrigger)
			{
				OnTriggerDown(data);
			}

			if (isTriggerTouch > 0 || isGripTouch > 0) {
				data.WhenTouchTriggerOrGrip?.Invoke();
			}
			if (isTriggerTouch < 0.1f && isGripTouch < 0.1f) { 
				data.WhenNotTouchTriggerAndGrip?.Invoke();
			}

			if (!isGrip) {
				// Use all energy to shoot
				OnGripUp?.Invoke(data);
			}
			if (!padAxisTouch)
			{
				OnPrimaryAxisTouchUp(data);
			}
			if (padAxisClick)
			{
				OnPrimaryAxisClick(data);
			}
			if (padAxis.magnitude >= 0.1f && padAxisTouch) {
				OnPrimaryAxisInput(padAxis);
			}
		}

		// Not in use
        public void OnDeviceInputDetected(DeviceInputDetected obj)
        {
			if (obj.Selector.SelectableObject == null) return;

			var interactable = obj.Selector.SelectableObject.GetComponent<InteractableBase>();
            var isGrip = obj.IsGrip;
            var isPrimary = obj.IsPrimary;
			var isTrigger = obj.IsTrigger;
            var padAxis = obj.TouchPadAxis;

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
				OnTriggerDown(data);
			}

			if (padAxis.magnitude > .1f)
			{
				interactable.OnTouchPad(padAxis);
				OnPrimaryAxisInput(padAxis);
			}

		}
		private void ChargeEnergy(GainEnergyEventData eventData) {
			if (eventData.InputReceiver.Selector.HandType != data.HandType) return;

			data.Energy += eventData.Energy;

			controllerInteractor.CancelSelect(eventData.Interactable);

			var targetPos = _controller.transform.TransformPoint(data.GrabEnergyOffset);

			eventData.Interactable.transform.DOMove(targetPos, data.GrabEasingDuration).OnComplete(() =>
			{
				OnEnergyChanged?.Invoke(data);

				data.WhenGainEnergy?.Invoke();

				Destroy(eventData.Interactable.transform.gameObject);
			});
		}		
		// Not in use
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
