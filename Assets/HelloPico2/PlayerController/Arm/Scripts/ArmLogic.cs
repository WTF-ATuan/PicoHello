using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Project;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;
using HelloPico2.InteractableObjects;
using HelloPico2.InputDevice.Scripts;
using HelloPico2.PlayerController.Player;

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
        private XRRayInteractor rayInteractor;
        public Interactor controllerInteractor { get; set; }

        #region Delegate
        public delegate void ValueAction (ArmData data);
		public delegate void AxisAction (Vector2 data);
		public delegate void InputAction (DeviceInputDetected obj);
		public ValueAction OnEnergyChanged;
		public ValueAction OnGripUp;
		public ValueAction OnTriggerUp;
		public ValueAction OnTriggerDown;
		public ValueAction OnPrimaryAxisTouchUp;
		public ValueAction OnPrimaryAxisClick;
		public AxisAction OnPrimaryAxisInput;
        public InputAction OnUpdateInput;
		#endregion

		private void Start()
		{
            SetUpXR();
            
			controllerInteractor = _controller.GetComponent<Interactor>();

            data.WhenGainEnergy.AddListener(() =>
            EventBus.Post(new AudioEventRequested(data.GainEnergyBallClipName, _controller.transform.position)));

            data.WhenShootProjectile.AddListener(() =>
            AudioPlayerHelper.PlayRandomAudio(data.ShootEnergyBallClipName, _controller.transform.position));            

            data.WhenShootChargedProjectile.AddListener(() =>
            EventBus.Post(new AudioEventRequested(data.ShootChargedEnergyBallClipName, _controller.transform.position)));
        }
        private void SetUpXR() {
            if (TryGetComponent<XRRayInteractor>(out rayInteractor))
            { 
                rayInteractor.sphereCastRadius = data.GrabDetectionRadius; 
                rayInteractor.maxRaycastDistance = data.GrabDistance; 
            }
            else
                throw new System.Exception("missing XRRayIneractor");
        }
		private void OnEnable()
		{
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
            EventBus.Subscribe<ReceiveDamageData>(ReceiveDamage);

            OnEnergyChanged += BroadCastEnergyAmount;
        }
        private void OnDisable()
		{
            OnEnergyChanged -= BroadCastEnergyAmount;
        }
        private void Update()
		{
            CheckInput();
        }
        private void CheckInput() {
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var isTriggerTouch);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out var isGripTouch);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var padAxisTouch);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var padAxisClick);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var padAxis);

            if (isTrigger)
            {
                OnTriggerDown?.Invoke(data);
            }
            else
            { 
                OnTriggerUp?.Invoke(data);
            }

            if (isTriggerTouch > 0 || isGripTouch > 0)
            {
                data.WhenTouchTriggerOrGrip?.Invoke();
            }
            if (isTriggerTouch < 0.1f && isGripTouch < 0.1f)
            {
                data.WhenNotTouchTriggerAndGrip?.Invoke();
            }

            if (!isGrip)
            {
                OnGripUp?.Invoke(data);
            }
            if (!padAxisTouch)
            {
                OnPrimaryAxisTouchUp?.Invoke(data);
            }
            if (padAxisClick)
            {
                OnPrimaryAxisClick?.Invoke(data);
            }
            //if (padAxis.magnitude >= 0.1f && padAxisTouch)
            //{
            //    OnPrimaryAxisInput(padAxis);
            //}
            OnPrimaryAxisInput?.Invoke(padAxis);
        }
        private void OnDeviceInputDetected(DeviceInputDetected obj)
        {
            if(obj.Selector.HandType == data.HandType)
                OnUpdateInput?.Invoke(obj);

			if (obj.Selector.SelectableObject == null) return;

			var interactable = obj.Selector.SelectableObject.GetComponent<InteractableBase>();
			var isTrigger = obj.IsTrigger;			
			var isTriggerTouch = obj.TriggerValue;			
            var isGrip = obj.IsGrip;
            var isGripTouch = obj.GripValue;
            var padAxisTouch = obj.IsPadTouch;
            var padAxisClick = obj.IsPadClick;
            var isPrimary = obj.IsPrimary;
            var padAxis = obj.TouchPadAxis;

            #region Updat Object events
            if (isGrip)
            {
                interactable.OnSelect(obj);
            }
            else
            {
                interactable.OnDrop();
            }

            if (isPrimary)
			{
				interactable.OnXOrAButton();
			}

			if (padAxis.magnitude > .1f)
			{
				interactable.OnTouchPad(padAxis);
			}
			#endregion
		}
        private void ReceiveDamage(ReceiveDamageData data)
        {            
            // Has 2 hands so split into half
            SpentEnergy(data.DamageAmount / 2);
        }
        public void SpentEnergy(float amount) {
            if (data.Energy - amount > 0)
                data.Energy -= amount;
            else
                data.Energy = 0;
        }
        public bool CheckHasEnergy() {
            return data.Energy > 0;
        }
        public bool CheckFullEnergy()
        {
            return data.Energy >= data.MaxEnergy;
        }

        private void BroadCastEnergyAmount(ArmData data)
        {
            NeedEnergyEventData needEnergyEventData = new NeedEnergyEventData();
            needEnergyEventData.Energy = data.Energy;
            needEnergyEventData.HandType = data.HandType;

            EventBus.Post(needEnergyEventData);
        }   
    }

    public class ArmGizmoDrawer
    {
        #if UNITY_EDITOR
        [DrawGizmo(GizmoType.Selected)]
        static void DrawGizmoForArmSettings(ArmLogic arm, GizmoType gizmoType)
        {
            Gizmos.color = new Color(1, 1, 0, .1f);

            Vector3 startDir, next;

            int rayAmount = Mathf.Clamp(Mathf.FloorToInt(100 * arm.data.GrabDetectionRadius / 3), 10, 100);
            float angle = 360f / rayAmount;
            for (int i = 0; i < rayAmount; i++)
            {
                startDir = arm.transform.right;
                next = arm.transform.position + Quaternion.Euler(0, 0, angle * i) * startDir * arm.data.GrabDetectionRadius;
                Gizmos.DrawRay(next, arm.transform.forward * arm.data.GrabDistance);
            }
        }
        #endif
    }
}
