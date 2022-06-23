using System;
using HelloPico2.InteractableObjects;
using Sirenix.OdinInspector;

namespace HelloPico2{
	[Serializable]
	public class HitTarget : ViewEventData{
		[LabelWidth(200)]
		[Required]
		public HitTargetBase targetBase;
	}
}