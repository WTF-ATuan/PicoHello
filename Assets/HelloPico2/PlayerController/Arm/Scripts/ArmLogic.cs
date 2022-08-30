using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Project;
using Unity.XR.PXR;
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
		public delegate void FloatAction (float data);
		public delegate void AxisAction (Vector2 data);
		public delegate void InputAction (DeviceInputDetected obj);
		public ValueAction OnEnergyChanged;
		public ValueAction OnGripTouch;
		public ValueAction OnGripUp;
		public ValueAction OnTriggerUp;
		public ValueAction OnTriggerDown;
		public ValueAction OnPrimaryAxisTouchUp;
		public ValueAction OnPrimaryAxisClick;
		public ValueAction OnPrimaryButtonClick;
		public ValueAction OnSecondaryButtonClick;
		public AxisAction OnPrimaryAxisInput;
        public InputAction OnUpdateInput;
		#endregion

		private void Start()
		{
            SetUpXR();
            
			controllerInteractor = _controller.GetComponent<Interactor>();

            data.WhenGainEnergy.AddListener(() =>
            GainEnergySound());

            data.WhenShootProjectile.AddListener(() =>
            EnergyBallSound());            

            data.WhenShootChargedProjectile.AddListener(() =>
            EventBus.Post(new AudioEventRequested(data.ShootChargedEnergyBallClipName, _controller.transform.position)));
        }
        List<string> GainEnergyClipNames = new List<string>();
        List<string> ShootEnergyBallClipNames = new List<string>();
        private void GainEnergySound()
        {
            if (GainEnergyClipNames.Count == 0)
                GainEnergyClipNames = new List<string>(data.GainEnergyBallClipName);

            var pick = AudioPlayerHelper.PlayRandomAudio(GainEnergyClipNames.ToArray(), _controller.transform.position);

            GainEnergyClipNames.Remove(pick);
        }
        private void EnergyBallSound() {
            if (ShootEnergyBallClipNames.Count == 0)
                ShootEnergyBallClipNames = new List<string>(data.ShootEnergyBallClipName);

            var pick = AudioPlayerHelper.PlayRandomAudio(ShootEnergyBallClipNames.ToArray(), _controller.transform.position);

            ShootEnergyBallClipNames.Remove(pick);
        }
        public void ChangeXRInteractSettings(float grabDetectionRadius, float grabDistance)
        {
            data.GrabDetectionRadius = grabDetectionRadius;
            data.GrabDistance = grabDistance;

            SetUpXR();
        }
        private void SetUpXR() {
            if (TryGetComponent<XRRayInteractor>(out rayInteractor))
            { 
                rayInteractor.sphereCastRadius = data.GrabDetectionRadius; 
                rayInteractor.maxRaycastDistance = data.GrabDistance; 

                data.originalGrabDistance = data.GrabDistance;
                data.originalGrabDetectionRadius = data.GrabDetectionRadius;
            }
            else
                throw new System.Exception("missing XRRayIneractor");
        }
		private void OnEnable()
		{
			EventBus.Subscribe<DeviceInputDetected>(OnDeviceInputDetected);
            EventBus.Subscribe<ReceiveDamageData>(ReceiveDamage);
            EventBus.Subscribe<GainArmorUpgradeData>(ArmorUpgrade); 

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

            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButton);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var secondaryButton);

            // PXR usage
            _controller.inputDevice.TryGetFeatureValue(PXR_Usages.grip1DAxis, out var grip1DAxis);

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
            if (isTriggerTouch < 0.1f && isGripTouch < 0.01f)
            {
                data.WhenNotTouchTriggerAndGrip?.Invoke();
            }
            if (isGripTouch <= data.GrabReleaseValue) {
                OnGripTouch?.Invoke(data);
            }
            if (!isGrip)
            {
                OnGripUp?.Invoke(data);
                data.currentGripFunctionTimer = 0;
                rayInteractor.maxRaycastDistance = data.originalGrabDistance;
                rayInteractor.sphereCastRadius = data.originalGrabDetectionRadius;
            }
            else 
            {
                data.currentGripFunctionTimer += Time.deltaTime;
                data.currentGripFunctionTimer = Mathf.Clamp(data.currentGripFunctionTimer,0,data.gripFunctionEffectiveTime);
                
                // Decreasing the raycast distance
                rayInteractor.maxRaycastDistance = Mathf.Lerp(data.originalGrabDistance, 0, data.currentGripFunctionTimer / data.gripFunctionEffectiveTime);                
                rayInteractor.sphereCastRadius = Mathf.Lerp(data.originalGrabDetectionRadius, 0, data.currentGripFunctionTimer / data.gripFunctionEffectiveTime);                
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
            if (primaryButton)
            {
                OnPrimaryButtonClick?.Invoke(data);
            }
            if (secondaryButton) 
            { 
                OnSecondaryButtonClick?.Invoke(data);                
            }
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

            //EventBus.Post(needEnergyEventData);
        }
        private void ArmorUpgrade(GainArmorUpgradeData data) {
            if (data.armorType != ArmorType.Nature)
                _data.ArmorController.ActiveArm(data.armorType, data.armorPart);
            else
                _data.ArmorController.AutoActiveWithOrder(data.armorType);
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
