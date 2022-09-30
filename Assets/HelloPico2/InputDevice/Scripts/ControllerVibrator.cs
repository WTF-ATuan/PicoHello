﻿using System;
using HelloPico2.InteractableObjects;
using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class ControllerVibrator : MonoBehaviour{
		public HandType handType;
		public VRType vrType;
		[InlineEditor] public VibrateData vibrateData;
		private XRRayInteractor _interactor;
		private ArmData _armData;
		private EnergyBallBehavior _energyBallBehavior;

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
			_energyBallBehavior = GetComponent<EnergyBallBehavior>();
			_armData.WhenGrip += OnGainEnergy;
			_energyBallBehavior.swordBehavior.WhenCollide += HitVibrate;
			_energyBallBehavior.shieldBehavior.WhenCollide += HitVibrate;
		}

		private void OnGainEnergy(){
			var max = _armData.originalGrabDetectionRadius;
			var min = _armData.GrabDetectionRadiusMin;
			var current = _interactor.sphereCastRadius;
			var lerpValue = (current - min) / (max - min);
			switch(vrType){
				case VRType.Phoenix:
					var gainClip = vibrateData.FindClip("Gain_Energy");
					VibratePhoenix(gainClip);
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

		public void HitVibrate(InteractableSettings.InteractableType interactableType){
			print("Hit " + interactableType);
			var isShield = interactableType == InteractableSettings.InteractableType.Shield;
			var isWhip = interactableType == InteractableSettings.InteractableType.Whip |
						 interactableType == InteractableSettings.InteractableType.Sword;
			switch(vrType){
				case VRType.Phoenix:
					AudioClip hitClip = null;
					if(isShield){
						hitClip = vibrateData.FindClip("Hit_Shield");
					}

					if(isWhip){
						hitClip = vibrateData.FindClip("Hit_Whip");
					}

					VibratePhoenix(hitClip);
					break;
				case VRType.Neo3:
					if(isWhip){
						VibrateNeo3(0.5f, 0.3f);
					}

					if(isShield){
						VibrateNeo3(0.7f, 0.5f);
					}

					break;
				case VRType.Oculus:
					if(isWhip){
						VibrateXR(0.5f, 0.3f);
					}

					if(isShield){
						VibrateXR(0.7f, 0.5f);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void PhoenixVibrateTest(AudioClip clip){
			VibratePhoenix(clip);
		}

		public void SetControllerAmp(float amp){
			PXR_Input.SetControllerAmp(amp);
		}

		private void VibratePhoenix(AudioClip clip){
			PXR_Input.StartVibrateBySharem(clip, handIndex, 0);
		}

		private void VibrateNeo3(float amplitude, float time = 0.2f){
			switch(handIndex){
				case 1:
					PXR_Input.SetControllerVibrationEvent((uint)handIndex - 1, 100, amplitude, (int)time * 100);
					break;
				case 2:
					PXR_Input.SetControllerVibrationEvent((uint)handIndex - 1, 100, amplitude, (int)time * 100);
					break;
			}
		}

		private void VibrateXR(float amplitude, float time = 0.2f){
			var xrController = GetComponent<XRController>();
			xrController.SendHapticImpulse(amplitude, time);
		}
	}

	public enum VRType{
		Phoenix,
		Neo3,
		Oculus
	}
}