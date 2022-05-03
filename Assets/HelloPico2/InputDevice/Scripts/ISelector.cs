using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public interface ISelector{
		void CancelSelect(IXRSelectInteractable selectable);
	}
}