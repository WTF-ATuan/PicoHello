using System;
using HelloPico2.PlayerController.Arm;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class ControllerVibrator : MonoBehaviour{
		public HandType handType;
		public VRType vrType;
		private XRRayInteractor _interactor;
		private ArmData _armData;

		private int handIndex{
			get{
				return handType switch{
					HandType.Left => 1,
					HandType.Right => 2,
					_ => 0
				};
			}
		}

		private void Start(){
			_interactor = GetComponent<XRRayInteractor>();
			_armData = GetComponent<ArmData>();
			_armData.WhenGrip += OnGainEnergy;
		}

		private void OnGainEnergy(){
			var max = _armData.originalGrabDetectionRadius;
			var min = _armData.GrabDetectionRadiusMin;
			var current = _interactor.sphereCastRadius;
			var lerpValue = (current - min) / (max - min);
			if(lerpValue < 0.1) lerpValue = 0.1f;
			switch(vrType){
				case VRType.Phoenix:
					break;
				case VRType.Neo3:
					VibrateNeo3(lerpValue);
					break;
				case VRType.Oculus:
					VibrateXR(lerpValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void VibratePhoenix(AudioClip clip){
			PXR_Input.StartVibrateBySharem(clip, handIndex, 0);
		}

		private void VibrateNeo3(float amplitude){
			switch(handIndex){
				case 1:
					PXR_Input.SetControllerVibrationEvent((uint)handIndex - 1 , 100 ,amplitude, 200);
					break;
				case 2:
					PXR_Input.SetControllerVibrationEvent((uint)handIndex - 1 , 100 ,amplitude, 200);
					break;
			}
		}

		private void VibrateXR(float amplitude){
			var xrController = GetComponent<XRController>();
			xrController.SendHapticImpulse(amplitude, 0.2f);
		}
	}

	public enum VRType{
		Phoenix,
		Neo3,
		Oculus
	}
}