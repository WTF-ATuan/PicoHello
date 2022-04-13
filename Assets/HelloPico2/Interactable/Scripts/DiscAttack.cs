using Game.Project;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interactable.Scripts{
	public class DiscAttack : MonoBehaviour{
		[SerializeField] private float chargeTime = 2f;
		private XRGrabInteractable _grabInteractable;
		private ColdDownTimer _timer;

		private InteractData _interactData;

		private void Start(){
			_grabInteractable = GetComponent<XRGrabInteractable>();
			_timer = new ColdDownTimer(chargeTime);
			_grabInteractable.selectEntered.AddListener(OnSelectEntered);
			_grabInteractable.selectExited.AddListener(OnSelectExited);
			_grabInteractable.hoverEntered.AddListener(OnHoverEntered);
		}

		private void OnHoverEntered(HoverEnterEventArgs obj){
			var interactor = obj.interactorObject;
			var controllerNode = GetControllerNode(interactor);
			var isSelected = _interactData.IsSelected;
			var isLeftHand = _interactData.ControllerNode == XRNode.LeftHand;
			var isRightHand = _interactData.ControllerNode == XRNode.RightHand;
			if(!isSelected) return;
			if(isLeftHand){
				if(controllerNode == XRNode.RightHand){
					ChangeToDisc();
				}
			}

			if(isRightHand){
				if(controllerNode == XRNode.LeftHand){
					ChangeToDisc();
				}
			}
		}

		private void OnSelectEntered(SelectEnterEventArgs obj){
			var interactor = obj.interactorObject;
			var controllerNode = GetControllerNode(interactor);
			_interactData.IsSelected = true;
			_interactData.ControllerNode = controllerNode;
		}

		private void OnSelectExited(SelectExitEventArgs obj){
			_interactData.IsSelected = false;
		}

		private XRNode GetControllerNode(IXRInteractor interactor){
			var controller = interactor.transform.GetComponent<XRController>();
			var controllerNode = controller.controllerNode;
			return controllerNode;
		}

		private void ChangeToDisc(){
			transform.localScale *= 2;
		}

		private struct InteractData{
			public bool IsSelected;
			public bool IsHovered;
			public XRNode ControllerNode;
		}
	}
}