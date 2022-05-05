using System;
using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class DeviceInputDetected{
		public bool IsTrigger;
		public bool IsGrip;
		public bool IsPrimary;
		public bool IsSecondary;
		public Vector2 TouchPadAxis;
		public ISelector Selector;
		public GameObject InteractableObject;
		public int InstanceID;

		public bool IsSameObject(GameObject gameObject){
			if(InstanceID.Equals(0)) return false;
			var instanceID = gameObject.GetInstanceID();
			return instanceID.Equals(InstanceID);
		}
	}
}