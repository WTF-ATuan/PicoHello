using HelloPico2.Hand.Scripts.Event;
using Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InteractorEventPoster : MonoBehaviour{
		private XRBaseInteractor _interactor;
		private XRController _controller;
		private UnityEngine.XR.InputDevice inputDevice => _controller.inputDevice;

		private void Start(){
			_controller = GetComponent<XRController>();
			_interactor = GetComponent<XRBaseInteractor>();
			RegisterEvent();
		}

		private void RegisterEvent(){
			_interactor.hoverEntered.AddListener(x => HandleHoverEvent(x, true));
			_interactor.hoverExited.AddListener(x => HandleHoverEvent(x, false));
			_interactor.selectEntered.AddListener(x => HandleSelectEvent(x, true));
			_interactor.selectExited.AddListener(x => HandleSelectEvent(x, false));
		}

		private void HandleHoverEvent(BaseInteractionEventArgs eventArgs, bool isEnter){
			if(isEnter){
				var hoverEnterEventArgs = (HoverEnterEventArgs)eventArgs;
				var interactable = hoverEnterEventArgs.interactableObject;
				var hoverObject = interactable.transform.gameObject;
				var handHoveredEvent = new HandHovered(hoverObject,inputDevice , true);
				EventBus.Post(handHoveredEvent);
			}
			else{
				var hoverExitEventArgs = (HoverExitEventArgs)eventArgs;
				var interactable = hoverExitEventArgs.interactableObject;
				var hoverObject = interactable.transform.gameObject;
				var handHoveredEvent = new HandHovered(hoverObject,inputDevice ,false);
				EventBus.Post(handHoveredEvent);
			}
		}

		private void HandleSelectEvent(BaseInteractionEventArgs eventArgs, bool isEnter){
			if(isEnter){
				var enterEventArgs = (SelectEnterEventArgs)eventArgs;
				var interactable = enterEventArgs.interactableObject;
				var selectObject = interactable.transform.gameObject;
				var selectedEvent = new HandSelected(selectObject,inputDevice ,true);
				EventBus.Post(selectedEvent);
			}
			else{
				var exitEventArgs = (SelectExitEventArgs)eventArgs;
				var interactable = exitEventArgs.interactableObject;
				var selectObject = interactable.transform.gameObject;
				var selectedEvent = new HandSelected(selectObject,inputDevice ,false);
				EventBus.Post(selectedEvent);
			}
		}
	}
}