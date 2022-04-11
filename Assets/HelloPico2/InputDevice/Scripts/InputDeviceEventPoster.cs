using HelloPico2.Hand.Scripts.Event;
using Project;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InputDeviceEventPoster : MonoBehaviour{
		private XRBaseInteractor interactor;

		private void Start(){
			interactor = GetComponent<XRBaseInteractor>();
			RegisterEvent();
		}

		private void RegisterEvent(){
			interactor.hoverEntered.AddListener(x => HandleHoverEvent(x, true));
			interactor.hoverExited.AddListener(x => HandleHoverEvent(x, false));
			interactor.selectEntered.AddListener(x => HandleSelectEvent(x, true));
			interactor.selectExited.AddListener(x => HandleSelectEvent(x, false));
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