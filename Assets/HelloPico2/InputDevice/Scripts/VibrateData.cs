using System;
using System.Collections.Generic;
using HelloPico2.InputDevice.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.InputDevice{
	[CreateAssetMenu(fileName = "Phoenix Vibrate Data",
		menuName = "HelloPico2/ScriptableObject/ Phoenix Vibrate Data",
		order = 0)]
	public class VibrateData : ScriptableObject{
		public VRType vrType = VRType.Phoenix;

		public List<PhoenixVibrateData> phoenixVibrateDataList;

		public AudioClip FindClip(string vibrateName){
			var vibrateData = phoenixVibrateDataList.Find(x => x.vibrateName.Equals(vibrateName));
			if(vibrateData == null) throw new Exception($"{vibrateName} is not found in {this}");
			return vibrateData.phoenixClip;
		}

		public PhoenixVibrateData FindSetting(string vibrateName){
			var vibrateData = phoenixVibrateDataList.Find(x => x.vibrateName.Equals(vibrateName));
			if(vibrateData == null) throw new Exception($"{vibrateName} is not found in {this}");
			return vibrateData;
		}
	}

	[Serializable]
	public class PhoenixVibrateData{
		[HideLabel] public string vibrateName = "None";

		[TitleGroup("Phoenix")] public AudioClip phoenixClip;
		[TitleGroup("Neo3 & XR")] public float amplitude = 0.3f;
		[TitleGroup("Neo3 & XR")] public float time = 0.2f;
	}
}