using System;
using System.Collections.Generic;
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
		private VRType VRType => vibrateData.vrType;
		private XRRayInteractor _interactor;
		private ArmData _armData;
		private EnergyBallBehavior _energyBallBehavior;
		private XRController _xrController;
		

		private readonly Dictionary<AudioClip, int> _vibrateSoundCache = new Dictionary<AudioClip, int>();

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

			SetDataCache();
		}

		private void SetDataCache(){
			var dataList = vibrateData.phoenixVibrateDataList;
			var controllerType = HandIndex == 1 ? PXR_Input.VibrateController.Left : PXR_Input.VibrateController.Right;
			foreach(var data in dataList){
				var soundID = 0;
				PXR_Input.SaveVibrateByCache(data.phoenixClip, controllerType, PXR_Input.ChannelFlip.No,
					PXR_Input.CacheConfig.CacheNoVibrate, ref soundID);
				_vibrateSoundCache.Add(data.phoenixClip, soundID);
			}
		}

		private void OnGainEnergy(){
			var max = _armData.originalGrabDetectionRadius;
			var min = _armData.GrabDetectionRadiusMin;
			var current = _interactor.sphereCastRadius;
			var lerpValue = (current - min) / (max - min);
			switch(VRType){
				case VRType.Phoenix:
					var gainClip = vibrateData.FindClip("Gain_Energy");
					VibratePhoenix(gainClip);
					break;
				case VRType.Neo3:
					var settings = vibrateData.FindSetting("Gain_Energy");
					VibrateNeo3(lerpValue * settings.amplitude);
					break;
				case VRType.Oculus:
					settings = vibrateData.FindSetting("Gain_Energy");
					VibrateXR(lerpValue * settings.amplitude);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public PhoenixVibrateData FindSettings(string settingName){
			return vibrateData.FindSetting(settingName);
		}

		public void DynamicVibrateWithSetting(PhoenixVibrateData setting, float step){
			switch(VRType){
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
			switch(VRType){
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
			switch(VRType){
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

		public void SetControllerAmp(float amp){
			PXR_Input.SetControllerAmp(amp);
		}

		private void VibratePhoenix(AudioClip clip){
			if(!_vibrateSoundCache.ContainsKey(clip)){
				throw new Exception($"{clip.name} is not in cache");
			}

			var soundID = _vibrateSoundCache[clip];
			PXR_Input.StartVibrateByCache(soundID);
		}

		private void VibrateNeo3(float amplitude, float time = 0.2f){
			switch(HandIndex){
				case 1:
					PXR_Input.SetControllerVibrationEvent((uint)HandIndex - 1, 100, amplitude, (int)time * 100);
					break;
				case 2:
					PXR_Input.SetControllerVibrationEvent((uint)HandIndex - 1, 100, amplitude, (int)time * 100);
					break;
			}
		}

		private void VibrateXR(float amplitude, float time = 0.2f){
			if(_xrController == null) _xrController = GetComponent<XRController>();
			_xrController.SendHapticImpulse(amplitude, time);
		}
	}

	public enum VRType{
		Phoenix,
		Neo3,
		Oculus
	}
}