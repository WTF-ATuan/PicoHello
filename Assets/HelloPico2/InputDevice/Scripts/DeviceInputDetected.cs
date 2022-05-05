using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class DeviceInputDetected{
		public bool IsTrigger;
		public bool IsGrip;
		public bool IsPrimary;
		public bool IsSecondary;
		public Vector2 TouchPadAxis;
		public ISelector Selector;

		public bool IsSameObject(GameObject gameObject){
			var selectableObject = Selector.SelectableObject;
			if(selectableObject == null){
				return false;
			}

			var selectableID = selectableObject.GetInstanceID();
			var instanceID = gameObject.GetInstanceID();
			return instanceID.Equals(selectableID);
		}
	}
}