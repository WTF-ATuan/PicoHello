using System;
using HelloPico2.InputDevice.Scripts;
using Unity.XR.PXR;
using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class BeamCollidedHaptic : MonoBehaviour, IBeamCollide{
		public float strength;
		public int time;
		public HandType type;


		public void OnCollide(){
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
	}
}