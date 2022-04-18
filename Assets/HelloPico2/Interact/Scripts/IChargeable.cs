using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Interactable.Scripts{
	public interface IChargeable{
		float chargeTime{ get; }
		void Activate(XRGrabInteractable interactable);
	}
}