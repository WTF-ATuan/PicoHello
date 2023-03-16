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
using System;

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
        [SerializeField] private bool _EnableAxisInputSFX = false;
        [SerializeField] private bool _EnableGrip = true;
        private int UpDown = 0;
        private int CurrentUpDown = 0;
        public bool enableGrip { get { return _EnableGrip; }set { _EnableGrip = value; } }
        public Interactor controllerInteractor { get; set; }

        private bool triggerValue { get; set; }
        private bool primaryButtonValue { get; set; }
        private bool secondaryButtonValue { get; set; }
        public bool gripActivation;// { get; set; }
        private Game.Project.ColdDownTimer disableTimer { get; set; }

        #region Delegate
        public delegate void ValueAction (ArmData data);
		public delegate void FloatAction (float data);
		public delegate void AxisAction (Vector2 data);
		public delegate void InputAction (DeviceInputDetected obj);
		public ValueAction OnEnergyChanged;
		public ValueAction OnGripTouch;
		public ValueAction OnGripUp;
		public ValueAction OnGripDown;
		public ValueAction OnTriggerUp;
		public ValueAction OnTriggerUpOnce;
		public ValueAction OnTriggerDown;
		public ValueAction OnTriggerDownOnce;
		public ValueAction OnPrimaryAxisTouchUp;
		public ValueAction OnPrimaryAxisClick;
		public ValueAction OnPrimaryButtonClick;
		public ValueAction OnPrimaryButtonClickWhenNoAxisInput;
		public ValueAction OnPrimaryButtonClickOnce;
		public ValueAction OnSecondaryButtonClick;
		public ValueAction OnSecondaryButtonClickWhenNoAxisInput;
		public ValueAction OnSecondaryButtonClickOnce;
		public ValueAction OnPrimarySecondaryButtonUpWhenNoAxisInput;
		public AxisAction OnPrimaryAxisInput;
        public InputAction OnUpdateInput;
        public Action OnEnableInput;
        public Action OnDisableInput;
        #endregion

        List<string> GainEnergyClipNames = new List<string>();
        List<string> TempClipNames = new List<string>();
        bool TempIsRapid;

        private bool isTrigger;
        private bool secondaryButton;
        private bool primaryButton;
        private Vector2 padAxis;
        private bool padAxisClick;
        private bool padAxisTouch;
        private float isGripTouch;
        private bool isGrip;
        private float isTriggerTouch;

		private void Start()
		{
            SetUpXR();
            
			controllerInteractor = _controller.GetComponent<Interactor>();

            data.WhenGainEnergy.AddListener(() =>
            GainEnergySound());
            data.WhenGainEnergy.AddListener(() =>
            CheckFullEnergyBehavior());

            data.WhenShootProjectile.AddListener((isRapid) =>
            EnergyBallSound(isRapid));            

            data.WhenShootChargedProjectile.AddListener(() =>
            EventBus.Post(new AudioEventRequested(data.ShootBombClipName, _controller.transform.position)));

            disableTimer = new Game.Project.ColdDownTimer(data.DisableInputCoolDownDuration);

            StartCoroutine(GripActivation());
        }
        private void GainEnergySound()
        {
            if (GainEnergyClipNames.Count == 0)
                GainEnergyClipNames = new List<string>(data.GainEnergyBallClipName);

            var pick = AudioPlayerHelper.PlayRandomAudio(GainEnergyClipNames.ToArray(), _controller.transform.position);

            GainEnergyClipNames.Remove(pick);
        }
        private void EnergyBallSound(bool isRapid) {
            if (TempIsRapid != isRapid)
            {
                TempIsRapid = isRapid;
                TempClipNames.Clear();
            }

            if (isRapid)
                PlayRandomSound(data.ShootRapidEnergyBallClipName);
            else
                PlayRandomSound(data.ShootEnergyBallClipName);
        }
        private void PlayRandomSound(string[] ClipNames) {
            if (TempClipNames.Count == 0)
                TempClipNames = new List<string>(ClipNames);

            var pick = AudioPlayerHelper.PlayRandomAudio(TempClipNames.ToArray(), _controller.transform.position);

            TempClipNames.Remove(pick);
        }
        private void CheckFullEnergyBehavior() {
            if (!CheckFullEnergy()) return;

            data.WhenFullEnergy?.Invoke();
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
            if (_EnableAxisInputSFX) OnPrimaryAxisInput += PlayAxisInputSFX;
        }
        private void OnDisable()
		{
            OnEnergyChanged -= BroadCastEnergyAmount;
            if (_EnableAxisInputSFX) OnPrimaryAxisInput -= PlayAxisInputSFX;
        }      
        
        private void PlayAxisInputSFX(Vector2 inputData)
        {
            CurrentUpDown = Mathf.RoundToInt(inputData.y);
            
            if (CurrentUpDown == UpDown) return;
                        
            UpDown = CurrentUpDown;

            if (UpDown > 0)
                AudioPlayerHelper.PlayAudio(data.toWhipClipName, transform.position);
            else if (UpDown < 0)
                AudioPlayerHelper.PlayAudio(data.toShieldClipName, transform.position);
        }

        private void Update()
		{
            if(disableTimer.CanInvoke())
                CheckInput();
        }
        private void CheckInput() {            
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out isTrigger);            
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out isTriggerTouch);            
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out isGrip);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out isGripTouch);            
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out padAxisTouch);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out padAxisClick);
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out padAxis);

            _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);            
            _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);

            if (isTrigger)
            {
                OnTriggerDown?.Invoke(data);
            }
            else
            { 
                OnTriggerUp?.Invoke(data);
            }

            if (triggerValue != isTrigger)
            {
                triggerValue = isTrigger;

                if (triggerValue)
                    OnTriggerDownOnce?.Invoke(data);
                else
                    OnTriggerUpOnce?.Invoke(data);
            }

            if (isTriggerTouch > 0 || isGripTouch > 0)
            {
                data.WhenTouchTriggerOrGrip?.Invoke();
            }
            if (isTriggerTouch < 0.1f && isGripTouch < 0.01f)
            {
                data.WhenNotTouchTriggerAndGrip?.Invoke();
            }

            if (_EnableGrip)
            {
                if (isGripTouch <= data.GrabReleaseValue)
                {
                    OnGripTouch?.Invoke(data);
                }
                if (!isGrip)
                {
                    OnGripUp?.Invoke(data);
                    data.currentGripFunctionTimer = 0;
                    //rayInteractor.maxRaycastDistance = data.originalGrabDistance;
                    rayInteractor.sphereCastRadius = data.originalGrabDetectionRadius;

                    data.WhenNotGrip?.Invoke();
                }
                else
                {
                    OnGripDown?.Invoke(data);
                    data.currentGripFunctionTimer += Time.deltaTime;
                    data.currentGripFunctionTimer = Mathf.Clamp(data.currentGripFunctionTimer, 0, data.gripFunctionEffectiveTime);

                    // Decreasing the raycast distance
                    //rayInteractor.maxRaycastDistance = Mathf.Lerp(data.originalGrabDistance, 0, data.currentGripFunctionTimer / data.gripFunctionEffectiveTime);                
                    rayInteractor.sphereCastRadius = Mathf.Lerp(data.originalGrabDetectionRadius, data.GrabDetectionRadiusMin, data.currentGripFunctionTimer / data.gripFunctionEffectiveTime);

                    data.WhenGrip?.Invoke();
                }
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

            if (primaryButtonValue != primaryButton) 
            {
                primaryButtonValue = primaryButton;

                if (primaryButton) OnPrimaryButtonClickOnce?.Invoke(data);
            }
            if (secondaryButtonValue != secondaryButton) 
            {
                secondaryButtonValue = secondaryButton;

                if(secondaryButton) OnSecondaryButtonClickOnce?.Invoke(data);
            }

            if (!padAxisTouch && padAxis.magnitude < 0.1f)
            {
                if (secondaryButton)                
                    OnSecondaryButtonClickWhenNoAxisInput?.Invoke(data);
                else if (primaryButton)                
                    OnPrimaryButtonClickWhenNoAxisInput?.Invoke(data);                

                if(!primaryButton && !secondaryButton)
                    OnPrimarySecondaryButtonUpWhenNoAxisInput?.Invoke(data);
            }
            else
                OnPrimaryAxisInput?.Invoke(padAxis);
        }
        private void OnDeviceInputDetected(DeviceInputDetected obj)
        {
            //if (disableTimer.CanInvoke()) return;

            if (obj.Selector.HandType == data.HandType)
                OnUpdateInput?.Invoke(obj);

			if (obj.Selector.SelectableObject == null) return;

			var interactable = obj.Selector.SelectableObject.GetComponent<InteractableBase>();
			var isTrigger = obj.IsTrigger;			
			var isTriggerTouch = obj.TriggerValue;			
            var isGrip = obj.IsGrip;
            var gripValue = obj.GripValue;
            var isGripTouch = obj.GripValue;
            var padAxisTouch = obj.IsPadTouch;
            var padAxisClick = obj.IsPadClick;
            var isPrimary = obj.IsPrimary;
            var padAxis = obj.TouchPadAxis;

            #region Updat Object events   
            if (gripValue > data._GripDeadRange)
            {
                if (gripActivation)
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
        public void DisableInputContinuously()
        {
            disableTimer.ModifyDuring(float.MaxValue);
            disableTimer.Reset();
        }
        public void EnableInput()
        {
            disableTimer.ModifyDuring(data.DisableInputCoolDownDuration);
            disableTimer.Reset();
            OnEnableInput?.Invoke();
        }
        public void DisableInput() {
            disableTimer.Reset();
            OnDisableInput?.Invoke();
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
        private IEnumerator GripActivation() {
            while (true)
            {
                gripActivation = true;
                yield return new WaitForSeconds(5);
                gripActivation = false;
                yield return new WaitForSeconds(.5f);
            }
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
