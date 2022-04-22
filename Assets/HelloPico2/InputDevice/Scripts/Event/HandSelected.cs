using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandSelected{
		public GameObject SelectedObject{ get; }
		public UnityEngine.XR.InputDevice inputDevice{ get; }
		public bool IsEnter{ get; }
		public int SelectedInstanceID{ get; }

		public HandSelected(GameObject selectedObject, UnityEngine.XR.InputDevice inputDevice, bool isEnter){
			SelectedObject = selectedObject;
			this.inputDevice = inputDevice;
			IsEnter = isEnter;
			SelectedInstanceID = selectedObject.GetInstanceID();
		}
	}
}