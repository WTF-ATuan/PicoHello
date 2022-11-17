using System;
using HelloPico2.InputDevice.Scripts;
using Unity.XR.PXR;
using UnityEngine;
using HandType = HelloPico2.InputDevice.Scripts.HandType;

namespace HelloPico2.InteractableObjects{
	public class InteractCollidedHaptic : MonoBehaviour, IInteractCollide{
		public float strength;
		public int time;
		public HandType type;


		public void OnCollide(InteractType type1, Collider selfCollider){
			switch(type){
				case HandType.Left:
					PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.LeftController);
					break;
				case HandType.Right:
					PXR_Input.SetControllerVibration(strength, time, PXR_Input.Controller.RightController);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Action<InteractType, Collider> ColliderEvent{ get; }
	}
}