using UnityEngine;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandHovered{
		public GameObject HoveredObject{ get; }
		public bool IsEnter{ get; }

		public int HoveredInstanceID{ get; }

		public HandHovered(GameObject hoveredObject, bool isEnter){
			HoveredObject = hoveredObject;
			IsEnter = isEnter;
			HoveredInstanceID = hoveredObject.GetInstanceID();
		}
	}
}