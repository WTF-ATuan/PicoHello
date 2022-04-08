using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.Hand.Scripts{
	public class DeviceEventHandler : MonoBehaviour{

		private XRController _controller;

		private void Start(){
			_controller = GetComponent<XRController>();
			if(!_controller) throw new Exception("Controller is not found");
		}

		public void SendHapticImpulse(float amplitude = 0.5f, float duration = 2f){
			_controller.SendHapticImpulse(amplitude, duration);
		}
	}
}