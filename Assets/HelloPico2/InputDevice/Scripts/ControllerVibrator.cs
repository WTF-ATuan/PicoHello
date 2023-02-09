using System;
using HelloPico2.InteractableObjects;
using HelloPico2.PlayerController.Arm;
using Sirenix.OdinInspector;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HelloPico2.InputDevice.Scripts{
	public class ControllerVibrator : MonoBehaviour{
		public HandType handType;
		[Required] [InlineEditor] public VibrateData vibrateData;
		private VRType _vrType;
		private XRRayInteractor _interactor;
		private ArmData _armData;
		private EnergyBallBehavior _energyBallBehavior;
		private XRController xrController;

		private int _sourceIDL;
		private int _sourceIDR;


		private int HandIndex{
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
			if(_armData) _armData.WhenGrip += OnGainEnergy;

			if(_energyBallBehavior){
				_energyBallBehavior.swordBehavior.WhenCollide += HitVibrate;
				_energyBallBehavior.shieldBehavior.WhenCollide += HitVibrate;
			}

			if(vibrateData.autoSetType){
				AutoSetVibrateType();
			}
			else{
				_vrType = vibrateData.vrType;
			}
		}

		private void AutoSetVibrateType(){
			var controllerDevice = PXR_Input.GetControllerDeviceType();
			_vrType = controllerDevice == PXR_Input.ControllerDevice.PICO_4 ? VRType.Phoenix : VRType.Neo3;
		}

		private void OnGainEnergy(){
			var max = _armData.originalGrabDetectionRadius;
			var min = _armData.GrabDetectionRadiusMin;
			var current = _interactor.sphereCastRadius;
			var lerpValue = (current - min) / (max - min);
			if(_vrType == VRType.Phoenix || _vrType == VRType.Neo3){
				var settings = vibrateData.FindSetting("Gain_Energy");
				VibrateNeo3(lerpValue * settings.amplitude);
			}
			else{
				var settings = vibrateData.FindSetting("Gain_Energy");
				VibrateXR(lerpValue * settings.amplitude);
			}
		}

		public PhoenixVibrateData FindSettings(string settingName){
			return vibrateData.FindSetting(settingName);
		}

		public void DynamicVibrateWithSetting(PhoenixVibrateData setting, float step){
			switch(_vrType){
				case VRType.Phoenix:
					VibratePhoenix(setting.phoenixClip);
					break;
				case VRType.Neo3:
					VibrateNeo3(step * setting.amplitude);
					break;
				case VRType.Oculus:
					VibrateXR(step * setting.amplitude);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void VibrateWithSetting(string settingName){
			var setting = vibrateData.FindSetting(settingName);
			switch(_vrType){
				case VRType.Phoenix:
					VibratePhoenix(setting.phoenixClip);
					break;
				case VRType.Neo3:
					VibrateNeo3(setting.amplitude, setting.time);
					break;
				case VRType.Oculus:
					VibrateXR(setting.amplitude, setting.time);
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
			switch(_vrType){
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
						var setting = vibrateData.FindSetting("Hit_Whip");
						VibrateNeo3(setting.amplitude, setting.time);
					}

					if(isShield){
						var setting = vibrateData.FindSetting("Hit_Shield");
						VibrateNeo3(setting.amplitude, setting.time);
					}

					break;
				case VRType.Oculus:
					if(isWhip){
						var setting = vibrateData.FindSetting("Hit_Whip");
						VibrateXR(setting.amplitude, setting.time);
					}

					if(isShield){
						var setting = vibrateData.FindSetting("Hit_Shield");
						VibrateXR(setting.amplitude, setting.time);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void PhoenixVibrateTest(AudioClip clip){
			VibratePhoenix(clip);
		}
		public void Neo3VibrateTest(float amp){
			VibrateNeo3(amp , 1000);
		}

		public void SetControllerAmp(float amp){
			PXR_Input.SetControllerAmp(amp);
		}

		private void VibratePhoenix(AudioClip clip){
			switch(HandIndex){
				case 1:
					PXR_Input.StartVibrateBySharem(clip, PXR_Input.VibrateController.Left, PXR_Input.ChannelFlip.No,
						ref _sourceIDL);
					break;
				case 2:
					PXR_Input.StartVibrateBySharem(clip, PXR_Input.VibrateController.Right, PXR_Input.ChannelFlip.No,
						ref _sourceIDR);
					break;
			}
		}

		private void VibrateNeo3(float amplitude, float time = 0.2f){
			switch(HandIndex){
				case 1:
					PXR_Input.SetControllerVibration(amplitude, (int)time * 100 , PXR_Input.Controller.LeftController);
					break;
				case 2:
					PXR_Input.SetControllerVibration(amplitude, (int)time * 100 , PXR_Input.Controller.RightController);
					break;
			}
		}

		private void VibrateXR(float amplitude, float time = 0.2f){
			if(xrController == null) xrController = GetComponent<XRController>();
			xrController.SendHapticImpulse(amplitude, time);
		}
	}

	public enum VRType{
		Phoenix,
		Neo3,
		Oculus
	}
}