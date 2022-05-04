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
		private readonly int _instanceID;


		public DeviceInputDetected(int instanceID){
			_instanceID = instanceID;
		}

		public bool IsSameObject(GameObject gameObject){
			var instanceID = gameObject.GetInstanceID();
			return instanceID.Equals(_instanceID);
		}
	}
}