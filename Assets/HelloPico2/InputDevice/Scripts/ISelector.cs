using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public interface ISelector{
		void CancelSelect(IXRSelectInteractable selectable);
		void StartSelect(IXRSelectInteractable selectable);
		bool HasSelection{ get; }
		GameObject SelectableObject{ get; }
		Transform SelectorTransform{ get; }
	}
}