using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandHovered{
		public GameObject HoveredObject{ get; }
		public IXRHoverInteractor HoverInteractor{ get; }
		public bool IsEnter{ get; }

		public int HoveredInstanceID{ get; }

		public HandHovered(GameObject hoveredObject, IXRHoverInteractor hoverInteractor, bool isEnter){
			HoveredObject = hoveredObject;
			HoverInteractor = hoverInteractor;
			IsEnter = isEnter;
			HoveredInstanceID = hoveredObject.GetInstanceID();
		}
	}
}