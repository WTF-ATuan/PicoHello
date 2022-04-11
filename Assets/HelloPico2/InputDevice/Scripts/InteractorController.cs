using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class InteractorController{
		private XRBaseInteractor Interactor{ get; }
		private XRInteractionManager Manager{ get; }

		public InteractorController(XRBaseInteractor interactor, XRInteractionManager manager){
			Interactor = interactor;
			Manager = manager;
		}

		public void Select(IXRSelectInteractable interactable){
			Manager.SelectEnter(Interactor, interactable);
		}
	}
}