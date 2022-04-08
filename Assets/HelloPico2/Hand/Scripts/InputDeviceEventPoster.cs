using HelloPico2.Hand.Scripts.Event;
using Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts{
	public class InputDeviceEventPoster : MonoBehaviour{
		private XRDirectInteractor _interactor;

		private void Start(){
			_interactor = GetComponent<XRDirectInteractor>();
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
				var handHoveredEvent = new HandHovered(hoverObject, true);
				EventBus.Post(handHoveredEvent);
			}
			else{
				var hoverExitEventArgs = (HoverExitEventArgs)eventArgs;
				var interactable = hoverExitEventArgs.interactableObject;
				var hoverObject = interactable.transform.gameObject;
				var handHoveredEvent = new HandHovered(hoverObject, false);
				EventBus.Post(handHoveredEvent);
			}
		}

		private void HandleSelectEvent(BaseInteractionEventArgs eventArgs, bool isEnter){
			if(isEnter){
				var enterEventArgs = (SelectEnterEventArgs)eventArgs;
				var interactable = enterEventArgs.interactableObject;
				var selectObject = interactable.transform.gameObject;
				var selectedEvent = new HandSelected(selectObject , true);
				EventBus.Post(selectedEvent);
			}
			else{
				var exitEventArgs = (SelectExitEventArgs)eventArgs;
				var interactable = exitEventArgs.interactableObject;
				var selectObject = interactable.transform.gameObject;
				var selectedEvent = new HandSelected(selectObject , false);
				EventBus.Post(selectedEvent);
			}
		}
	}
}