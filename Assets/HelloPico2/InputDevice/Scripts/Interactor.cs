using System;
using Project;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	[RequireComponent(typeof(XRBaseInteractor))]
	[RequireComponent(typeof(XRController))]
	public class Interactor : MonoBehaviour, ISelector{
		private XRController _controller;
		private XRBaseInteractor _interactor;
		private Transform _selectableTransform;
		private Vector3 _previousPosition = Vector3.zero;

		private void Start(){
			_controller = GetComponent<XRController>();
			_interactor = GetComponent<XRBaseInteractor>();
			_interactor.selectEntered.AddListener(x => { _selectableTransform = x.interactableObject.transform; });
			_interactor.selectExited.AddListener(x => { _selectableTransform = null; });
		}

		private void Update(){
			DetectInput();
		}

		private void FixedUpdate(){
			DetectVelocity();
		}

		private void DetectVelocity(){
			var currentPosition = transform.position;
			var currentOffset = currentPosition - _previousPosition;
			var speed = currentOffset.magnitude / Time.fixedDeltaTime;
			_previousPosition = currentPosition;
			Speed = speed;
		}

		public bool HasSelection => _interactor.hasSelection;
		public GameObject SelectableObject => _selectableTransform ? _selectableTransform.gameObject : null;
		public Transform SelectorTransform => _interactor.attachTransform;
		public float Speed{ get; private set; }

		public HandType HandType{
			get{
				if(_controller.controllerNode == XRNode.LeftHand) return HandType.Left;

				if(_controller.controllerNode == XRNode.RightHand) return HandType.Right;

				throw new ArgumentException($"{_controller.controllerNode} is not handType");
			}
		}

		public void CancelSelect(IXRSelectInteractable selectable){
			var hasSelection = _interactor.hasSelection;
			if(!hasSelection) return;
			var interactionManager = _interactor.interactionManager;
			interactionManager.SelectExit(_interactor, selectable);
		}

		public void StartSelect(IXRSelectInteractable selectable){
			var hasSelection = _interactor.hasSelection;
			if(hasSelection) return;
			var interactionManager = _interactor.interactionManager;
			interactionManager.SelectEnter(_interactor, selectable);
		}

		private void DetectInput(){
			var inputDevice = _controller.inputDevice;
			inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
			inputDevice.TryGetFeatureValue(CommonUsages.trigger, out var touchValue);
			inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
			inputDevice.TryGetFeatureValue(CommonUsages.grip, out var gripValue);
			inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var isPadTouch);
			inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var isPadClick);
			inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var isPrimary);
			inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var isSecondary);
			var inputDetected = new DeviceInputDetected{
				IsTrigger = isTrigger,
				TouchValue = touchValue,
				IsGrip = isGrip,
				GripValue = gripValue,
				IsPrimary = isPrimary,
				IsSecondary = isSecondary,
				TouchPadAxis = touchPadAxis,
				IsPadTouch = isPadTouch,
				IsPadClick = isPadClick,
				Selector = this,
			};
			EventBus.Post(inputDetected);
		}

		public void SetHaptic(float strength, int time){
			switch(HandType){
				case HandType.Left:
					PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.LeftController);
					break;
				case HandType.Right:
					PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.RightController);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public enum HandType{
		Left,
		Right
	}
}