using UnityEngine;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandSelected{
		public GameObject SelectedObject{ get; }
		public bool IsEnter{ get; }
		public int SelectedInstanceID{ get; }

		public HandSelected(GameObject selectedObject, bool isEnter){
			SelectedObject = selectedObject;
			IsEnter = isEnter;
			SelectedInstanceID = selectedObject.GetInstanceID();
		}
	}
}