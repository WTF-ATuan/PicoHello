using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class DeviceInputDetected{
		public bool IsTrigger{ get; set; }
		public bool IsGrip{ get; set; }
		public bool IsPrimary{ get; set; }
		public bool IsSecondary{ get; set; }
		public Vector2 TouchPadAxis{ get; set; }
		public float TouchValue{ get; set; }
		public float GripValue{ get; set; }
		public bool IsPadTouch{ get; set; }
		public bool IsPadClick{ get; set; }
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