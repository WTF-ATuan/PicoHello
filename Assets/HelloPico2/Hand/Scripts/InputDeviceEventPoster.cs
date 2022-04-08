using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

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
		}
		else{
			var hoverExitEventArgs = (HoverExitEventArgs)eventArgs;
		}
	}

	private void HandleSelectEvent(BaseInteractionEventArgs eventArgs, bool isEnter){
		if(isEnter){
			var selectEnterEventArgs = (SelectEnterEventArgs)eventArgs;
		}
		else{
			var exitEventArgs = (SelectExitEventArgs)eventArgs;
		}
	}
}