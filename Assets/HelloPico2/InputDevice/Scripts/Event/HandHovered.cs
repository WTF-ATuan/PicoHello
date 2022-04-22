using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandHovered{
		public GameObject HoveredObject{ get; }
		public UnityEngine.XR.InputDevice inputDevice{ get; }
		public bool IsEnter{ get; }

		public int HoveredInstanceID{ get; }

		public HandHovered(GameObject hoveredObject, UnityEngine.XR.InputDevice inputDevice, bool isEnter){
			HoveredObject = hoveredObject;
			this.inputDevice = inputDevice;
			IsEnter = isEnter;
			HoveredInstanceID = hoveredObject.GetInstanceID();
		}
	}
}