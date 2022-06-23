using System;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;

namespace HelloPico2{
	[Serializable]
	public class Interactable : ViewEventData{
		[LabelWidth(200)]
		[Required]
		public InteractableBase interactable;

	}
}