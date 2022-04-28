using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	[RequireComponent(typeof(XRBaseInteractor))]
	[RequireComponent(typeof(XRController))]
	public class Interactor : MonoBehaviour{
		private XRController _controller;
		private XRBaseInteractor _interactor;


		private AttackBase _attackBase;

		private void Start(){
			_controller = GetComponent<XRController>();
			_interactor = GetComponent<XRBaseInteractor>();
			_interactor.selectEntered.AddListener(x => {
				_attackBase = x.interactableObject.transform.GetComponent<AttackBase>();
			});
		}

		private void Update(){
			var hasSelection = _interactor.hasSelection;
			var isEmpty = _attackBase == null;
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
			_attackBase.IsTrigger = isTrigger;
			_attackBase.IsGrip = isGrip;
			_attackBase.IsPrimary = isPrimary;
			_attackBase.IsSecondary = isSecondary;
			if(touchPadAxis.magnitude > 0.1f){
				_attackBase.OnTouchPad(touchPadAxis);
			}
		}
	}

	public abstract class AttackBase : MonoBehaviour{
		public bool IsTrigger{ private get; set; }
		public bool IsGrip{ private get; set; }
		public bool IsPrimary{ private get; set; }
		public bool IsSecondary{ private get; set; }
		public virtual void OnTouchPad(Vector2 touchPadAxis){ }
	}
}