﻿using Project;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	[RequireComponent(typeof(XRBaseInteractor))]
	[RequireComponent(typeof(XRController))]
	public class Interactor : MonoBehaviour , ISelector{
		private XRController _controller;
		private XRBaseInteractor _interactor;


		private Transform _interactableTransform;

		private void Start(){
			_controller = GetComponent<XRController>();
			_interactor = GetComponent<XRBaseInteractor>();
			_interactor.selectEntered.AddListener(x => { _interactableTransform = x.interactableObject.transform; });
			_interactor.selectExited.AddListener(x => { _interactableTransform = null; });
		}

		private void Update(){
			var hasSelection = _interactor.hasSelection;
			var isEmpty = _interactableTransform == null;
			if(!hasSelection || isEmpty) return;
			DetectInput();
		}

		private void DetectInput(){
			var inputDevice = _controller.inputDevice;
			inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var isTrigger);
			inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out var isGrip);
			inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPadAxis);
			inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var isPrimary);
			inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var isSecondary);
			var inputDetected = new DeviceInputDetected(_interactableTransform.gameObject.GetInstanceID()){
				IsTrigger = isTrigger,
				IsGrip = isGrip,
				IsPrimary = isPrimary,
				IsSecondary = isSecondary,
				TouchPadAxis = touchPadAxis,
				Selector = this,
			};
			EventBus.Post(inputDetected);
		}

		public void CancelSelect(IXRSelectInteractable selectable){
			var hasSelection = _interactor.hasSelection;
			if(!hasSelection) return;
			var interactionManager = _interactor.interactionManager;
			interactionManager.SelectExit(_interactor , selectable);
		}
	}
}