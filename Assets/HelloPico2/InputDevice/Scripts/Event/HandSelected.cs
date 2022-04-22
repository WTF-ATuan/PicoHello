using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts.Event{
	public class HandSelected{
		public GameObject SelectedObject{ get; }
		public IXRSelectInteractor selectInteractor{ get; }
		public bool IsEnter{ get; }
		public int SelectedInstanceID{ get; }

		public HandSelected(GameObject selectedObject, IXRSelectInteractor selectInteractor, bool isEnter){
			SelectedObject = selectedObject;
			this.selectInteractor = selectInteractor;
			IsEnter = isEnter;
			SelectedInstanceID = selectedObject.GetInstanceID();
		}
	}
}