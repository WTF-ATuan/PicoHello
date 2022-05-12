using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public interface ISelector{
		bool HasSelection{ get; }
		GameObject SelectableObject{ get; }
		Transform SelectorTransform{ get; }
		HandType HandType{ get; }
		void CancelSelect(IXRSelectInteractable selectable);
		void StartSelect(IXRSelectInteractable selectable);
		void SetHaptic(float strength, int time);
	}
}